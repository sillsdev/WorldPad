using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FdoUi;
using SIL.FieldWorks.Common.Utils;
using XCore;

namespace SIL.FieldWorks.XWorks
{
	/// <summary>
	/// Just a shell class for containing runtime Switches for controling the diagnostic output.
	/// This could go in any file in the XWorks namespace, It's just here as a starting point.
	/// </summary>
	public class RuntimeSwitches
	{
		/// Tracing variable - used to control when and what is output to the debug and trace listeners
		public static TraceSwitch RecordTimingSwitch = new TraceSwitch("XWorks_Timing", "Used for diagnostic timing output", "Off");
		public static TraceSwitch linkListenerSwitch = new TraceSwitch("XWorks_LinkListener", "Used for diagnostic output", "Off");
	}

	/// <summary>
	/// LinkListenerListener handles Hyper linking and history
	/// </summary>
	[XCore.MediatorDispose]
	public class LinkListener : IxCoreColleague, IFWDisposable
	{
		const int kmaxDepth = 50;		// Limit the stacks to 50 elements (LT-729).
		protected Mediator m_mediator;
		protected LinkedList<FwLink> m_backStack;
		protected LinkedList<FwLink> m_forwardStack;
		protected FwLink m_currentContext;

		private bool m_fFollowingLink = false;
		private int m_cBackStackOrig = 0;
		private FwLink m_lnkActive = null;
		private bool m_fUsingHistory = false;

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="LinkListener"/> class.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		public LinkListener()
		{
			m_backStack = new LinkedList<FwLink>();
			m_forwardStack = new LinkedList<FwLink>();
			m_currentContext = null;
		}

		#region IDisposable & Co. implementation
		// Region last reviewed: never

		/// <summary>
		/// Check to see if the object has been disposed.
		/// All public Properties and Methods should call this
		/// before doing anything else.
		/// </summary>
		public void CheckDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(String.Format("'{0}' in use after being disposed.", GetType().Name));
		}

		/// <summary>
		/// True, if the object has been disposed.
		/// </summary>
		private bool m_isDisposed = false;

