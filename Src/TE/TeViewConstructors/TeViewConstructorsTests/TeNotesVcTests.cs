// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2007, SIL International. All Rights Reserved.
// <copyright from='2007' to='2007' company='SIL International'>
//		Copyright (c) 2007, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: TeNotesVcTests.cs
// Responsibility: TE Team
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NUnit.Framework;
using NMock;
using NMock.Constraints;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Scripture;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.RootSites;
using SIL.FieldWorks.FDO.FDOTests;
using SIL.FieldWorks.Test.TestUtils;
using SIL.FieldWorks.Common.FwUtils;

namespace SIL.FieldWorks.TE
{
	#region DummyTeNotesVc class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// DummyTeNotesVc class that provides access to the methods we want to test
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class DummyTeNotesVc: TeNotesVc
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// C'tor
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public DummyTeNotesVc(FdoCache cache) : base(cache)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Provides access to the "MakeLabelFromText" method.
		/// </summary>
		/// <param name="labelText">text to put in the label</param>
		/// <param name="styleName">style name to use for the character style on the text</param>
		/// <param name="ann">The annotation.</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public ITsString CallMakeLabelFromText(string labelText, string styleName,
			ScrScriptureNote ann)
		{
			CheckDisposed();

			return base.MakeLabelFromText(labelText, styleName, ann);
		}
	}
	#endregion

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Tests for TeNotesVc
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class TeNotesVcTests : ScrInMemoryFdoTestBase
	{
		#region Data members
		DummyTeNotesVc m_vc;
		IScrBook m_gen;
		#endregion

		#region Setup
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initialize the FDO cache and open database
		/// </summary>
		/// <remarks>This method is called before each test</remarks>
		/// ------------------------------------------------------------------------------------
		public override void Initialize()
		{
			base.Initialize();
			m_vc = new DummyTeNotesVc(Cache);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Called to make the test data for the tests
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void CreateTestData()
		{
			m_gen = m_scrInMemoryCache.AddBookWithTwoSections(1, "Genesis");
		}
		#endregion

		#region Tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the reference label is correctly generated for an annotation on a
		/// Scripture paragraph
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void MakeLabelFromText_AnnotationOnScripturePara()
		{
			CheckDisposed();

			ScrScriptureNote note = new ScrScriptureNote();
			m_scr.BookAnnotationsOS[0].NotesOS.Append(note);
			note.BeginRef = 01001011;
			note.EndRef = 01001011;
			note.BeginObjectRA = m_gen.SectionsOS[1].ContentOA.ParagraphsOS[0];
			ITsString tss = m_vc.CallMakeLabelFromText("GEN 1:11", "WhoCares", note);
			Assert.AreEqual(2, tss.RunCount);
			Assert.AreEqual("GEN 1:11", tss.get_RunText(0));
			Assert.AreEqual("WhoCares",
				tss.get_Properties(0).GetStrPropValue((int)FwTextPropType.ktptNamedStyle));
			CheckFinalSpaceInReferenceLabel(tss);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the reference label is correctly generated for an annotation on a
		/// footnote paragraph
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void MakeLabelFromText_AnnotationOnFootnote()
		{
			CheckDisposed();

			ScrScriptureNote note = new ScrScriptureNote();
			m_scr.BookAnnotationsOS[0].NotesOS.Append(note);
			note.BeginRef = 01001011;
			note.EndRef = 01001011;
			StFootnote footnote = m_scrInMemoryCache.AddFootnote(m_gen,
				(IStTxtPara)m_gen.SectionsOS[1].ContentOA.ParagraphsOS[0], 0);
			note.BeginObjectRA = m_scrInMemoryCache.AddParaToMockedText(footnote.Hvo, "Whatever");
			ITsString tss = m_vc.CallMakeLabelFromText("GEN 1:11", "WhoCares", note);
			Assert.AreEqual(3, tss.RunCount);
			Assert.AreEqual("\u0032", tss.get_RunText(0));
			Assert.AreEqual("GEN 1:11", tss.get_RunText(1));
			Assert.AreEqual("Marlett",
				tss.get_Properties(0).GetStrPropValue((int)FwTextPropType.ktptFontFamily));
			Assert.AreEqual("WhoCares",
				tss.get_Properties(1).GetStrPropValue((int)FwTextPropType.ktptNamedStyle));
			CheckFinalSpaceInReferenceLabel(tss);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the reference label is correctly generated for an annotation on a
		/// Back translation
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void MakeLabelFromText_AnnotationOnBackTrans()
		{
			CheckDisposed();

			ScrScriptureNote note = new ScrScriptureNote();
			m_scr.BookAnnotationsOS[0].NotesOS.Append(note);
			note.BeginRef = 01001011;
			note.EndRef = 01001011;
			StTxtPara para = (StTxtPara)m_gen.SectionsOS[1].ContentOA.ParagraphsOS[0];
			note.BeginObjectRA = para.GetOrCreateBT();
			ITsString tss = m_vc.CallMakeLabelFromText("GEN 1:11", "WhoCares", note);
			Assert.AreEqual(2, tss.RunCount);
			Assert.AreEqual("GEN 1:11", tss.get_RunText(0));
			Assert.AreEqual("WhoCares",
				tss.get_Properties(0).GetStrPropValue((int)FwTextPropType.ktptNamedStyle));
			CheckFinalSpaceInReferenceLabel(tss);
		}
		#endregion

		#region helper method
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Checks the final space in reference label.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void CheckFinalSpaceInReferenceLabel(ITsString tss)
		{
			int iRun = tss.RunCount - 1;
			Assert.AreEqual(" ", tss.get_RunText(iRun));
			ITsTextProps ttp = tss.get_Properties(iRun);
			Assert.AreEqual(null, ttp.GetStrPropValue((int)FwTextPropType.ktptNamedStyle));
			int var;
			Assert.AreEqual(Cache.DefaultUserWs,
				ttp.GetIntPropValues((int)FwTextPropType.ktptWs, out var));
		}
		#endregion
	}
}
