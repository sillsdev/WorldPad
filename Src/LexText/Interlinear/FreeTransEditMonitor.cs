using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.LangProj;
using SIL.FieldWorks.FDO.Scripture;

namespace SIL.FieldWorks.IText
{
	/// <summary>
	/// This class monitors edits to a particular writing system of CmAnnotation.Comment.
	/// By implementing IVwNotifyChange, it receives notifications of edits, and keeps
	/// track if the change is to the relevant field and writing system of a CmIndirectAnnotation
	/// that is the Free Translation of a segment of a paragraph of Scripture.
	/// When it is told to disconnect, or notified of a selection change to a different property,
	/// if there have been relevant edits it updates the relevant CmTranslation of the paragraph.
	///
	/// For an example of the standard use pattern, see refs to m_ftMonitor in InterlinDocView.
	/// </summary>
	public abstract class EditMonitor : IVwNotifyChange, IDisposable
	{
		protected readonly FdoCache m_cache;
		protected readonly int m_ws;
		// If this is null, we have not detected any edits to an interesting object. If set, it is the
		// object we are currently editing; we will need to perform our monitor action when we lose focus etc.
		protected CmObject m_objectToMonitor;
		// During certain operations, especially converting CmTranslation to Segmented, we don't want
		// any other changes made; that conversion can happen while expanding a lazy box, and PropChanged
		// during that can get messy.
		private static bool s_fDisableAll;

		/// <summary>
		/// Used to disable editor monitoring temporarily.
		/// Usage: using new EditMonitor.DisableEditMonitors() {some action }
		/// </summary>
		public class DisableEditMonitors: IDisposable
		{
			public DisableEditMonitors()
			{
				s_fDisableAll = true;
			}

			public void Dispose()
			{
				s_fDisableAll = false;
			}
		}

		public EditMonitor(FdoCache cache, int transWs)
		{
			m_cache = cache;
			m_ws = transWs;
			cache.MainCacheAccessor.AddNotification(this);
		}

		/// <summary>
		/// Call this method when the containing control loses focus. It is also called in other cases
		/// where we wish to update the main translation from the interlinear one if any PropChanged's have been
		/// detected.
		/// </summary>
		public void LoseFocus()
		{
			if (m_objectToMonitor != null)
				DoMonitorAction();
		}

		private bool m_fInUpdate;

		/// <summary>
		/// This is the core method that actually updates the CmTranslation from the segmented one.
		/// </summary>
		private void DoMonitorAction()
		{
			m_fInUpdate = true;
			try
			{
				MonitorAction();
			}
			finally
			{
				m_fInUpdate = false;
				m_objectToMonitor = null;
			}
		}

		/// <summary>
		/// The object being currently edited is hvo. If it is the kind of object you are interested in, return the object;
		/// otherwise, return null.
		/// </summary>
		/// <param name="hvo"></param>
		/// <returns></returns>
		protected abstract CmObject GetObjectToMonitor(int hvo);

		/// <summary>
		/// Some change has happened to the object being monitored, and it is time to update whatever dependent thing
		/// you want to. Implement this method to actually do it.
		/// </summary>
		protected abstract void MonitorAction();

		/// <summary>
		/// The property we are interested in monitoring.
		/// </summary>
		protected abstract int PropertyOfInterest { get; }
		/// <summary>
		/// Called by the Views code when anything changes, this tests whether what is changing is interesting
		/// (or whether something else has started changing after we noted an interesting change) and takes
		/// appropriate action.
		/// </summary>
		public void PropChanged(int hvo, int tag, int ivMin, int cvIns, int cvDel)
		{
			if (m_fInUpdate || s_fDisableAll)
				return; // We don't need to process the changes we make ourself!
			if (m_cache.ActionHandlerAccessor == null)
				return; // We're in the middle of some suppressed-undo task, we only need to switch for user-visible changes.
			if (m_cache.ActionHandlerAccessor.IsUndoOrRedoInProgress)
				return; // side effects should be undone by their own UndoActions.
			// If it's the same object we're already monitoring, no need to check its type.
			CmObject newMonitorObject;
			if (m_objectToMonitor != null && hvo == m_objectToMonitor.Hvo)
				newMonitorObject = m_objectToMonitor;
			else
				newMonitorObject = GetObjectToMonitor(hvo);

			if (m_objectToMonitor != null)
			{
				// If we already know about some change that is interesting, and the new change
				// is not a further change to the same property of that same object, go ahead and do your action.
				if (newMonitorObject == null || newMonitorObject.Hvo != m_objectToMonitor.Hvo || tag != PropertyOfInterest)
					DoMonitorAction();
				else if (ShouldActAtOnce())
				{
					DoMonitorAction();
					return;
					// No need to remember the object, it's up to date.
				}
			}
			if (tag == PropertyOfInterest)
				SetMonitorObject(newMonitorObject); // remember this one has a modified comment so we need to update eventually.
		}

