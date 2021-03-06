using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

using SIL.FieldWorks.Common.RootSites;
using XCore;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Ling;
using SIL.Utils;
using SIL.FieldWorks.LexText.Controls;
using SIL.FieldWorks.Common.Framework.DetailControls;
using SIL.FieldWorks.Common.Utils;

namespace SIL.FieldWorks.XWorks.LexEd
{
	public class MSADlgLauncher : ButtonLauncher
	{
		private SIL.FieldWorks.XWorks.LexEd.MSADlglauncherView m_msaDlglauncherView;
		private System.ComponentModel.IContainer components = null;

		public MSADlgLauncher()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			Height = m_panel.Height;
		}

		/// <summary>
		/// Initialize the launcher.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="obj"></param>
		/// <param name="flid"></param>
		/// <param name="fieldName"></param>
		public override void Initialize(FdoCache cache, ICmObject obj, int flid, string fieldName,
			IPersistenceProvider persistProvider, Mediator mediator, string displayNameProperty, string displayWs)
		{
			CheckDisposed();

			base.Initialize(cache, obj, flid, fieldName, persistProvider, mediator, displayNameProperty, displayWs);
			m_msaDlglauncherView.Init(mediator, obj as MoMorphSynAnalysis);
		}

		/// <summary>
		/// Handle launching of the MSA editor.
		/// </summary>
		protected override void HandleChooser()
		{
			using (MsaCreatorDlg dlg = new MsaCreatorDlg())
			{
				MoMorphSynAnalysis originalMsa = m_obj as MoMorphSynAnalysis;
				ILexEntry entry = LexEntry.CreateFromDBObject(m_cache, originalMsa.OwnerHVO);
				dlg.SetDlgInfo(m_cache,
					m_persistProvider,
					m_mediator,
					entry,
					DummyGenericMSA.Create(originalMsa),
					originalMsa.Hvo,
					true,
					String.Format(LexEdStrings.ksEditX, Slice.Label));
				if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
				{
					DummyGenericMSA dummyMsa = dlg.DummyMSA;
					if (!originalMsa.EqualsMsa(dummyMsa))
					{
						m_cache.BeginUndoTask(LexEdStrings.ksUndoEditFunction,
							LexEdStrings.ksRedoEditFunction);
						// The UpdateOrReplace call may end up disposing this. So any variables we
						// need after it must be copied to the stack.
						FdoCache cache = m_cache;
						Slice parent = Slice;
						IMoMorphSynAnalysis newMsa = originalMsa.UpdateOrReplace(dummyMsa);
						cache.EndUndoTask();
					}
				}
			}
		}

		/// <summary>
		/// Get the mediator.
		/// </summary>
		protected override XCore.Mediator Mediator
		{
			get { return m_mediator; }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_msaDlglauncherView = new SIL.FieldWorks.XWorks.LexEd.MSADlglauncherView();
			this.m_panel.SuspendLayout();
			this.SuspendLayout();
			//
			// m_panel
			//
			this.m_panel.Name = "m_panel";
			//
			// m_btnLauncher
			//
			this.m_btnLauncher.Name = "m_btnLauncher";
			//
			// m_msaDlglauncherView
			//
			this.m_msaDlglauncherView.BackColor = System.Drawing.SystemColors.Window;
			this.m_msaDlglauncherView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_msaDlglauncherView.Group = null;
			this.m_msaDlglauncherView.Location = new System.Drawing.Point(0, 0);
			this.m_msaDlglauncherView.Mediator = null;
			this.m_msaDlglauncherView.Name = "m_msaDlglauncherView";
			this.m_msaDlglauncherView.ReadOnlyView = false;
			this.m_msaDlglauncherView.ScrollPosition = new System.Drawing.Point(0, 0);
			this.m_msaDlglauncherView.ShowRangeSelAfterLostFocus = false;
			this.m_msaDlglauncherView.Size = new System.Drawing.Size(128, 24);
			this.m_msaDlglauncherView.SizeChangedSuppression = false;
			this.m_msaDlglauncherView.TabIndex = 0;
			this.m_msaDlglauncherView.WsPending = -1;
			this.m_msaDlglauncherView.Zoom = 1F;
			//
			// MSADlgLauncher
			//
			this.Controls.Add(this.m_msaDlglauncherView);
			this.MainControl = this.m_msaDlglauncherView;
			this.Name = "MSADlgLauncher";
			this.Size = new System.Drawing.Size(150, 24);
			this.Controls.SetChildIndex(this.m_panel, 0);
			this.Controls.SetChildIndex(this.m_msaDlglauncherView, 0);
			this.m_panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
