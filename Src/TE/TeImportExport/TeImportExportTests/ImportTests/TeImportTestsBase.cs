// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2007, SIL International. All Rights Reserved.
// <copyright from='2007' to='2007' company='SIL International'>
//		Copyright (c) 2007, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: TeImportTestsBase.cs
// Responsibility: TE Team
//
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.LangProj;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Scripture;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.Common.ScriptureUtils;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.ScrImportComponents;
using SIL.FieldWorks.Test.TestUtils;
using SIL.FieldWorks.Common.RootSites;
using SIL.FieldWorks.FDO.FDOTests;
using SILUBS.SharedScrUtils;

namespace SIL.FieldWorks.TE.ImportTests
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Base class for several import test classes
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TeImportTestsBase: TeTestBase
	{
		#region Member variables
		/// <summary></summary>
		protected DummyTeImporter m_importer;
		/// <summary></summary>
		protected int m_wsVern; // writing system info needed by tests
		/// <summary></summary>
		protected ITsTextProps m_ttpVernWS; // simple run text props expected by tests
		private static ITsStrBldr s_strBldr;
		/// <summary></summary>
		protected BCVRef m_titus;
		/// <summary></summary>
		protected FwStyleSheet m_styleSheet;
		/// <summary></summary>
		protected ScrImportSet m_settings;
		/// <summary></summary>
		protected int m_wsAnal;
		/// <summary></summary>
		protected ITsTextProps m_ttpAnalWS;
		#endregion

		#region Setup/Teardown
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize the importer
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();

			m_styleSheet = new FwStyleSheet();
			m_styleSheet.Init(Cache, m_scr.Hvo, (int)Scripture.ScriptureTags.kflidStyles);
			InitWsInfo();

			DummyTeImporter.s_translatorNoteDefn = m_scrInMemoryCache.m_translatorNoteDefn;
			DummyTeImporter.s_consultantNoteDefn = m_scrInMemoryCache.m_consultantNoteDefn;

			m_titus = new BCVRef(56001001);
			m_settings = new DummyScrImportSet();
			m_scr.ImportSettingsOC.Add(m_settings);
			m_settings.ImportTypeEnum = TypeOfImport.Other;
			m_settings.StartRef = m_titus;
			m_settings.EndRef = m_titus;
			m_settings.ImportTranslation = true;
			InitializeImportSettings();

			m_importer = new DummyTeImporter(m_settings, Cache, m_styleSheet, m_scrInMemoryCache);
			m_importer.Initialize();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes import settings (mappings and options) for "Other" type of import.
		/// Individual test fixtures can override for other types of import.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected virtual void InitializeImportSettings()
		{
			DummyTeImporter.MakeSFImportTestSettings(m_settings);
			m_settings.ImportBackTranslation = false;
			m_settings.ImportBookIntros = true;
			m_settings.ImportAnnotations = false;
		}

		#region IDisposable override
		/// ------------------------------------------------------------------------------------
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
		/// ------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if (disposing)
			{
				// Dispose managed resources here.
				if (m_importer != null)
					m_importer.Dispose();
			}

			// Dispose unmanaged resources here, whether disposing is true or false.
			m_styleSheet = null; // FwStyleSheet should implement IDisposable.
			m_settings = null;
			m_importer = null; // TeImporter should implement IDisposable.
			if (m_ttpVernWS != null)
			{
				//				Marshal.ReleaseComObject(m_ttpVernWS);
				m_ttpVernWS = null;
			}
			if (m_ttpAnalWS != null)
			{
				//				Marshal.ReleaseComObject(m_ttpAnalWS);
				m_ttpAnalWS = null;
			}

			base.Dispose(disposing);
		}
		#endregion IDisposable override

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TearDown]
		public override void Exit()
		{
			CheckDisposed();

			m_importer.Dispose();
			m_importer = null;
			m_styleSheet = null;
			m_settings = null;

			base.Exit();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void CreateTestData()
		{
			m_scrInMemoryCache.InitializeWritingSystemEncodings();
			// setup the default vernacular WS
			m_scrInMemoryCache.CacheAccessor.CacheVecProp(Cache.LangProject.Hvo,
				(int)LangProject.LangProjectTags.kflidCurVernWss,
				new int[] { InMemoryFdoCache.s_wsHvos.XKal }, 1);
			Cache.LangProject.CacheDefaultWritingSystems();
			m_scrInMemoryCache.InitializeAnnotationDefs();
			m_scrInMemoryCache.InitializeScrAnnotationCategories();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Init writing system info and some props needed by some tests.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected void InitWsInfo()
		{
			Debug.Assert(Cache != null);

			// get writing system info needed by tests
			m_wsVern = Cache.DefaultVernWs;
			m_wsAnal = Cache.DefaultAnalWs;

			// init simple run text props expected by tests
			ITsPropsBldr tsPropsBldr = TsPropsBldrClass.Create();
			tsPropsBldr.SetIntPropValues((int)FwTextPropType.ktptWs, 0, m_wsVern);
			m_ttpVernWS = tsPropsBldr.GetTextProps();
			tsPropsBldr.SetIntPropValues((int)FwTextPropType.ktptWs, 0, m_wsAnal);
			m_ttpAnalWS = tsPropsBldr.GetTextProps();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void InitializeCache()
		{
			base.InitializeCache();
			Cache.MapType(typeof(StTxtPara), typeof(ScrTxtPara));
			Cache.MapType(typeof(StFootnote), typeof(ScrFootnote));
		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// returns the default vernacular writing system
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected int DefaultVernWs
		{
			get { return m_wsVern; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// returns the default analysis writing system
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected int DefaultAnalWs
		{
			get { return m_wsAnal; }
		}
		#endregion

		#region Helper functions for tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the given run of m_importer.NormalParaStrBldr contains the specified text
		/// and properties. This method assumes vernacular WS.
		/// </summary>
		/// <param name="iRun">zero-based run index</param>
		/// <param name="text">Expected run text</param>
		/// <param name="charStyleName">character style name, or null if expecting default
		/// paragraph character props</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifyBldrRun(int iRun, string text, string charStyleName)
		{
			VerifyBldrRun(iRun, text, charStyleName, m_wsVern, m_importer.NormalParaStrBldr);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the given run of m_importer.NormalParaStrBldr contains the specified text
		/// and properties.
		/// </summary>
		/// <param name="iRun">zero-based run index</param>
		/// <param name="text">Expected run text</param>
		/// <param name="charStyleName">character style name, or null if expecting default
		/// paragraph character props</param>
		/// <param name="wsExpected">expected writing system for the run</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifyBldrRun(int iRun, string text, string charStyleName, int wsExpected)
		{
			VerifyBldrRun(iRun, text, charStyleName, wsExpected, m_importer.NormalParaStrBldr);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the given run of the given ITsStrBldr contains the specified text
		/// and properties.
		/// </summary>
		/// <param name="iRun">zero-based run index</param>
		/// <param name="text">Expected run text</param>
		/// <param name="charStyleName">character style name, or null if expecting default
		/// paragraph character props</param>
		/// <param name="wsExpected">expected writing system for the run</param>
		/// <param name="strBldr">the string builder in which to verify the run</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifyBldrRun(int iRun, string text, string charStyleName, int wsExpected,
			ITsStrBldr strBldr)
		{
			s_strBldr = strBldr;

			Assert.AreEqual(text, s_strBldr.get_RunText(iRun));
			ITsTextProps ttpExpected = CharStyleTextProps(charStyleName, wsExpected);
			ITsTextProps ttpRun = s_strBldr.get_Properties(iRun);
			string sWhy;
			if (!TsTextPropsHelper.PropsAreEqual(ttpExpected, ttpRun, out sWhy))
				Assert.Fail(sWhy);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the requested footnote.
		/// </summary>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// ------------------------------------------------------------------------------------
		protected IStFootnote GetFootnote(int iFootnoteIndex)
		{
			return m_importer.GetFootnote(iFootnoteIndex);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the processing of a footnote has resulted in the the ORC (Object
		/// Replacement Character) being inserted in the proper place in the dummy
		/// importer's NormalParaStrBldr.
		/// </summary>
		/// <param name="iRun">Zero-base index of the run that should contain the ORC</param>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifyBldrFootnoteOrcRun(int iRun, int iFootnoteIndex)
		{
			ITsTextProps orcProps =
				m_importer.NormalParaStrBldr.get_Properties(iRun);
			IStFootnote footnote = GetFootnote(iFootnoteIndex);
			Guid guid = Cache.GetGuidFromId(footnote.Hvo);
			string objData = orcProps.GetStrPropValue((int)FwTextPropType.ktptObjData);
			Assert.AreEqual((char)(int)FwObjDataTypes.kodtOwnNameGuidHot, objData[0]);
			// Send the objData string without the first character because the first character
			// is the object replacement character and the rest of the string is the GUID.
			Assert.AreEqual(guid, MiscUtils.GetGuidFromObjData(objData.Substring(1)));
			string sOrc = m_importer.NormalParaStrBldr.get_RunText(iRun);
			Assert.AreEqual(StringUtils.kchObject, sOrc[0]);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote has been created in the DB with the given number of runs,
		/// the first of which has the specified text, using the default footnote properties.
		/// </summary>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// <param name="sFirstFootnoteSegment">Expected footnote contents</param>
		/// <param name="runCount">Number of runs expected in the footnote para</param>
		/// ------------------------------------------------------------------------------------
		protected ITsString VerifyComplexFootnote(int iFootnoteIndex, string sFirstFootnoteSegment,
			int runCount)
		{
			return VerifySimpleFootnote(iFootnoteIndex, sFirstFootnoteSegment, iFootnoteIndex, "a",
				ScrStyleNames.NormalFootnoteParagraph, runCount);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote has been created in the DB with a single default run having
		/// the specified text, using the default footnote properties.
		/// </summary>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// <param name="sFootnoteSegment">Expected footnote contents</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifySimpleFootnote(int iFootnoteIndex, string sFootnoteSegment)
		{
			VerifySimpleFootnote(iFootnoteIndex, sFootnoteSegment, "a");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote has been created in the DB with a single default run having
		/// the specified text.
		/// </summary>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// <param name="sFootnoteSegment">Expected footnote contents</param>
		/// <param name="sMarker">One of: <c>"a"</c>, for automatic alpha sequence; <c>"*"</c>,
		/// for a literal marker (we always use "*" in these tests); or <c>string.Empty</c>,
		/// for no marker</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifySimpleFootnote(int iFootnoteIndex, string sFootnoteSegment,
			string sMarker)
		{
			VerifySimpleFootnote(iFootnoteIndex, sFootnoteSegment, iFootnoteIndex, sMarker,
				ScrStyleNames.NormalFootnoteParagraph, 1);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote has been created in the DB with a single default run having
		/// the specified text.
		/// </summary>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// <param name="sFootnoteSegment">Expected footnote contents</param>
		/// <param name="iAutoNumberedFootnoteIndex">zero-based index of this footnote
		/// in list of all auto-numbered footnotes. This could the be the third footnote
		/// overall, but only the second auto-numbered footnote.</param>
		/// <param name="sMarker">One of: <c>"a"</c>, for automatic alpha sequence; <c>"*"</c>,
		/// for a literal marker (we always use "*" in these tests); or <c>string.Empty</c>,
		/// for no marker</param>
		/// <param name="sParaStyleName">Name of the paragraph style</param>
		/// <param name="runCount">Number of runs expected in the footnote para</param>
		/// ------------------------------------------------------------------------------------
		protected ITsString VerifySimpleFootnote(int iFootnoteIndex, string sFootnoteSegment,
			int iAutoNumberedFootnoteIndex, string sMarker, string sParaStyleName, int runCount)
		{
			return m_importer.VerifySimpleFootnote(iFootnoteIndex, sFootnoteSegment, iAutoNumberedFootnoteIndex,
				sMarker, sParaStyleName, runCount);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote marker ORC (Object Replacement Character) has been inserted
		/// as the iRunth run in the given TsString (vernacular).
		/// </summary>
		/// <param name="tssPara">TsString to check</param>
		/// <param name="iRun">Zero-based index of run to check for the ORC</param>
		/// ------------------------------------------------------------------------------------
		protected void VerifyFootnoteMarkerOrcRun(ITsString tssPara, int iRun)
		{
			VerifyFootnoteMarkerOrcRun(tssPara, iRun, DefaultVernWs, false);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote marker ORC (Object Replacement Character) has been inserted
		/// as the iRunth run in the given TsString.
		/// </summary>
		/// <param name="tssPara">TsString to check</param>
		/// <param name="iRun">Zero-based index of run to check for the ORC</param>
		/// <param name="ws">Writing system that should be used for the ORC</param>
		/// <param name="fBT">Indicates whether this is checking a back translation.</param>
		/// ------------------------------------------------------------------------------------
		public static void VerifyFootnoteMarkerOrcRun(ITsString tssPara, int iRun, int ws,
			bool fBT)
		{
			Debug.Assert(tssPara.RunCount > iRun, "Trying to access run #" + iRun +
				" when there are only " + tssPara.RunCount + " run(s).");
			string sOrcRun = tssPara.get_RunText(iRun);
			Assert.AreEqual(1, sOrcRun.Length);
			Assert.AreEqual(StringUtils.kchObject, sOrcRun[0]);
			ITsTextProps ttpOrcRun = tssPara.get_Properties(iRun);
			int nDummy;
			int wsActual = ttpOrcRun.GetIntPropValues((int)FwTextPropType.ktptWs,
				out nDummy);
			Assert.AreEqual(ws, wsActual, "Wrong writing system for footnote marker in text");
			string objData = ttpOrcRun.GetStrPropValue((int)FwTextPropType.ktptObjData);
			FwObjDataTypes orcType = (fBT) ? FwObjDataTypes.kodtNameGuidHot :
				FwObjDataTypes.kodtOwnNameGuidHot;
			Assert.AreEqual((char)(int)orcType, objData[0]);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a footnote has been created in the DB with a single default run having
		/// the specified text and the given translation.
		/// </summary>
		/// <param name="iFootnoteIndex">zero-based footnote index</param>
		/// <param name="sFootnoteSegment">Expected footnote contents</param>
		/// <param name="sFootnoteTransSegment">Expected footnote translation contents</param>
		/// <param name="sMarker">One of: <c>"a"</c>, for automatic alpha sequence; <c>"*"</c>,
		/// for a literal marker (we always use "*" in these tests); or <c>string.Empty</c>,
		/// for no marker</param>
		/// <param name="sParaStyleName">Name of the paragraph style</param>
		/// ------------------------------------------------------------------------------------
		public void VerifyFootnoteWithTranslation(int iFootnoteIndex, string sFootnoteSegment,
			string sFootnoteTransSegment, string sMarker, string sParaStyleName)
		{
			// Force reload of book
			IStFootnote footnote = m_importer.GetFootnote(iFootnoteIndex);
			if (sMarker == "a")
				sMarker = new string((char)((int)'a' + (iFootnoteIndex % 26)), 1);
			if (sMarker != null)
			{
				AssertEx.RunIsCorrect(footnote.FootnoteMarker.UnderlyingTsString, 0,
					sMarker, ScrStyleNames.FootnoteMarker, m_wsVern);
			}
			else
			{
				Assert.IsNull(footnote.FootnoteMarker.Text);
			}
			FdoOwningSequence<IStPara> footnoteParas = footnote.ParagraphsOS;
			Assert.AreEqual(1, footnoteParas.Count);
			StTxtPara para = (StTxtPara)footnoteParas[0];
			Assert.AreEqual(StyleUtils.ParaStyleTextProps(sParaStyleName), para.StyleRules);
			ITsString tss = para.Contents.UnderlyingTsString;
			Assert.AreEqual(1, tss.RunCount);
			AssertEx.RunIsCorrect(tss, 0, sFootnoteSegment, null, m_wsVern);
			// Check Translation
			if (sFootnoteTransSegment != null)
			{
				Assert.AreEqual(1, para.TranslationsOC.Count);
				ICmTranslation trans = para.GetBT();
				tss = trans.Translation.AnalysisDefaultWritingSystem.UnderlyingTsString;
				Assert.AreEqual(1, tss.RunCount);
				AssertEx.RunIsCorrect(tss, 0, sFootnoteTransSegment, null, m_wsAnal);
			}
			else
				Assert.AreEqual(0, para.TranslationsOC.Count);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a picture has been created in the DB with a single default run having
		/// the specified text.
		/// </summary>
		/// <param name="para">paragraph that owns the picture</param>
		/// <param name="iPictureIndex">zero-based picture index</param>
		/// <param name="sPictureCaption">Expected picture caption</param>
		/// <param name="sPictureTransCaption">Expected picture caption translation</param>
		/// ------------------------------------------------------------------------------------
		public void VerifyPictureWithTranslation(IStTxtPara para, int iPictureIndex,
			string sPictureCaption, string sPictureTransCaption)
		{
			List<ICmPicture> pictures = para.GetPictures();
			ICmPicture picture = pictures[iPictureIndex];
			Assert.AreEqual(sPictureCaption, picture.Caption.VernacularDefaultWritingSystem.Text);
			Assert.AreEqual(sPictureTransCaption, picture.Caption.AnalysisDefaultWritingSystem.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that a section has been properly added to the given ScrBook, including the
		/// section's heading and contents objects.
		/// </summary>
		/// <param name="book">the ScrBook object to be tested</param>
		/// <param name="iSection">index of the new section (must be the last section in book)
		/// </param>
		/// ------------------------------------------------------------------------------------
		protected void VerifyNewSectionExists(IScrBook book, int iSection)
		{
			Assert.AreEqual(iSection + 1, book.SectionsOS.Count);
			if (iSection < 0)
				return;

			Assert.IsTrue(book.SectionsOS[iSection].HeadingOA.IsValidObject());
			Assert.AreEqual(book.SectionsOS[iSection].HeadingOA.Hvo,
				m_importer.HvoSectionHeading); //empty section heading so far
			Assert.IsTrue(book.SectionsOS[iSection].ContentOA.IsValidObject());
			Assert.AreEqual(book.SectionsOS[iSection].ContentOAHvo,
				m_importer.HvoSectionContent); //empty section contents
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns run textprops representing a character style of the given name using the
		/// given writing system.
		/// </summary>
		/// <param name="styleName">The character style name for which to create props</param>
		/// <param name="ws">The writing system to use for the props</param>
		/// <returns>requested text props</returns>
		/// <remarks>If styleName is not given, the resulting props contains only the
		/// given writing system.</remarks>
		/// <remarks>For char style, props should contain ws (writing system) and char style
		/// name. Without a char style, props should contain ws only.</remarks>
		/// ------------------------------------------------------------------------------------
		protected ITsTextProps CharStyleTextProps(string styleName, int ws)
		{
			// If no style name, return simple ws props
			if (styleName == null || styleName.Length == 0)
			{
				if (ws == m_wsVern)
				{
					// for minor performance gain, use the vern ws props we've already built
					return m_ttpVernWS;
				}
			}

			// Return props for the given char style name
			return StyleUtils.CharStyleTextProps(styleName, ws);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns run textprops representing a character style of the given name using the
		/// vernacular writing system.
		/// </summary>
		/// <param name="styleName">The character style name for which to create props</param>
		/// <returns>requested text props</returns>
		/// <remarks>If styleName is not given, the resulting props contains only the
		/// vernacular writing system.</remarks>
		/// <remarks>For char style, props should contain ws (writing system) and char style
		/// name. Without a char style, props should contain ws only.</remarks>
		/// ------------------------------------------------------------------------------------
		protected ITsTextProps CharStyleTextProps(string styleName)
		{
			return CharStyleTextProps(styleName, m_wsVern);
		}
		#endregion
	}
}