		/// <summary>
		/// Override this if there are circumstances in which the monitor action should be performed
		/// immediately on PropChanged, even though there is no change in the current object.
		/// </summary>
		protected virtual bool ShouldActAtOnce()
		{
			return false;
		}

		/// <summary>
		/// Record a new object to monitor. If there was an old one, this is called after running DoMonitorAction()
		/// on the old one. It is guaranteed to be called before DoMonitorAction, and the monitored object
		/// will not be changed in any other way.
		/// </summary>
		/// <param name="newMonitorObject"></param>
		/// <returns></returns>
		protected virtual void SetMonitorObject(CmObject newMonitorObject)
		{
			m_objectToMonitor = newMonitorObject;
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

		~EditMonitor()
		{
			Dispose(false);
		}
		protected void Dispose(bool disposingProperly)
		{
			if (disposingProperly)
			{
				if (m_objectToMonitor != null)
				{
					if (m_cache.IsDisposed)
						Debug.WriteLine("Edit monitor not updated before dispose!");
					else
						DoMonitorAction();
				}
				if (!m_cache.IsDisposed)
					m_cache.MainCacheAccessor.RemoveNotification(this);
			}
		}
	}

	/// <summary>
	/// This class monitors edits to a particular writing system of CmAnnotation.Comment.
	/// By implementing IVwNotifyChange, it receives notifications of edits, and keeps
	/// track if the change is to the relevant field and writing system of a CmIndirectAnnotation
	/// that is the Free Translation of a segment of a paragraph of Scripture.
	/// When it is told to disconnect, or notified of a selection change to a different property,
	/// if there have been relevant edits it updates the relevant CmTranslation of the paragraph.
	///
	/// For an example of the standard use pattern, see refs to m_ftMonitor in InterlinDocView.
	/// </summary>
	public class FreeTransEditMonitor : EditMonitor
	{
		private int m_hvoFtDefn; // The annotation defn that identifies free translations.
		private ScrTxtPara m_para; // the paragraph that corresponds to our monitored annotation.
		//// If this is null, we have not detected any edits to an interesting free translation. If set, it is the
		//// free translation we are currently editing; we will need to update the CmTranslation of the corresponding
		//// paragraph when we are done editing it.
		//private CmIndirectAnnotation m_currentFt;

		public FreeTransEditMonitor(FdoCache cache, int transWs)
			: base(cache, transWs)
		{
			m_hvoFtDefn = m_cache.GetIdFromGuid(new Guid(LangProject.kguidAnnFreeTranslation));
		}
		// If hvo is an CmIndirectAnnotation that is the free translation of a segment of Scripture, return the
		// annotation; otherwise return null.
		CmIndirectAnnotation GetAnnotation(int hvo)
		{
			if (m_cache.GetClassOfObject(hvo) != CmIndirectAnnotation.kclsidCmIndirectAnnotation)
				return null;
			// Return result unless it is disqualified somehow.
			CmIndirectAnnotation result = CmObject.CreateFromDBObject(m_cache, hvo) as CmIndirectAnnotation;
			if (result.AnnotationTypeRAHvo != m_hvoFtDefn)
				return null; // not a free translation
			if (result.AppliesToRS.Count == 0)
				return null; // huh? all FTs should applyTo their segment.
			CmBaseAnnotation cbaSeg = result.AppliesToRS[0] as CmBaseAnnotation;
			if (cbaSeg == null)
				return null; // huh? again for same reason
			StTxtPara para = cbaSeg.BeginObjectRA as StTxtPara;
			if (para == null)
				return null; // also unlikely, a  segment should link to a paragraph
			// Make sure the paragraph belongs to Scripture.
			if (!ScrTxtPara.IsScripturePara(para.Hvo, m_cache))
				return null; // not Scripture
			return result; // finally!
		}

