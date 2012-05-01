// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005, SIL International. All Rights Reserved.
// <copyright from='2005' to='2005' company='SIL International'>
//		Copyright (c) 2005, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: TeKeyTermsInitTests.cs
// Responsibility: TE Team
// ---------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Xml;

using NUnit.Framework;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.LangProj;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.Controls;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.FDO.FDOTests;
using SIL.FieldWorks.Test.TestUtils;
using SIL.FieldWorks.Common.ScriptureUtils;
using System.Collections.Generic;

namespace SIL.FieldWorks.TE
{
	#region DummyTeKeyTermsInit class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class DummyTeKeyTermsInit: TeKeyTermsInit
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="DummyTeKeyTermsInit"/> class.
		/// </summary>
		/// <param name="scr">The Scripture object.</param>
		/// ------------------------------------------------------------------------------------
		public DummyTeKeyTermsInit(IScripture scr) : base(scr)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Call the base class method
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void CallLoadKeyTerms(ICmPossibilityList keyTermsList,
			BiblicalTermsList terms, List<BiblicalTermsLocalization> localizations)
		{
			IScripture scr = keyTermsList.Cache.LangProject.TranslatedScriptureOA;
			new DummyTeKeyTermsInit(scr).LoadKeyTerms(null, null, keyTermsList, terms, localizations);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Call the base class method
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static void CallLoadKeyTerms(ICmPossibilityList oldKeyTermsList,
			ICmPossibilityList newKeyTermsList, BiblicalTermsList terms,
			List<BiblicalTermsLocalization> localizations)
		{
			IScripture scr = newKeyTermsList.Cache.LangProject.TranslatedScriptureOA;
			new DummyTeKeyTermsInit(scr).LoadKeyTerms(null, oldKeyTermsList, newKeyTermsList,
				terms, localizations);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Calls the deserialize biblical terms file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>A BiblicalTermsList object</returns>
		/// ------------------------------------------------------------------------------------
		internal static BiblicalTermsList CallDeserializeBiblicalTermsFile(string filename)
		{
			return TeKeyTermsInit.DeserializeBiblicalTermsFile(filename);
		}
	}
	#endregion

	#region TeKeyTermsInitTestBase class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TeKeyTermsInitTestBase is a base class for tests related to the
	/// <see cref="TeKeyTermsInit"/> class and it's supporting classes.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TeKeyTermsInitTestBase : BaseTest
	{
		#region Data members
		protected FdoCache m_fdoCache;
		protected ILgWritingSystemFactory m_wsf;
		protected int m_wsEn; // English Writing System
		private TsStrFactory m_factory;
		#endregion

		#region Setup, Teardown
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Connect to TestLangProj and start an undo task;
		/// init the base class we are testing
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TestFixtureSetUp]
		public override void FixtureSetup()
		{
			CheckDisposed();
			base.FixtureSetup();

			Debug.Assert(m_fdoCache == null, "m_fdoCache is not null");
			//if (m_fdoCache != null)
			//	m_fdoCache.DisposeWithWSFactoryShutdown();
			m_fdoCache = InDatabaseFdoTestBase.SetupCache();
			ScrReferenceTests.InitializeScrReferenceForTests();

			m_wsf = m_fdoCache.LanguageWritingSystemFactoryAccessor;
			m_wsf.BypassInstall = true;
			m_wsEn = m_wsf.GetWsFromStr("en");

			m_factory = TsStrFactoryClass.Create();
		}

		#region IDisposable override

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
		protected override void Dispose(bool disposing)
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if (disposing)
			{
				// Dispose managed resources here.
				if (m_fdoCache != null)
					m_fdoCache.DisposeWithWSFactoryShutdown();
				FdoCache.RestoreTestLangProj();
			}

			// Dispose unmanaged resources here, whether disposing is true or false.
			m_fdoCache = null;

			base.Dispose(disposing);
		}
		#endregion IDisposable override
		#endregion

		#region Helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds an occurrence to and old-style sense.
		/// </summary>
		/// <param name="term">The ChkTerm (a sense or subsense).</param>
		/// <param name="reference">The Scripture reference.</param>
		/// <param name="wordform">The wordform.</param>
		/// <param name="keyword">The keyword (transliterated from Greek)</param>
		/// <returns>The newly added ChkRef</returns>
		/// ------------------------------------------------------------------------------------
		protected ChkRef AddOccurrenceToOldStyleSense(ChkTerm term, int reference,
			IWfiWordform wordform, string keyword)
		{
			return AddOccurrenceToOldStyleSense(term, reference, wordform, keyword,
				(wordform == null) ? KeyTermRenderingStatus.Unassigned : KeyTermRenderingStatus.Assigned);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds an occurrence to and old-style sense.
		/// </summary>
		/// <param name="term">The ChkTerm (a sense or subsense).</param>
		/// <param name="reference">The Scripture reference.</param>
		/// <param name="wordform">The wordform.</param>
		/// <param name="keyword">The keyword (transliterated from Greek)</param>
		/// <param name="status">The rendering status.</param>
		/// <returns>The newly added ChkRef</returns>
		/// ------------------------------------------------------------------------------------
		protected ChkRef AddOccurrenceToOldStyleSense(ChkTerm term, int reference,
			IWfiWordform wordform, string keyword, KeyTermRenderingStatus status)
		{
			ChkRef occurrence = new ChkRef();
			term.OccurrencesOS.Append(occurrence);
			occurrence.KeyWord.UnderlyingTsString = m_factory.MakeString(keyword, m_wsEn);
			occurrence.Ref = reference;
			// If wordform is null, status must either be unassigned or ignored
			// If wordform is not null, status must either be assigned or auto-assigned
			Debug.Assert(((wordform == null) && (status == KeyTermRenderingStatus.Unassigned ||
				status == KeyTermRenderingStatus.Ignored)) ||
				((wordform != null) && (status == KeyTermRenderingStatus.Assigned ||
				status == KeyTermRenderingStatus.AutoAssigned)));
			occurrence.Status = status;
			occurrence.RenderingRA = wordform;

			return occurrence;
		}
		#endregion
	}
	#endregion