		/// <summary>
		/// See if the object has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return m_isDisposed; }
		}

		/// <summary>
		/// Finalizer, in case client doesn't dispose it.
		/// Force Dispose(false) if not already called (i.e. m_isDisposed is true)
		/// </summary>
		/// <remarks>
		/// In case some clients forget to dispose it directly.
		/// </remarks>
		~LinkListener()
		{
			Dispose(false);
			// The base class finalizer is called automatically.
		}

		/// <summary>
		///
		/// </summary>
		/// <remarks>Must not be virtual.</remarks>
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Executes in two distinct scenarios.
		///
		/// 1. If disposing is true, the method has been called directly
		/// or indirectly by a user's code via the Dispose method.
		/// Both managed and unmanaged resources can be disposed.
		///
		/// 2. If disposing is false, the method has been called by the
		/// runtime from inside the finalizer and you should not reference (access)
		/// other managed objects, as they already have been garbage collected.
		/// Only unmanaged resources can be disposed.
		/// </summary>
		/// <param name="disposing"></param>
		/// <remarks>
		/// If any exceptions are thrown, that is fine.
		/// If the method is being done in a finalizer, it will be ignored.
		/// If it is thrown by client code calling Dispose,
		/// it needs to be handled by fixing the bug.
		///
		/// If subclasses override this method, they should call the base implementation.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (m_isDisposed)
				return;

			if (disposing)
			{
				// Dispose managed resources here.
				if (m_mediator != null)
				{
					m_mediator.RemoveColleague(this);
					m_mediator.PropertyTable.SetProperty("LinkListener", null, false);
					m_mediator.PropertyTable.SetPropertyPersistence("LinkListener", false);
				}
				if (m_backStack != null)
					m_backStack.Clear();
				if (m_forwardStack != null)
					m_forwardStack.Clear();
			}

			// Dispose unmanaged resources here, whether disposing is true or false.
			m_mediator = null;
			m_currentContext = null;
			m_backStack = null;
			m_forwardStack = null;

			m_isDisposed = true;
		}

		#endregion IDisposable & Co. implementation

		/// <summary>
		/// Return the current link.
		/// </summary>
		public FwLink CurrentContext
		{
			get
			{
				CheckDisposed();
				return m_currentContext;
			}
		}

		public void Init(Mediator mediator, XmlNode configurationParameters)
		{
			CheckDisposed();

			m_mediator = mediator;
			mediator.AddColleague(this);
			mediator.PropertyTable.SetProperty("LinkListener", this);
			mediator.PropertyTable.SetPropertyPersistence("LinkListener", false);
		}

		/// <summary>
		/// return an array of all of the objects which should
		/// 1) be queried when looking for someone to deliver a message to
		/// 2) be potential recipients of a broadcast
		/// </summary>
		/// <returns></returns>
		public IxCoreColleague[] GetMessageTargets()
		{
			CheckDisposed();

			return new IxCoreColleague[]{this};
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnAddContextToHistory(object _link)
		{
			CheckDisposed();

			//Debug.WriteLineIf(RuntimeSwitches.linkListenerSwitch.TraceInfo, "OnAddContextToHistory(" + m_currentContext + ")", RuntimeSwitches.linkListenerSwitch.DisplayName);
			FwLink lnk = (FwLink)_link;
			if (lnk.EssentiallyEquals(m_currentContext))
			{
				//Debug.WriteLineIf(RuntimeSwitches.linkListenerSwitch.TraceInfo, "   Link equals current context.", RuntimeSwitches.linkListenerSwitch.DisplayName);
				return true;
			}
			if (m_currentContext != null &&
				//not where we just came from via a "Back" call
				((m_forwardStack.Count == 0) || (m_currentContext != m_forwardStack.Last.Value)))
			{
				//Debug.WriteLineIf(RuntimeSwitches.linkListenerSwitch.TraceInfo, "  Pushing current to back: " + m_currentContext, RuntimeSwitches.linkListenerSwitch.DisplayName);
				Push(m_backStack, m_currentContext);
			}
			// Try to omit intermediate targets which are added to the stack when switching
			// tools.  This doesn't work in OnFollowLink() because the behavior of following
			// the link is not synchronous even when SendMessage is used at the first two
			// levels of handling.
			if (m_fFollowingLink && lnk.EssentiallyEquals(m_lnkActive))
			{
				int howManyAdded = m_backStack.Count - m_cBackStackOrig;
				for( ; howManyAdded > 1; --howManyAdded)
				{
					m_backStack.RemoveLast();
				}
				m_fFollowingLink = false;
				m_cBackStackOrig = 0;
				m_lnkActive = null;
			}
			// The forward stack should be cleared by jump operations that are NOT spawned by
			// a Back or Forward (ie, history) operation.  This is the standard behavior in
			// browsers, for example (as far as I know).
			if (m_fUsingHistory)
			{
				if (lnk.EssentiallyEquals(m_lnkActive))
				{
					m_fUsingHistory = false;
					m_lnkActive = null;
				}
			}
			else
			{
				m_forwardStack.Clear();
			}

			m_currentContext = lnk;
			return true;
		}

		private void Push(LinkedList<FwLink> stack, FwLink context)
		{
			stack.AddLast(context);
			while (stack.Count > kmaxDepth)
				stack.RemoveFirst();
		}

		private FwLink Pop(LinkedList<FwLink> stack)
		{
			FwLink lnk = stack.Last.Value;
			stack.RemoveLast();
			return lnk;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnCopyLocationAsHyperlink(object unused)
		{
			CheckDisposed();

			if (m_currentContext!= null)
				Clipboard.SetDataObject(m_currentContext.ToString(),true);
			return true;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnHistoryBack(object unused)
		{
			CheckDisposed();

			if (m_backStack.Count > 0)
			{
				if (m_currentContext!= null)
				{
					Push(m_forwardStack, m_currentContext);
				}
				m_fUsingHistory = true;
				m_lnkActive = Pop(m_backStack);
				FollowActiveLink();
			}

			return true;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnHistoryForward(object unused)
		{
			CheckDisposed();

			if (m_forwardStack.Count > 0)
			{
				m_fUsingHistory = true;
				m_lnkActive = Pop(m_forwardStack);
				FollowActiveLink();
			}
			return true;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnDisplayHistoryForward(object commandObject, ref UIItemDisplayProperties display)
		{
			CheckDisposed();

			display.Enabled = m_forwardStack.Count > 0;
			return true;
		}
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnDisplayHistoryBack(object commandObject, ref UIItemDisplayProperties display)
		{
			CheckDisposed();

			display.Enabled = m_backStack.Count > 0;
			return true;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnTestFollowLink(object unused)
		{
			CheckDisposed();

			FdoCache cache = (FdoCache)m_mediator.PropertyTable.GetValue("cache");
			int[] hvos =cache.LangProject.LexDbOA.EntriesOC.HvoArray;

			m_mediator. SendMessage("FollowLink",
				FwLink.Create("lexiconEdit",
					cache.GetGuidFromId(hvos[hvos.Length -1])/*the last one*/,
					cache.ServerName,
					cache.DatabaseName));
			return true;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public bool OnFollowLink(object lnk)
		{
			CheckDisposed();

			m_fFollowingLink = true;
			m_cBackStackOrig = m_backStack.Count;
			m_lnkActive = lnk as FwLink;

			bool b = FollowActiveLink();
			return b;
		}

		private bool FollowActiveLink()
		{
			try
			{
				if (m_lnkActive.TargetGuid != Guid.Empty)
				{
					// allow tools to skip loading a record if we're planning to jump to one.
					// interested tools will need to reset this "JumpToRecord" property after handling OnJumpToRecord.
					m_mediator.PropertyTable.SetProperty("SuspendLoadingRecordUntilOnJumpToRecord",
						m_lnkActive.ToolName + "," + m_lnkActive.TargetGuid.ToString(),
						PropertyTable.SettingsGroup.LocalSettings);
				}
				m_mediator.SendMessage("SetToolFromName", m_lnkActive.ToolName);
				// Note: It can be Guid.Empty in cases where it was never set,
				// or more likely, when the HVO was set to -1.
				if (m_lnkActive.TargetGuid != Guid.Empty)
				{
					FdoCache cache = (FdoCache)m_mediator.PropertyTable.GetValue("cache");
					// Allow this to happen after the processing of the tool change above by using the Broadcast
					// method on the mediator, the SendMessage would process it before the above msg and it would
					// use the wrong RecordList.  (LT-3260)
					m_mediator.BroadcastMessageUntilHandled("JumpToRecord", cache.GetIdFromGuid(m_lnkActive.TargetGuid));
				}

				foreach (Property property in m_lnkActive.PropertyTableEntries)
				{
					m_mediator.PropertyTable.SetProperty(property.name, property.value);
					//TODO: I can't think at the moment of what to do about setting
					//the persistence or ownership of the property...at the moment the only values we're putting
					//in there are strings or bools
				}
				m_mediator.BroadcastMessageUntilHandled("LinkFollowed", m_lnkActive);
			}
			catch(Exception err)
			{
				string s;
				if (err.InnerException != null && err.InnerException.Message != null && err.InnerException.Message.Length > 0)
					s = String.Format(xWorksStrings.UnableToFollowLink0, err.InnerException.Message);
				else
					s = xWorksStrings.UnableToFollowLink;
				MessageBox.Show(s, xWorksStrings.FailedJump, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
				return false;
			}

			return true;	//we handled this.
		}
	}
}