		public static void UpdateMainTransFromSegmented(StTxtPara para, int[] wss)
		{
			if (!para.IsValidObject())
				return; // in merge, paragraph may be modified then deleted.
			FdoCache cache = para.Cache;
			BtConverter.EnsureMainParaSegments(para, wss[0]);
			ISilDataAccess sda = cache.MainCacheAccessor;
			List<int> segments = para.Segments;
			int kflidFT = StTxtPara.SegmentFreeTranslationFlid(cache);
			ITsString tssContents = para.Contents.UnderlyingTsString;
			IScripture scr = para.Cache.LangProject.TranslatedScriptureOA;
			ICmTranslation originalBT = para.GetBT(); // Can be null
			string sUnfinished = BackTranslationStatus.Unfinished.ToString();
			foreach (int ws in wss)
			{
				ITsStrBldr bldr = TsStrBldrClass.Create();
				bool wantNextSpace = false; // suppresses space before the first thing we add.
				bool haveBtText = false; // Text that isn't segment label text
				foreach (int hvoSeg in segments)
				{
					// If it's a label, insert it directly. Suppress following space.
					int beginOffset = sda.get_IntProp(hvoSeg, (int) CmBaseAnnotation.CmBaseAnnotationTags.kflidBeginOffset);
					int endOffset = sda.get_IntProp(hvoSeg, (int) CmBaseAnnotation.CmBaseAnnotationTags.kflidEndOffset);
					ITsString tssFt;
					// Whether we want to insert a space before the current segment is determined by the previous one.
					// Save that value so we can set wantSpace appropriately for the following one.
					bool wantSpace = wantNextSpace;
					if (SegmentBreaker.HasLabelText(tssContents, beginOffset, endOffset))
					{
						tssFt = (new CmBaseAnnotation(cache, hvoSeg)).TextAnnotated;
						tssFt = scr.ConvertCVNumbersInStringForBT(CorrectFootnotes(tssFt), ws);
						wantNextSpace = false;
					}
					else
					{
						int hvoFt = sda.get_ObjectProp(hvoSeg, kflidFT);
						tssFt = sda.get_MultiStringAlt(hvoFt, (int) CmAnnotation.CmAnnotationTags.kflidComment, ws);
						haveBtText |= (tssFt.Length > 0);
						wantNextSpace = EndsWithNonSpace(tssFt);
					}
					if (tssFt.Length > 0)
					{
						if (wantSpace)
						{
							// The preceding segment should typically be followed by a space.
							if (!StartsWithSpaceOrOrc(tssFt))
								bldr.Replace(bldr.Length, bldr.Length, " ", null);
						}
						bldr.ReplaceTsString(bldr.Length, bldr.Length, tssFt);
					}
				}

				// If the back translation doesn't have text, we don't want to create verse
				// segment labels. This prevents the problem where the book thinks it has a
				// back translation because of automatically generated verse labels (TE-8283).
				if (!haveBtText)
				{
					// This check might not be needed, but it shouldn't hurt anything.
					if (originalBT != null)
					{
						if (originalBT.Translation.GetAlternative(ws).Length > 0)
						{
							string origStatus = originalBT.Status.GetAlternative(ws);
							if (!String.IsNullOrEmpty(origStatus) && origStatus != sUnfinished)
								originalBT.Status.SetAlternative(sUnfinished, ws);
						}
					}
					continue;
				}

				ITsString newFt = bldr.GetString();
				ICmTranslation trans;
				if (newFt.Length == 0)
				{
					trans = para.GetBT();
					if (trans == null)
						return; // don't bother creating one to store an empty translation!
				}
				else
				{
					trans = para.GetOrCreateBT();

				}
				// Don't write unless it changed...PropChanged can be expensive.
				if (!trans.Translation.GetAlternative(ws).UnderlyingTsString.Equals(newFt))
				{
					trans.Translation.SetAlternative(newFt, ws);
					trans.Status.SetAlternative(sUnfinished, ws);
				}
			}
		}

		/// <summary>
		/// It's important to update the CmTranslation at once if the segmented one is empty,
		/// because if all the segmented one is entirely empty and we Refresh, the out-of-date CmTranslation
		/// may get copied back to the segmented one (see TE-7902).
		/// </summary>
		/// <returns></returns>
		protected override bool ShouldActAtOnce()
		{
			CmAnnotation ft = m_objectToMonitor as CmAnnotation;
			if (ft == null)
				return false; // paranoia
			return String.IsNullOrEmpty(ft.Comment.GetAlternativeTss(m_ws).Text);
		}

		/// <summary>
		/// True if a string starts with a space or ORC and so does not require a space inserted before it.
		/// (Also if it is empty...don't need to put a space if we aren't going to put something after it.)
		/// </summary>
		/// <param name="tssFt"></param>
		/// <returns></returns>
		private static bool StartsWithSpaceOrOrc(ITsString tssFt)
		{
			if (tssFt.Length == 0)
				return true;
			char first = tssFt.GetChars(0, 1)[0];
			return first == StringUtils.kchObject || first == ' ';
		}