	#region TeKeyTermsInitTests class
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TeKeyTermsInitTests is a collection of tests for static methods of the
	/// <see cref="TeKeyTermsInit"/> class
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class TeKeyTermsInitTests : TeKeyTermsInitTestBase
	{
		#region Tests for deserializing BiblicalTerms.xml
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Reads the valid biblical terms XML file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void ReadValidBiblicalTermsXmlFile()
		{
			BiblicalTermsList list = DummyTeKeyTermsInit.CallDeserializeBiblicalTermsFile(
				DirectoryFinder.FWCodeDirectory + @"\BiblicalTerms.xml");
			Assert.AreEqual(9600 - 5001 + 1829, list.KeyTerms.Count, "Wrong number of terms read. (Note: Terms 1829-5000 don't exist.)");
			for (int i = 0; i < list.KeyTerms.Count; i++)
				Assert.AreEqual(i + ((i < 1828) ? 1 : 3173), list.KeyTerms[i].Id);

			Term term = list.KeyTerms[0];
			Assert.AreEqual("PN", term.Category);
			Assert.AreEqual("Aaron", term.Gloss);
			Assert.AreEqual("Greek", term.Language);
			Assert.AreEqual("\u1F08\u03B1\u03C1\u1F7D\u03BD", term.Lemma);
			Assert.AreEqual("ID_1", term.OrigID);
			Assert.AreEqual("Aarwn", term.Transliteration);
			List<long> refs = term.References;
			Assert.AreEqual(5, refs.Count);
			Assert.AreEqual(4200100522, refs[0]);
			Assert.AreEqual(4400704003, refs[1]);
			Assert.AreEqual(5800500415, refs[2]);
			Assert.AreEqual(5800701131, refs[3]);
			Assert.AreEqual(5800900422, refs[4]);

			term = list.KeyTerms[1741];
			Assert.AreEqual("KT", term.Category);
			Assert.AreEqual("hospitably", term.Gloss);
			Assert.AreEqual("Greek", term.Language);
			Assert.AreEqual("\u03C6\u03B9\u03BB\u1F73\u03C9-15", term.Lemma);
			Assert.AreEqual("ID_1194", term.OrigID);
			Assert.AreEqual("philew-15", term.Transliteration);
			Assert.AreEqual("affection", term.Domain);
			Assert.AreEqual("\u03C6\u03B9\u03BB\u03BF\u03C6\u03C1\u1F79\u03BD\u03C9\u03C2", term.Form);
			Assert.AreEqual("\u03C6\u03B9\u03BB\u1F71\u03B3\u03B1\u03B8\u03BF\u03C2, \u03C6\u03B9\u03BB\u03B1\u03B4\u03B5\u03BB\u03C6\u1F77\u03B1, \u03C6\u03B9\u03BB\u1F71\u03B4\u03B5\u03BB\u03C6\u03BF\u03C2, \u03C6\u1F77\u03BB\u03B1\u03BD\u03B4\u03C1\u03BF\u03C2, \u03C6\u03B9\u03BB\u03B1\u03BD\u03B8\u03C1\u03C9\u03C0\u1F77\u03B1, \u03C6\u03B9\u03BB\u03B1\u03C1\u03B3\u03C5\u03C1\u1F77\u03B1, \u03C6\u03B9\u03BB\u1F71\u03C1\u03B3\u03C5\u03C1\u03BF\u03C2, \u03C6\u1F77\u03BB\u03B1\u03C5\u03C4\u03BF\u03C2, \u03C6\u03B9\u03BB\u1F75\u03B4\u03BF\u03BD\u03BF\u03C2, \u03C6\u03B9\u03BB\u1F77\u03B1, \u03C6\u03B9\u03BB\u1F79\u03B8\u03B5\u03BF\u03C2, \u03C6\u1F77\u03BB\u03BF\u03C2, \u03C6\u03B9\u03BB\u1F79\u03C3\u03C4\u03BF\u03C1\u03B3\u03BF\u03C2, \u03C6\u03B9\u03BB\u03BF\u03C6\u03C1\u1F79\u03BD\u03C9\u03C2", term.Including);
			refs = term.References;
			Assert.AreEqual(1, refs.Count);
			Assert.AreEqual(4402800721, refs[0]);

			term = list.KeyTerms[list.KeyTerms.Count - 1];
			Assert.AreEqual("PN", term.Category);
			Assert.AreEqual("Tishbite", term.Gloss);
			Assert.AreEqual("Hebrew", term.Language);
			Assert.AreEqual("\u05EA\u05BC\u05B4\u05E9\u05C1\u05B0\u05D1\u05BC\u05B4\u05D9", term.Lemma);
			Assert.AreEqual("ID_3098", term.OrigID);
			Assert.AreEqual("tatt\u0259nay", term.Transliteration);
			refs = term.References;
			Assert.AreEqual(6, refs.Count);
			Assert.AreEqual(1101700101, refs[0]);
			Assert.AreEqual(1102101701, refs[1]);
			Assert.AreEqual(1102102801, refs[2]);
			Assert.AreEqual(1200100301, refs[3]);
			Assert.AreEqual(1200100801, refs[4]);
			Assert.AreEqual(1200903601, refs[5]);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Expect an exception attempting to read a missing biblical terms XML file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
#if DEBUG
		[ExpectedException(typeof(Exception),
			ExpectedMessage = "Error reading globomunkey12.xml: file is missing or has invalid XML syntax.")]
#else
		[ExpectedException(typeof(Exception))]
#endif
		public void ReadMissingBiblicalTermsXmlFile()
		{
			DummyTeKeyTermsInit.CallDeserializeBiblicalTermsFile("globomunkey12.xml");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Expect an exception attempting to read a bogus biblical terms XML file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		[ExpectedException(typeof(Exception))]
		public void ReadBogusBiblicalTermsXmlFile()
		{
			DummyTeKeyTermsInit.CallDeserializeBiblicalTermsFile(
				DirectoryFinder.FWCodeDirectory + @"\BiblicalTermsEn.xml");
		}
		#endregion

		#region Test of GetWsFromLocFile method
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Tests that the GetWsFromLocFile method correctly retireves the HVO of a writing
		/// system from a BiblicalTerms localization file
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TestGetWsFromLocFile()
		{
			Assert.AreEqual(m_wsEn,
				TeKeyTermsInit.GetWsFromLocFile(m_fdoCache.LanguageWritingSystemFactoryAccessor,
				"c:\\fw\\distfiles\\BiblicalTerms-en.xml"));
			Assert.AreEqual(0,
				TeKeyTermsInit.GetWsFromLocFile(m_fdoCache.LanguageWritingSystemFactoryAccessor,
				"c:\\fw\\distfiles\\BiblicalTerms-q2z.xml"));
		}
		#endregion

		#region Tests for loading a new list from scratch
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test creating a simple key term
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SimpleKeyTermInit()
		{
			CheckDisposed();

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			terms.KeyTerms.Add(new Term(3, "KT", "\u03b1\u03b2\u03b2\u03b1", "Greek",
				"abba; father", null, null,
				4101403603, 4500801516, 4800400618));

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(1);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(1);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("KT", "Key terms"));
			loc.Terms.Add(new TermLocalization(3, "abba; father", "title for God, literally dad"));
			localizations.Add(loc);

			ILangProject lp = m_fdoCache.LangProject;
			ICmPossibilityList chkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(chkTermsList);

			// Load the term
			DummyTeKeyTermsInit.CallLoadKeyTerms(chkTermsList, terms, localizations);

			int wsGreek = m_wsf.GetWsFromStr("grc");

			// Make sure there is one category (possibility)
			Assert.AreEqual(1, chkTermsList.PossibilitiesOS.Count);
			ICmPossibility ktCategory = chkTermsList.PossibilitiesOS[0];
			Assert.AreEqual("KT", ktCategory.Abbreviation.GetAlternative(m_wsEn));

			// Make sure there is one keyterm in that category
			Assert.AreEqual(1, ktCategory.SubPossibilitiesOS.Count);

			// Look at the contents of the ChkTerm
			IChkTerm keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(3, keyterm.TermId);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("abba", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("title for God, literally dad",
				keyterm.Description.GetAlternative(m_wsEn).Text);
			Assert.AreEqual("father", keyterm.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);

			// There should be 3 references for this key term
			Assert.AreEqual(3, keyterm.OccurrencesOS.Count);

			IChkRef reference = keyterm.OccurrencesOS[0];
			Assert.AreEqual(41014036, reference.Ref);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", reference.KeyWord.Text);
			Assert.AreEqual(3, reference.Location);

			reference = keyterm.OccurrencesOS[1];
			Assert.AreEqual(45008015, reference.Ref);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", reference.KeyWord.Text);
			Assert.AreEqual(16, reference.Location);

			reference = keyterm.OccurrencesOS[2];
			Assert.AreEqual(48004006, reference.Ref);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", reference.KeyWord.Text);
			Assert.AreEqual(18, reference.Location);

			bool fFoundResource = false;
			foreach (CmResource resource in lp.TranslatedScriptureOA.ResourcesOC)
			{
				if (resource.Name == "BiblicalTerms")
				{
					Assert.AreEqual(terms.Version, resource.Version);
					fFoundResource = true;
				}
			}
			Assert.IsTrue(fFoundResource);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test creating key term list with items having sense numbers
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void StripOffSenseNumbers()
		{
			CheckDisposed();

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			terms.KeyTerms.Add(new Term(13, "KT", "\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2-1", "Greek",
				"good; goodness; good act",
				"\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2, \u1F00\u03B3\u03B1\u03B8\u03C9\u03C3\u1F7B\u03BD\u03B7",
				"\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F73\u03C9, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u03B9\u03B1, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F79\u03C2, \u1F00\u03B3\u03B1\u03B8\u03C9\u03C3\u1F7B\u03BD\u03B7, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C5\u03C1\u03B3\u03B5\u03C9",
				04000504518, 04001203405, 04001203502));
			terms.KeyTerms.Add(new Term(14, "KT", "\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2-2", "Greek",
				"do good; perform good deeds; good works",
				"\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u03B9\u03B1, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F73\u03C9, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C5\u03C1\u03B3\u03B5\u03C9",
				"\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F73\u03C9, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u03B9\u03B1, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F79\u03C2, \u1F00\u03B3\u03B1\u03B8\u03C9\u03C3\u1F7B\u03BD\u03B7, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C5\u03C1\u03B3\u03B5\u03C9",
				04200600913, 04200603304));
			terms.KeyTerms.Add(new Term(15, "KT", "\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2-3", "Greek",
				"one who does good; one who benefits others",
				"\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F79\u03C2",
				"\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F73\u03C9, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u03B9\u03B1, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F79\u03C2, \u1F00\u03B3\u03B1\u03B8\u03C9\u03C3\u1F7B\u03BD\u03B7, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C5\u03C1\u03B3\u03B5\u03C9",
				06000201412));
			terms.KeyTerms.Add(new Term(98, "PN", "\u1F08\u03BB\u1F73\u03BE\u03B1\u03BD\u03B4\u03C1\u03BF\u03C2-1",
				"Greek", "Alexander", null, null, 04101502112));

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(1);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(3);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("KT", "Key terms"));
			loc.Terms.Add(new TermLocalization(13, "good; goodness; good act", "positive moral qualities of the most general nature"));
			loc.Terms.Add(new TermLocalization(14, "do good; perform good deeds; good works", "to engage in doing what is good"));
			loc.Terms.Add(new TermLocalization(15, "one who does good; one who benefits others", "one who customarily does good"));
			localizations.Add(loc);

			ILangProject lp = m_fdoCache.LangProject;
			ICmPossibilityList chkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(chkTermsList);

			// Load the term
			DummyTeKeyTermsInit.CallLoadKeyTerms(chkTermsList, terms, localizations);

			int wsGreek = m_wsf.GetWsFromStr("grc");

			// Make sure there are two categories (possibilities)
			Assert.AreEqual(2, chkTermsList.PossibilitiesOS.Count);
			ICmPossibility ktCategory = null, pnCategory = null;
			foreach (ICmPossibility cat in chkTermsList.PossibilitiesOS)
			{
				string catNameEn = cat.Abbreviation.GetAlternative(m_wsEn);
				switch (catNameEn)
				{
					case "KT": ktCategory = cat; break;
					case "PN": pnCategory = cat; break;
					default: Assert.Fail("Unexpected category in the list: " + catNameEn); break;
				}
			}

			// Make sure there are three key terms in the KT category
			Assert.AreEqual(3, ktCategory.SubPossibilitiesOS.Count);

			// Look at the contents of each ChkTerm
			IChkTerm keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(13, keyterm.TermId);
			Assert.AreEqual("\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("good", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("positive moral qualities of the most general nature",
				keyterm.Description.GetAlternative(m_wsEn).Text);
			Assert.AreEqual("goodness; good act", keyterm.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);
			Assert.AreEqual(3, keyterm.OccurrencesOS.Count);
			Assert.AreEqual("\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2, \u1F00\u03B3\u03B1\u03B8\u03C9\u03C3\u1F7B\u03BD\u03B7",
				keyterm.OccurrencesOS[0].KeyWord.Text);

			keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[1];
			Assert.AreEqual(14, keyterm.TermId);
			Assert.AreEqual("\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("do good", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual(2, keyterm.OccurrencesOS.Count);
			Assert.AreEqual("\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u03B9\u03B1, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F73\u03C9, \u1F00\u03B3\u03B1\u03B8\u03BF\u03C5\u03C1\u03B3\u03B5\u03C9",
				keyterm.OccurrencesOS[0].KeyWord.Text);

			keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[2];
			Assert.AreEqual(15, keyterm.TermId);
			Assert.AreEqual("\u1F00\u03B3\u03B1\u03B8\u03BF\u03C2", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("one who does good", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual(1, keyterm.OccurrencesOS.Count);
			Assert.AreEqual("\u1F00\u03B3\u03B1\u03B8\u03BF\u03C0\u03BF\u03B9\u1F79\u03C2",
				keyterm.OccurrencesOS[0].KeyWord.Text);

			// Make sure there is one term in the PN category
			Assert.AreEqual(1, pnCategory.SubPossibilitiesOS.Count);

			// Check the contents and occurrences of the ChkTerm
			IChkTerm pnTerm = (IChkTerm)pnCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(98, pnTerm.TermId);
			Assert.AreEqual("\u1F08\u03BB\u1F73\u03BE\u03B1\u03BD\u03B4\u03C1\u03BF\u03C2", pnTerm.Name.GetAlternative(wsGreek));
			Assert.AreEqual(1, pnTerm.OccurrencesOS.Count);
			Assert.AreEqual("\u1F08\u03BB\u1F73\u03BE\u03B1\u03BD\u03B4\u03C1\u03BF\u03C2",
				pnTerm.OccurrencesOS[0].KeyWord.Text);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test creating a key term when the description is null
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void KeyTermInit_NullDescription()
		{
			CheckDisposed();

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			terms.KeyTerms.Add(new Term(56, "FA", "\u1F00\u03B5\u03C4\u1F79\u03C2", "Greek",
				"eagle; vulture", null, null,
				4002402809, 4201703717, 6600400728, 6600801306, 6601201409));

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(1);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(1);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("FA", "Fauna"));
			loc.Terms.Add(new TermLocalization(56, "eagle; vulture", null));
			localizations.Add(loc);

			ILangProject lp = m_fdoCache.LangProject;
			ICmPossibilityList chkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(chkTermsList);

			// Load the term
			DummyTeKeyTermsInit.CallLoadKeyTerms(chkTermsList, terms, localizations);

			int wsGreek = m_wsf.GetWsFromStr("grc");

			// Make sure there is one category (possibility)
			Assert.AreEqual(1, chkTermsList.PossibilitiesOS.Count);
			ICmPossibility faCategory = chkTermsList.PossibilitiesOS[0];
			Assert.AreEqual("FA", faCategory.Abbreviation.GetAlternative(m_wsEn));

			// Make sure there is one entry in the Fauna category
			Assert.AreEqual(1, faCategory.SubPossibilitiesOS.Count);

			// Look at the contents of the ChkTerm
			IChkTerm keyterm = (IChkTerm)faCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(56, keyterm.TermId);
			Assert.AreEqual("\u1F00\u03B5\u03C4\u1F79\u03C2", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("eagle", keyterm.Name.GetAlternative(m_wsEn));
			Assert.IsNull(keyterm.Description.GetAlternative(m_wsEn).Text);
			Assert.AreEqual("vulture", keyterm.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);

			// There should be 5 references for this key term
			Assert.AreEqual(5, keyterm.OccurrencesOS.Count);

			IChkRef reference = keyterm.OccurrencesOS[0];
			Assert.AreEqual(40024028, reference.Ref);
			Assert.AreEqual("\u1F00\u03B5\u03C4\u1F79\u03C2", reference.KeyWord.Text);
			Assert.AreEqual(9, reference.Location);

			reference = keyterm.OccurrencesOS[1];
			Assert.AreEqual(42017037, reference.Ref);
			Assert.AreEqual("\u1F00\u03B5\u03C4\u1F79\u03C2", reference.KeyWord.Text);
			Assert.AreEqual(17, reference.Location);

			reference = keyterm.OccurrencesOS[2];
			Assert.AreEqual(66004007, reference.Ref);
			Assert.AreEqual("\u1F00\u03B5\u03C4\u1F79\u03C2", reference.KeyWord.Text);
			Assert.AreEqual(28, reference.Location);

			reference = keyterm.OccurrencesOS[3];
			Assert.AreEqual(66008013, reference.Ref);
			Assert.AreEqual("\u1F00\u03B5\u03C4\u1F79\u03C2", reference.KeyWord.Text);
			Assert.AreEqual(6, reference.Location);

			reference = keyterm.OccurrencesOS[4];
			Assert.AreEqual(66012014, reference.Ref);
			Assert.AreEqual("\u1F00\u03B5\u03C4\u1F79\u03C2", reference.KeyWord.Text);
			Assert.AreEqual(9, reference.Location);

			bool fFoundResource = false;
			foreach (CmResource resource in lp.TranslatedScriptureOA.ResourcesOC)
			{
				if (resource.Name == "BiblicalTerms")
				{
					Assert.AreEqual(terms.Version, resource.Version);
					fFoundResource = true;
				}
			}
			Assert.IsTrue(fFoundResource);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test reading key terms in different categories
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TwoCategoriesChkTermInit()
		{
			CheckDisposed();

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			// Using asterisks to ensure that localizations are coming from localizations, not from
			// base key terms list.
			terms.KeyTerms.Add(new Term(1, "PN", "\u1F08\u03B1\u03C1\u1F7D\u03BD", "Greek",
				"Aaron***", null, null, 4200100522, 4400704003));
			terms.KeyTerms.Add(new Term(3, "KT", "\u03b1\u03b2\u03b2\u03b1", "Greek",
				"abba***; father***", null, null, 4101403603));
			terms.KeyTerms.Add(new Term(7325, "PN",
				"\u05DB\u05BC\u05B7\u05D1\u05BC\u05D5\u05B9\u05DF", "Hebrew",
				"Cabbon***", null, null, 601504001));

			ILangProject lp = m_fdoCache.LangProject;
			ICmPossibilityList chkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(chkTermsList);

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(1);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(3);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("KT", "Key terms"));
			loc.Categories.Add(new CategoryLocalization("PN", "Proper names"));
			loc.Terms.Add(new TermLocalization(1, "Aaron", "the elder brother of Moses"));
			loc.Terms.Add(new TermLocalization(3, "abba; father", "title for God, literally dad"));
			loc.Terms.Add(new TermLocalization(7325, "Cabbon", "town; territory of Judah"));
			localizations.Add(loc);

			// Load the terms
			DummyTeKeyTermsInit.CallLoadKeyTerms(chkTermsList, terms, localizations);

			int wsGreek = m_wsf.GetWsFromStr("grc");
			int wsHebrew = m_wsf.GetWsFromStr("hbo");

			// Make sure there are two categories (possibilities)
			Assert.AreEqual(2, chkTermsList.PossibilitiesOS.Count);
			ICmPossibility ktCategory = null, pnCategory = null;
			foreach (ICmPossibility cat in chkTermsList.PossibilitiesOS)
			{
				string catNameEn = cat.Name.GetAlternative(m_wsEn);
				switch (catNameEn)
				{
					case "Key terms": ktCategory = cat; break;
					case "Proper names": pnCategory = cat; break;
					default: Assert.Fail("Unexpected category in the list: " + catNameEn); break;
				}
			}

			// ********* Key Term Category **********
			Assert.IsNotNull(ktCategory);
			// Make sure there is one keyterm (possibility)
			Assert.AreEqual(1, ktCategory.SubPossibilitiesOS.Count);

			// Look at the contents of the ChkTerm
			IChkTerm keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(3, keyterm.TermId);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("abba", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("father", keyterm.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);

			// There should be 1 reference for this key term
			Assert.AreEqual(1, keyterm.OccurrencesOS.Count);

			IChkRef reference = keyterm.OccurrencesOS[0];
			Assert.AreEqual(41014036, reference.Ref);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", reference.KeyWord.Text);
			Assert.AreEqual(3, reference.Location);

			// ********* Proper Names Category **********
			Assert.IsNotNull(pnCategory);
			Assert.AreEqual("PN", pnCategory.Abbreviation.GetAlternative(m_wsEn));

			// Make sure there are two terms (possibilities)
			Assert.AreEqual(2, pnCategory.SubPossibilitiesOS.Count);

			// Look at the contents of the first Proper Name
			IChkTerm pnAaron = (IChkTerm)pnCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(1, pnAaron.TermId);
			Assert.AreEqual("\u1F08\u03B1\u03C1\u1F7D\u03BD", pnAaron.Name.GetAlternative(wsGreek));
			Assert.AreEqual("Aaron", pnAaron.Name.GetAlternative(m_wsEn));
			Assert.IsNull(pnAaron.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, pnAaron.RenderingsOC.Count);

			// There should be 2 references for this key term
			Assert.AreEqual(2, pnAaron.OccurrencesOS.Count);

			reference = pnAaron.OccurrencesOS[0];
			Assert.AreEqual(42001005, reference.Ref);
			Assert.AreEqual("\u1F08\u03B1\u03C1\u1F7D\u03BD", reference.KeyWord.Text);
			Assert.AreEqual(22, reference.Location);

			reference = pnAaron.OccurrencesOS[1];
			Assert.AreEqual(44007040, reference.Ref);
			Assert.AreEqual("\u1F08\u03B1\u03C1\u1F7D\u03BD", reference.KeyWord.Text);
			Assert.AreEqual(3, reference.Location);

			// Look at the contents of the second Proper Name
			IChkTerm pnCabbon = (IChkTerm)pnCategory.SubPossibilitiesOS[1];
			Assert.AreEqual(7325, pnCabbon.TermId);
			Assert.AreEqual("\u05DB\u05BC\u05B7\u05D1\u05BC\u05D5\u05B9\u05DF",
				pnCabbon.Name.GetAlternative(wsHebrew));
			Assert.AreEqual("Cabbon", pnCabbon.Name.GetAlternative(m_wsEn));
			Assert.IsNull(pnCabbon.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, pnCabbon.RenderingsOC.Count);

			// There should be 1 reference for this key term
			Assert.AreEqual(1, pnCabbon.OccurrencesOS.Count);

			reference = pnCabbon.OccurrencesOS[0];
			Assert.AreEqual(6015040, reference.Ref);
			Assert.AreEqual("\u05DB\u05BC\u05B7\u05D1\u05BC\u05D5\u05B9\u05DF",
				reference.KeyWord.Text);
			Assert.AreEqual(1, reference.Location);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test creating a key term with multiple localizations
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void MultipleLocalizations()
		{
			CheckDisposed();

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			terms.KeyTerms.Add(new Term(3, "KT", "\u03b1\u03b2\u03b2\u03b1", "Greek",
				"abba; father", null, null, 4101403603));

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(2);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(1);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("KT", "Key terms"));
			loc.Terms.Add(new TermLocalization(3, "abba; father", "title for God, literally dad"));
			localizations.Add(loc);
			loc = new BiblicalTermsLocalization(1);
			int wsEs = m_wsf.GetWsFromStr("es");
			loc.WritingSystemHvo = wsEs;
			loc.Categories.Add(new CategoryLocalization("KT", "Palabras claves"));
			loc.Terms.Add(new TermLocalization(3, "abba; padre", "t\u00EDtulo de Dios, literalmente pap\u00E1"));
			localizations.Add(loc);

			ILangProject lp = m_fdoCache.LangProject;
			ICmPossibilityList chkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(chkTermsList);

			// Load the term
			DummyTeKeyTermsInit.CallLoadKeyTerms(chkTermsList, terms, localizations);

			// Make sure there is one category (possibility)
			Assert.AreEqual(1, chkTermsList.PossibilitiesOS.Count);
			ICmPossibility ktCategory = chkTermsList.PossibilitiesOS[0];
			Assert.AreEqual("KT", ktCategory.Abbreviation.GetAlternative(m_wsEn));
			Assert.AreEqual("Key terms", ktCategory.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("Palabras claves", ktCategory.Name.GetAlternative(wsEs));

			// Make sure there is one keyterm in that category
			Assert.AreEqual(1, ktCategory.SubPossibilitiesOS.Count);

			// Look at the contents of the ChkTerm
			IChkTerm keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(3, keyterm.TermId);
			// Check the English localization
			Assert.AreEqual("abba", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("title for God, literally dad",
				keyterm.Description.GetAlternative(m_wsEn).Text);
			Assert.AreEqual("father", keyterm.SeeAlso.GetAlternative(m_wsEn));

			// Check the Spanish localization
			Assert.AreEqual("abba", keyterm.Name.GetAlternative(wsEs));
			Assert.AreEqual("t\u00EDtulo de Dios, literalmente pap\u00E1",
				keyterm.Description.GetAlternative(wsEs).Text);
			Assert.AreEqual("padre", keyterm.SeeAlso.GetAlternative(wsEs));
		}
		#endregion

		#region Tests for upgrading a list of key terms from the old style list
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test upgrading the list of key terms to the new BiblicalTerms format when the
		/// existing project has no renderings assigned (TE-6810)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void SimpleUpgradeWhenExistingListHasNoRenderings()
		{
			CheckDisposed();

			ILangProject lp = m_fdoCache.LangProject;
			ICmPossibilityList oldKeyTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(oldKeyTermsList);
			ChkTerm term = new ChkTerm();
			oldKeyTermsList.PossibilitiesOS.Append(term);
			term.Name.SetAlternative("Abba", m_wsEn);
			term.Description.SetAlternative("Daddy", m_wsEn);
			AddOccurrenceToOldStyleSense(term, 41014036, null, "abba");

			term = new ChkTerm();
			oldKeyTermsList.PossibilitiesOS.Append(term);
			term.Name.SetAlternative("Angel", m_wsEn);
			ChkTerm subsense = new ChkTerm();
			term.SubPossibilitiesOS.Append(subsense);
			subsense.Name.SetAlternative("Heavenly being", m_wsEn);
			subsense.Description.SetAlternative("Supernatural being sent as a messenger from God", m_wsEn);
			AddOccurrenceToOldStyleSense(subsense, 040001020, null, "aggelos");

			subsense = new ChkTerm();
			term.SubPossibilitiesOS.Append(subsense);
			subsense.Name.SetAlternative("Demon", m_wsEn);
			subsense.Description.SetAlternative("A fallen angel", m_wsEn);
			AddOccurrenceToOldStyleSense(subsense, 040007022, null, "daimonion");

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			terms.KeyTerms.Add(new Term(3, "KT", "\u03b1\u03b2\u03b2\u03b1", "Greek",
				"abba; father", null, null, 4101403603, 4500801516, 4800400618));
			terms.KeyTerms.Add(new Term(21, "KT", "\u1F04\u03B3\u03B3\u03B5\u03BB\u03BF\u03C2-1", "Greek",
				"angel", null, null, 040001020));
			terms.KeyTerms.Add(new Term(363, "KT", "\u03B4\u03B1\u03B9\u03BC\u1F79\u03BD\u03B9\u03BF\u03BD",
				"Greek", "demon", "\u1F04\u03B3\u03B3\u03B5\u03BB\u03BF\u03C2",
				"\u03B4\u03B1\u03B9\u03BC\u03BF\u03BD\u1F77\u03B6\u03BF\u03BC\u03B1\u03B9, \u03B4\u03B1\u03B9\u03BC\u03BF\u03BD\u03B9\u1F7D\u03B4\u03B7\u03C2, \u03B4\u03B1\u1F77\u03BC\u03C9\u03BD",
				040007022));

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(1);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(1);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("KT", "Key terms"));
			loc.Terms.Add(new TermLocalization(3, "abba; father", "title for God, literally dad"));
			localizations.Add(loc);

			// Load the terms
			ICmPossibilityList newChkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(newChkTermsList);
			DummyTeKeyTermsInit.CallLoadKeyTerms(oldKeyTermsList, newChkTermsList, terms, localizations);

			int wsGreek = m_wsf.GetWsFromStr("grc");

			// Make sure the old list has been blown away
			// We can't do this now because we delete the list asynchronously
			// Assert.IsFalse(oldKeyTermsList.IsValidObject());
			oldKeyTermsList = null;

			// Make sure there is one category (possibility) in the new list
			Assert.AreEqual(1, newChkTermsList.PossibilitiesOS.Count);
			ICmPossibility ktCategory = newChkTermsList.PossibilitiesOS[0];
			Assert.AreEqual("KT", ktCategory.Abbreviation.GetAlternative(m_wsEn));

			// Make sure there are three keyterms in that category
			Assert.AreEqual(3, ktCategory.SubPossibilitiesOS.Count);

			// Look at the contents of the first ChkTerm
			IChkTerm keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(3, keyterm.TermId);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("abba", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("title for God, literally dad",
				keyterm.Description.GetAlternative(m_wsEn).Text);
			Assert.AreEqual("father", keyterm.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);
			Assert.AreEqual(3, keyterm.OccurrencesOS.Count, "There should be 3 references for abba");
			Assert.AreEqual(0, keyterm.OccurrencesOS[0].RenderingRAHvo);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[0].Status);
			Assert.AreEqual(0, keyterm.OccurrencesOS[1].RenderingRAHvo);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[1].Status);
			Assert.AreEqual(0, keyterm.OccurrencesOS[2].RenderingRAHvo);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[2].Status);

			// Briefly check the other two ChkTerms
			keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[1];
			Assert.AreEqual(21, keyterm.TermId);
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);
			Assert.AreEqual(1, keyterm.OccurrencesOS.Count);
			Assert.AreEqual(0, keyterm.OccurrencesOS[0].RenderingRAHvo);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[0].Status);

			keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[2];
			Assert.AreEqual(363, keyterm.TermId);
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);
			Assert.AreEqual(1, keyterm.OccurrencesOS.Count);
			Assert.AreEqual(0, keyterm.OccurrencesOS[0].RenderingRAHvo);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[0].Status);

			bool fFoundResource = false;
			foreach (CmResource resource in lp.TranslatedScriptureOA.ResourcesOC)
			{
				if (resource.Name == "BiblicalTerms")
				{
					Assert.AreEqual(terms.Version, resource.Version);
					fFoundResource = true;
				}
			}
			Assert.IsTrue(fFoundResource);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test upgrading the list of key terms to the new BiblicalTerms format when the
		/// existing project has some renderings assigned (TE-6803 & TE-6806)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void UpgradeWhenExistingListHasRenderings()
		{
			CheckDisposed();

			ILangProject lp = m_fdoCache.LangProject;

			IWfiWordform abc = lp.WordformInventoryOA.AddRealWordform("abc", lp.DefaultVernacularWritingSystem);
			IWfiWordform def = lp.WordformInventoryOA.AddRealWordform("def", lp.DefaultVernacularWritingSystem);
			IWfiWordform ghi = lp.WordformInventoryOA.AddRealWordform("ghi", lp.DefaultVernacularWritingSystem);

			ICmPossibilityList oldKeyTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(oldKeyTermsList);
			ChkTerm term = new ChkTerm();
			oldKeyTermsList.PossibilitiesOS.Append(term);
			term.Name.SetAlternative("Abba", m_wsEn);
			term.Description.SetAlternative("Daddy", m_wsEn);
			AddOccurrenceToOldStyleSense(term, 41014036, abc, "abba", KeyTermRenderingStatus.Assigned);
			AddOccurrenceToOldStyleSense(term, 45008015, abc, "abba", KeyTermRenderingStatus.AutoAssigned);
			AddOccurrenceToOldStyleSense(term, 48004006, null, "abba"); // Not rendered
			AddOccurrenceToOldStyleSense(term, 48004007, abc, "abba"); // This one is not in new list

			term = new ChkTerm();
			oldKeyTermsList.PossibilitiesOS.Append(term);
			term.Name.SetAlternative("Angel", m_wsEn);
			ChkTerm subsense = new ChkTerm();
			term.SubPossibilitiesOS.Append(subsense);
			subsense.Name.SetAlternative("Heavenly being", m_wsEn);
			subsense.Description.SetAlternative("Supernatural being sent as a messenger from God", m_wsEn);
			AddOccurrenceToOldStyleSense(subsense, 040001020, def, "aggelos");
			AddOccurrenceToOldStyleSense(subsense, 040001024, ghi, "aggelos");
			AddOccurrenceToOldStyleSense(subsense, 040002013, null, "aggelos", KeyTermRenderingStatus.Ignored);

			subsense = new ChkTerm();
			term.SubPossibilitiesOS.Append(subsense);
			subsense.Name.SetAlternative("Demon", m_wsEn);
			subsense.Description.SetAlternative("A fallen angel", m_wsEn);
			AddOccurrenceToOldStyleSense(subsense, 040007022, null, "daimonion");

			BiblicalTermsList terms = new BiblicalTermsList();
			terms.Version = Guid.NewGuid();
			terms.KeyTerms = new List<Term>();
			terms.KeyTerms.Add(new Term(3, "KT", "\u03b1\u03b2\u03b2\u03b1", "Greek",
				"abba; father", null, null, 4101403603, 4500801516, 4800400618));
			terms.KeyTerms.Add(new Term(21, "KT", "\u1F04\u03B3\u03B3\u03B5\u03BB\u03BF\u03C2", "Greek",
				"angel", null, null, 04000102006, 04000102413, 04000201305));
			terms.KeyTerms.Add(new Term(363, "KT", "\u03B4\u03B1\u03B9\u03BC\u1F79\u03BD\u03B9\u03BF\u03BD-1",
				"Greek", "demon", "\u03B4\u03B1\u03B9\u03BC\u1F79\u03BD\u03B9\u03BF\u03BD",
				"\u03B4\u03B1\u03B9\u03BC\u03BF\u03BD\u1F77\u03B6\u03BF\u03BC\u03B1\u03B9, \u03B4\u03B1\u03B9\u03BC\u03BF\u03BD\u03B9\u1F7D\u03B4\u03B7\u03C2, \u03B4\u03B1\u1F77\u03BC\u03C9\u03BD",
				04000702219));

			List<BiblicalTermsLocalization> localizations = new List<BiblicalTermsLocalization>(1);
			BiblicalTermsLocalization loc = new BiblicalTermsLocalization(1);
			loc.WritingSystemHvo = m_wsEn;
			loc.Categories.Add(new CategoryLocalization("KT", "Key terms"));
			loc.Terms.Add(new TermLocalization(3, "abba; father", "title for God, literally dad"));
			localizations.Add(loc);

			// Load the terms
			ICmPossibilityList newChkTermsList = new CmPossibilityList();
			lp.CheckListsOC.Add(newChkTermsList);
			DummyTeKeyTermsInit.CallLoadKeyTerms(oldKeyTermsList, newChkTermsList, terms,
				localizations);

			int wsGreek = m_wsf.GetWsFromStr("grc");

			// Make sure the old list has been blown away
			// We can't do this now because we delete the list asynchronously
			// Assert.IsFalse(oldKeyTermsList.IsValidObject());
			oldKeyTermsList = null;

			// Make sure there is one category (possibility) in the new list
			ICmPossibility ktCategory = newChkTermsList.PossibilitiesOS[0];
			Assert.AreEqual("KT", ktCategory.Abbreviation.GetAlternative(m_wsEn));

			// Make sure there are three keyterms in that category
			Assert.AreEqual(3, ktCategory.SubPossibilitiesOS.Count);

			// Check the ChkTerm and ChkRefs for "abba"
			IChkTerm keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[0];
			Assert.AreEqual(3, keyterm.TermId);
			Assert.AreEqual("\u03b1\u03b2\u03b2\u03b1", keyterm.Name.GetAlternative(wsGreek));
			Assert.AreEqual("abba", keyterm.Name.GetAlternative(m_wsEn));
			Assert.AreEqual("title for God, literally dad",
				keyterm.Description.GetAlternative(m_wsEn).Text);
			Assert.AreEqual("father", keyterm.SeeAlso.GetAlternative(m_wsEn));
			Assert.AreEqual(1, keyterm.RenderingsOC.Count);
			ChkRendering abbaRendering = new ChkRendering(m_fdoCache, keyterm.RenderingsOC.HvoArray[0]);
			Assert.AreEqual(abc, abbaRendering.SurfaceFormRA);
			Assert.AreEqual(3, keyterm.OccurrencesOS.Count, "There should be 3 references for abba");
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, keyterm.OccurrencesOS[0].Status);
			Assert.AreEqual(abc, keyterm.OccurrencesOS[0].RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, keyterm.OccurrencesOS[1].Status);
			Assert.AreEqual(abc, keyterm.OccurrencesOS[1].RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[2].Status);
			Assert.AreEqual(0, keyterm.OccurrencesOS[2].RenderingRAHvo);

			// Check the ChkRefs for "angel"
			keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[1];
			Assert.AreEqual(21, keyterm.TermId);
			Assert.AreEqual(2, keyterm.RenderingsOC.Count);
			bool fFoundDEF = false;
			bool fFoundGHI = false;
			foreach (IChkRendering rendering in keyterm.RenderingsOC)
			{
				fFoundDEF |= (rendering.SurfaceFormRAHvo == def.Hvo);
				fFoundGHI |= (rendering.SurfaceFormRAHvo == ghi.Hvo);
			}
			Assert.IsTrue(fFoundDEF);
			Assert.IsTrue(fFoundGHI);
			Assert.AreEqual(3, keyterm.OccurrencesOS.Count);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, keyterm.OccurrencesOS[0].Status);
			Assert.AreEqual(def, keyterm.OccurrencesOS[0].RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, keyterm.OccurrencesOS[1].Status);
			Assert.AreEqual(ghi, keyterm.OccurrencesOS[1].RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Ignored, keyterm.OccurrencesOS[2].Status);
			Assert.IsNull(keyterm.OccurrencesOS[2].RenderingRA);

			// Check the ChkRefs for "demon"
			keyterm = (IChkTerm)ktCategory.SubPossibilitiesOS[2];
			Assert.AreEqual(363, keyterm.TermId);
			Assert.AreEqual(0, keyterm.RenderingsOC.Count);
			Assert.AreEqual(1, keyterm.OccurrencesOS.Count);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, keyterm.OccurrencesOS[0].Status);
			Assert.AreEqual(0, keyterm.OccurrencesOS[0].RenderingRAHvo);
		}
		#endregion
	}
}
#endregion