		/// <summary>
		/// True if the string ends with a non-space (and so needs a space inserted after it).
		/// </summary>
		/// <param name="tssFt"></param>
		/// <returns></returns>
		private static bool EndsWithNonSpace(ITsString tssFt)
		{
			int length = tssFt.Length;
			if (length == 0)
				return true;
			return tssFt.GetChars(length - 1, length) != " ";
		}

		/// <summary>
		/// Any ORCs in the given string with ktptObjData of type kodtOwnNameGuidHot (meaning a GUID that
		/// 'owns' the footnote) should be changed to kodtNameGuidHot, since the BT does NOT own
		/// the footnote.
		/// </summary>
		/// <param name="tssFt"></param>
		/// <returns></returns>
		private static ITsString CorrectFootnotes(ITsString tssFt)
		{
			ITsStrBldr bldr = null;
			int crun = tssFt.RunCount;
			for (int iRun = 0; iRun < crun; iRun++)
			{
				string sOrc = tssFt.get_RunText(iRun);
				if (String.IsNullOrEmpty(sOrc))
					continue;
				if (StringUtils.kchObject != sOrc[0])
					continue;
				ITsTextProps orcPropsParaFootnote = tssFt.get_Properties(iRun);
				string objData = orcPropsParaFootnote.GetStrPropValue((int) FwTextPropType.ktptObjData);
				if (String.IsNullOrEmpty(objData))
					continue;
				if((char) (int) FwObjDataTypes.kodtOwnNameGuidHot != objData[0])
					continue;
				// OK, need to fix it.
				if (bldr == null)
					bldr = tssFt.GetBldr();
				objData = ((char)(int)FwObjDataTypes.kodtNameGuidHot).ToString() + objData.Substring(1);
				bldr.SetStrPropValue(tssFt.get_MinOfRun(iRun), tssFt.get_LimOfRun(iRun), (int)FwTextPropType.ktptObjData, objData);
			}
			if (bldr == null)
				return tssFt; // no change
			else
				return bldr.GetString();
		}

		/// <summary>
		/// Update the paragraph's old-style translation...unless it's been deleted altogether.
		/// </summary>
		protected override void MonitorAction()
		{
			if (m_para.IsValidObject())
				UpdateMainTransFromSegmented(m_para, new int[] { m_ws });
		}

		protected override CmObject GetObjectToMonitor(int hvo)
		{
			return GetAnnotation(hvo);
		}

		/// <summary>
		/// It's safer to save the paragraph here...the segment might get deleted by one of the edits
		/// we're monitoring.
		/// </summary>
		/// <param name="newMonitorObject"></param>
		protected override void SetMonitorObject(CmObject newMonitorObject)
		{
			base.SetMonitorObject(newMonitorObject);
			if (m_objectToMonitor != null)
			{
				CmBaseAnnotation seg = (m_objectToMonitor as CmIndirectAnnotation).AppliesToRS[0] as CmBaseAnnotation;
				m_para = new ScrTxtPara(m_cache, seg.BeginObjectRAHvo);
			}
			else
			{
				m_para = null;
			}
		}

		protected override int PropertyOfInterest
		{
			get { return (int)CmAnnotation.CmAnnotationTags.kflidComment; }
		}
	}

	/// <summary>
	/// This class is a sort of 'opposite' of FreeTransEditMonitor. It monitors CmTranslation.Translation,
	/// and updates the segmented BT when the CmTranslation changes.
	/// </summary>
	public class CmTranslationEditMonitor  : EditMonitor
	{
		public CmTranslationEditMonitor(FdoCache cache, int transWs) : base(cache, transWs)
		{
		}

		protected override CmObject GetObjectToMonitor(int hvo)
		{
			if (m_cache.GetClassOfObject(hvo) != CmTranslation.kclsidCmTranslation)
				return null;
			// Return result unless it is disqualified somehow.
			CmTranslation result = CmObject.CreateFromDBObject(m_cache, hvo) as CmTranslation;
			StTxtPara para = result.Owner as StTxtPara;
			if (para == null)
				return null; // not a translation of Scripture...unlikely unless this is hooked up to some unexpected view.
			// Make sure the paragraph belongs to Scripture.
			if (!ScrTxtPara.IsScripturePara(para.Hvo, m_cache))
				return null; // not Scripture
			return result; // finally!
		}

		protected override void MonitorAction()
		{
			if (m_cache.GetIdFromGuid(LangProject.kguidAnnPunctuationInContext) == 0)
				return; // Testing, can't do conversion.
			BtConverter converter = new BtConverter(m_objectToMonitor.Owner as StTxtPara);
			converter.ConvertCmTransToInterlin(m_ws);
		}

		protected override int PropertyOfInterest
		{
			get { return (int)CmTranslation.CmTranslationTags.kflidTranslation; }
		}
	}
}
