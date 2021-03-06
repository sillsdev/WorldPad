// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2008, SIL International. All Rights Reserved.
// <copyright from='2006' to='2008' company='SIL International'>
//		Copyright (c) 2008, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: KeyTermsTests.cs
// Responsibility: TE Team
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using NUnit.Framework;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Scripture;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.Controls;
using SIL.FieldWorks.Common.RootSites;
using SIL.FieldWorks.Common.Framework;
using Microsoft.Win32;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.Common.ScriptureUtils;

namespace SIL.FieldWorks.TE
{

	internal class DummyKeyTermsViewWrapper : KeyTermsViewWrapper
	{
		/// ------------------------------------------------------------------------------------
		public DummyKeyTermsViewWrapper(Control parent, FdoCache cache, object createInfo, RegistryKey settingsRegKey)
			: base(parent, cache, createInfo, settingsRegKey, 0, string.Empty)
		{
		}

		internal KeyTermsTree KeyTermsTree
		{
			get { return m_ktTree; }
		}
	}

	/// <summary>
	/// Summary description for KeyTermsTests.
	/// </summary>
	[TestFixture]
	public class KeyTermsTests : TeTestBase
	{
		private DummyDraftViewForm m_draftForm;
		private DummyDraftView m_draftView;
		private DummyKeyTermsViewWrapper m_ktVwWrapper;
		private IScrBook m_book;

		#region DummyDbOps
		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public class DummyDbOps : DbOps
		{
			/// ------------------------------------------------------------------------------------
			/// <summary>
			///
			/// </summary>
			/// ------------------------------------------------------------------------------------
			static public void SetDummyOps()
			{
				s_DbOps = new DummyDbOps();
			}

			/// <summary>
			/// Restore DbOps to using its own interfaces.
			/// </summary>
			static public void DisableDummyOps()
			{
				s_DbOps = new DbOps();
			}

			/// ------------------------------------------------------------------------------------
			/// <summary>
			///
			/// </summary>
			/// <param name="cache"></param>
			/// <param name="sql"></param>
			/// <param name="param"></param>
			/// <param name="val"></param>
			/// <returns></returns>
			/// ------------------------------------------------------------------------------------
			protected override bool InternalReadOneIntFromCommand(FdoCache cache, string sql, string param,
				out int val)
			{
				val = 0;
				return false;
			}

			/// <summary>
			///
			/// </summary>
			/// <param name="cache"></param>
			/// <param name="sql"></param>
			/// <param name="param"></param>
			/// <returns></returns>
			protected override List<int> InternalReadIntsFromCommand(FdoCache cache, string sql, string param)
			{
				return new List<int>(new int[] { });
			}
		}
		#endregion

		#region Setup and Teardown
		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SetUp]
		public override void Initialize()
		{
			CheckDisposed();
			base.Initialize();

			DummyDbOps.SetDummyOps();
			Application.DoEvents();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TearDown]
		public override void Exit()
		{
			CheckDisposed();

			DummyDbOps.DisableDummyOps();
			m_ktVwWrapper = null;
			m_draftView = null;
			m_draftForm.Close();
			m_draftForm = null;
			m_book = null;

			base.Exit();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void CreateTestData()
		{
			Cache.MapType(typeof(ChkRef), typeof(KeyTermRef));

			// Create a word form inventory for the language project
			Cache.LangProject.WordformInventoryOA = new WordformInventory();
			// create a book with some scripture data in it
			m_book = m_scrInMemoryCache.AddBookToMockedScripture(40, "Matthew");
			IScrSection section = m_scrInMemoryCache.AddSectionToMockedBook(m_book.Hvo);
			m_scrInMemoryCache.AddSectionHeadParaToSection(section.Hvo, "Section Heading Text", ScrStyleNames.SectionHead);
			StTxtPara para = m_scrInMemoryCache.AddParaToMockedSectionContent(section.Hvo, ScrStyleNames.NormalParagraph);

			m_scrInMemoryCache.AddRunToMockedPara(para, "1", ScrStyleNames.ChapterNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "1", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "The beginning of Matthew has some words.", null);
			m_scrInMemoryCache.AddRunToMockedPara(para, "2", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "Verse two has even more stuff in it with words. And the word possible.", null);
			m_scrInMemoryCache.AddRunToMockedPara(para, "3", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "Verse three does not have the key term in it.", null);
			m_scrInMemoryCache.AddRunToMockedPara(para, "4", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "Verse four has the word possible in it.", null);
			m_scrInMemoryCache.AddRunToMockedPara(para, "5", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "Verse five has the wrong word impossible in it.", null);
			section.AdjustReferences();

			// Create a key term for the term "words"
			IChkTerm keyTermWords = CreateChkTerm("words");

			// Create references of MAT 1:1, MAT 1:2, and MAT 1:3 for the key term "words"
			CreateChkRef(keyTermWords, 40001001);
			CreateChkRef(keyTermWords, 40001002);
			CreateChkRef(keyTermWords, 40001003);

			// Create a key term for the term "possible"
			IChkTerm keyTermPossible = CreateChkTerm("possible");

			// Create references of MAT 1:2, MAT 1:4, and MAT 1:5 for the key term "possible"
			CreateChkRef(keyTermPossible, 40001002);
			CreateChkRef(keyTermPossible, 40001004);
			CreateChkRef(keyTermPossible, 40001005);

			// Create a draft form, draft view, and a key terms view.
			if (m_draftForm != null)
				m_draftForm.Dispose();
			m_draftForm = new DummyDraftViewForm(Cache);
			m_draftForm.DeleteRegistryKey();

			m_scr.RestartFootnoteSequence = true;

			// Create a key terms view
			ChecksDraftViewCreateInfo keyTermsCreateInfo = new ChecksDraftViewCreateInfo(
				"KeyTermsDraftView", true);
			m_ktVwWrapper = new DummyKeyTermsViewWrapper(m_draftForm, Cache,
				keyTermsCreateInfo, m_draftForm.SettingsKey);
			m_ktVwWrapper.DisplayUI = false;
			m_draftForm.Controls.Add(m_ktVwWrapper);

			// Fill in the renderings view with the Key Terms
			m_ktVwWrapper.Visible = true;
			((ISelectableView)m_ktVwWrapper).ActivateView();
			((DummyDraftView)m_ktVwWrapper.DraftView).MakeRoot();
			//m_keyTermsViewWrapper.RenderingsView.MakeRoot();
			m_draftForm.Show();

			m_draftView = m_draftForm.DraftView;
			m_draftView.Width = 20;
			m_draftView.Height = 20;
			m_draftView.CallOnLayout();
			m_draftForm.Hide();
		}

		private static IChkRef CreateChkRef(IChkTerm keyTermPossible, int bcv)
		{
			IChkRef chkRef = new ChkRef();
			keyTermPossible.OccurrencesOS.Append(chkRef);
			chkRef.Ref = bcv;
			return chkRef;
		}

		private IChkTerm CreateChkTerm(string name)
		{
			ICmPossibilityList keyTermsList = Cache.LangProject.KeyTermsList;
			IChkTerm keyTerm = keyTermsList.PossibilitiesOS.Append(new ChkTerm()) as IChkTerm;
			keyTerm.Name.SetAlternative(name, Cache.DefaultUserWs);
			return keyTerm;
		}

		private IChkTerm CreateSubChkTerm(IChkTerm parentKeyTerm, string name)
		{
			IChkTerm keyTerm = parentKeyTerm.SubPossibilitiesOS.Append(new ChkTerm()) as IChkTerm;
			keyTerm.Name.SetAlternative(name, Cache.DefaultUserWs);
			return keyTerm;
		}
		#endregion

		#region Helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetCurrentLineInRenderingCtrl(int index)
		{
			KeyTermsGrid grid = ReflectionHelper.GetField(m_ktVwWrapper.RenderingsControl,
				"m_dataGridView") as KeyTermsGrid;

			if (grid == null || index < 0 || index >= grid.Rows.Count)
				return;

			grid.ClearSelection();
			grid.CurrentCell = grid[0, index];
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Selects the given term in the KT tree and the given occurrence in the list of
		/// occurrences.
		/// </summary>
		/// <param name="sTerm">The analysis term.</param>
		/// <param name="iOccurrence">The i occurrence.</param>
		/// <returns>The selected term</returns>
		/// ------------------------------------------------------------------------------------
		private IChkTerm SelectTermAndOccurrence(string sTerm, int iOccurrence)
		{
			IChkTerm term = SelectTerm(sTerm);
			LoadRenderingsAndSelectOccurrenceInRenderingCtrl(term, iOccurrence);
			return term;
		}

		private IChkRef LoadRenderingsAndSelectOccurrenceInRenderingCtrl(IChkTerm term, int iOccurrence)
		{
			m_ktVwWrapper.RenderingsControl.LoadRenderingsForKeyTerm(term.Hvo, null);
			SetCurrentLineInRenderingCtrl(iOccurrence);
			return term.OccurrencesOS[iOccurrence];
		}

		private IChkTerm SelectTerm(string sTerm)
		{
			int iTerm = 0;
			switch (sTerm)
			{
				case "word": iTerm = 0; break;
				case "possible": iTerm = 1; break;
			}
			IChkTerm term = (IChkTerm)Cache.LangProject.KeyTermsList.PossibilitiesOS[iTerm];
			return term;
		}
		#endregion

		#region BookFilter tests

		void CheckOccurrencesCount(int expectedCount, TreeNode node)
		{
			int hvoKeyTerm = (int)node.Tag;
			ChkTerm keyTerm = new ChkTerm(Cache, hvoKeyTerm, false, false);
			Assert.AreEqual(expectedCount, keyTerm.OccurrencesOS.Count);
		}

		void CheckRenderingsCount(int expectedCount, TreeNode nodeToSelect)
		{
			m_ktVwWrapper.KeyTermsTree.SelectedNode = nodeToSelect;
			KeyTermRenderingsControl renderingsControl = m_ktVwWrapper.RenderingsControl;
			Assert.AreEqual(expectedCount, renderingsControl.ReferenceCount);
		}

		/// <summary>
		/// TE-4500. Filter the terms list in the Key Terms view.
		/// </summary>
		[Test]
		public void ApplyBookFilterToKeyTermsTree()
		{
			// Add mark and key terms/occurrences to matthew setup.
			IScrBook mark = AddBookMark();
			IChkTerm nonsense1 = CreateChkTerm("nonsense1");
			IChkTerm nonsense1_1 = CreateSubChkTerm(nonsense1, "nonsense1_1");
			CreateChkRef(nonsense1_1, 40001004); // Mat 1:4
			CreateChkRef(nonsense1_1, 41001001); // Mar 1:1
			IChkTerm nonsense1_2 = CreateSubChkTerm(nonsense1, "nonsense1_2");
			CreateChkRef(nonsense1_2, 41001001); // Mar 1:1

			m_ktVwWrapper.ApplyBookFilter = false;
			m_draftView.BookFilter.ShowAllBooks();
			m_ktVwWrapper.UpdateBookFilter();
			KeyTermsTree ktree = m_ktVwWrapper.KeyTermsTree;
			// first make sure we're setup for all the books (i.e. no book filter).
			Assert.IsTrue(m_draftView.BookFilter.AllBooks);
			// key terms "nonsense1", "possible", and "word"
			Assert.AreEqual(3, ktree.Nodes.Count);
			// occurrences of "word" (all in Matthew)
			CheckRenderingsCount(3, ktree.Nodes[2]);
			// sub items for "nonsense"
			Assert.AreEqual(2, ktree.Nodes[0].Nodes.Count);
			// occurrences for nonsense1_1 (in Matthew and Mark)
			CheckRenderingsCount(2, ktree.Nodes[0].Nodes[0]);
			// occurrences for nonsense1_2 (in Mark)
			CheckRenderingsCount(1, ktree.Nodes[0].Nodes[1]);

			// Add Matthew to BookFilter.
			m_ktVwWrapper.ApplyBookFilter = false;
			m_draftView.BookFilter.UpdateFilter(new int[] { m_book.Hvo });
			m_ktVwWrapper.UpdateBookFilter();
			// Check KeyTerms
			Assert.AreEqual(1, m_draftView.BookFilter.BookCount);
			// key terms "nonsense1", "possible", and "word"
			Assert.AreEqual(3, ktree.Nodes.Count);
			// occurrences of "word" (all in Matthew)
			CheckRenderingsCount(3, ktree.Nodes[2]);
			// sub items for "nonsense"
			Assert.AreEqual(1, ktree.Nodes[0].Nodes.Count);
			// occurrences for nonsense1_1 (in Matthew and Mark)
			CheckRenderingsCount(2, ktree.Nodes[0].Nodes[0]);
			// Apply Book Filter (Matthew) to Occurrences.
			m_ktVwWrapper.ApplyBookFilter = true;
			CheckRenderingsCount(1, ktree.Nodes[0].Nodes[0]);

			// Remove Matthew and Add Mark
			m_ktVwWrapper.ApplyBookFilter = false;
			m_draftView.BookFilter.UpdateFilter(new int[] { mark.Hvo });
			m_ktVwWrapper.UpdateBookFilter();
			// Check KeyTerms
			Assert.AreEqual(1, m_draftView.BookFilter.BookCount);
			// key terms "nonsense" in Mark
			Assert.AreEqual(1, ktree.Nodes.Count);
			// sub items for "nonsense"
			Assert.AreEqual(2, ktree.Nodes[0].Nodes.Count);
			// occurrences for nonsense1_2 (in Mark)
			CheckRenderingsCount(1, ktree.Nodes[0].Nodes[1]);
			// occurrences for nonsense1_1 (in Matthew and Mark)
			CheckRenderingsCount(2, ktree.Nodes[0].Nodes[0]);
			m_ktVwWrapper.ApplyBookFilter = true;
			// Apply Book Filter (Mark) to Occurrences.
			CheckRenderingsCount(1, ktree.Nodes[0].Nodes[0]);
		}

		#endregion BookFilter tests

		#region AssignVernacularEquivalent tests
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method with an explicitly-defined rendering,
		/// an automatically-defined rendering, and a verse missing the key term rendering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent()
		{
			CheckDisposed();

			// Select the word "words" in MAT 1:1 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 36, 41);

			// select the key term "words" and its first reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 0);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// check it
			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("words", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method with an explicitly-defined rendering,
		/// and an automatically-defined rendering. Then explicitly define the automatically-
		/// defined rendering. (TE-6265)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_AfterAutoAssign()
		{
			CheckDisposed();

			// Select the word "words" in MAT 1:1 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 36, 41);

			// select the key term "words" and its first reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 0);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// Confirm that the second occurrence is auto-assigned with "words".
			Assert.AreEqual(3, keyTerm1.OccurrencesOS.Count);
			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref2.Status);

			// Now select and assign the word "stuff" for the autoassigned verse in MAT 1:2.
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 67, 72);
			SetCurrentLineInRenderingCtrl(1);

			m_ktVwWrapper.AssignVernacularEquivalent();

			// We expect that the status of this occurrence would now be "assigned" and the
			// vernacular rendering would be "stuff".
			wordform2 = ref2.RenderingRA;
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref2.Status);
			Assert.AreEqual("stuff", wordform2.Form.VernacularDefaultWritingSystem);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method with an explicitly-defined rendering when
		/// the rendering contains an embedded footnote marker. The case for an automatically-defined
		/// rendering and a verse missing the key term rendering are also handled. (TE-5445)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_WithFootnoteEmbedded()
		{
			CheckDisposed();

			// Insert a footnote into the middle of the word "words" which will be assigned as
			// the vernacular equivalent.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = (StTxtPara)section.ContentOA.ParagraphsOS[0];
			m_scrInMemoryCache.AddFootnote(m_book, para, 38);

			// Select the word "words" in MAT 1:1 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 36, 42);

			// select the key term "words" and its first reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 0);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// check it
			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("words", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method with an explicitly-defined rendering,
		/// an automatically-defined rendering, and a word that contains the key term.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_WholeWords()
		{
			CheckDisposed();

			// select the key term "possible" and its first reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("possible", 0);

			// Select the word "possible" in MAT 1:2 in the draft view
			IStTxtPara para = (IStTxtPara)m_book.SectionsOS[0].ContentOA.ParagraphsOS[0];
			int ichPossible = para.Contents.Text.IndexOf("possible");
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, ichPossible, ichPossible + 8);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// check it
			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("possible", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("possible", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method in multiple books with an explicitly-
		/// defined rendering, an automatically-defined rendering, and a verse missing the key
		/// term rendering.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_MultipleBooks()
		{
			CheckDisposed();

			// create another book with some scripture data in it
			AddBookMark();
			IChkTerm keyTerm1 = SetupKeyTermsForMark();

			// check it
			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("words", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);

			IChkRef ref4 = keyTerm1.OccurrencesOS[3];
			IWfiWordform wordform4 = ref4.RenderingRA;
			Assert.AreEqual("words", wordform4.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref4.Status);

			IChkRef ref5 = keyTerm1.OccurrencesOS[4];
			IWfiWordform wordform5 = ref5.RenderingRA;
			Assert.AreEqual("words", wordform5.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref5.Status);
		}

		private IScrBook AddBookMark()
		{
			IScrBook mark = m_scrInMemoryCache.AddBookToMockedScripture(41, "Mark");
			IScrSection section = m_scrInMemoryCache.AddSectionToMockedBook(mark.Hvo);
			m_scrInMemoryCache.AddSectionHeadParaToSection(section.Hvo, "Section Heading Text", ScrStyleNames.SectionHead);
			StTxtPara para = m_scrInMemoryCache.AddParaToMockedSectionContent(section.Hvo, ScrStyleNames.NormalParagraph);

			m_scrInMemoryCache.AddRunToMockedPara(para, "1", ScrStyleNames.ChapterNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "1", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "The beginning of Mark has some words.", null);
			m_scrInMemoryCache.AddRunToMockedPara(para, "2", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "Verse two has even more stuff in it with words.", null);
			m_scrInMemoryCache.AddRunToMockedPara(para, "3", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "Verse three does not have the key term in it.", null);
			section.AdjustReferences();
			return mark;
		}

		/// <summary>
		/// Add occurrences to "words" occurrences.
		/// </summary>
		/// <returns></returns>
		private IChkTerm SetupKeyTermsForMark()
		{
			// select the key term "words" and its first reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 0);

			AppendMarkOccurrencesToKeyTerm(keyTerm1);

			// Select the word "words" in MAT 1:1 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 36, 41);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();
			return keyTerm1;
		}

		private static void AppendMarkOccurrencesToKeyTerm(IChkTerm keyTerm1)
		{
			// Create references of MRK 1:1 for the key term
			CreateChkRef(keyTerm1, 41001001);

			// Create references of MRK 1:2 for the key term
			CreateChkRef(keyTerm1, 41001002);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method when one of the automatically-defined
		/// key terms is the first reference of a verse bridge.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_AtVerseBridgeStart()
		{
			CheckDisposed();

			// Add para with verse bridge at end of first section.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = m_scrInMemoryCache.AddParaToMockedSectionContent(section.Hvo,
				ScrStyleNames.NormalParagraph);
			m_scrInMemoryCache.AddRunToMockedPara(para, "6-8", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "This verse bridge also has some words.", null);
			section.AdjustReferences();

			// select the key term "words" and its second reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 1);

			// Create references of MAT 1:6 for the key term
			IChkRef newRef4 = new ChkRef();
			keyTerm1.OccurrencesOS.Append(newRef4);
			newRef4.Ref = 40001006;

			// Select the word "words" in MAT 1:2 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 83, 89);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// check it
			Assert.AreEqual(4, keyTerm1.OccurrencesOS.Count);

			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("words", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);

			IChkRef ref4 = keyTerm1.OccurrencesOS[3];
			IWfiWordform wordform4 = ref4.RenderingRA;
			Assert.AreEqual("words", wordform4.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref4.Status);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method when one of the automatically-defined
		/// key terms is within a verse bridge.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_WithinVerseBridge()
		{
			CheckDisposed();

			// Add para with verse bridge at end of first section.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = m_scrInMemoryCache.AddParaToMockedSectionContent(section.Hvo,
				ScrStyleNames.NormalParagraph);
			m_scrInMemoryCache.AddRunToMockedPara(para, "6-8", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "This verse bridge also has some words.", null);
			section.AdjustReferences();

			// select the key term "words" and its second reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 1);

			// Create references for MAT 1:7 for the key term
			IChkRef newRef4 = new ChkRef();
			keyTerm1.OccurrencesOS.Append(newRef4);
			newRef4.Ref = 40001007;

			// Select the word "words" in MAT 1:2 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 83, 89);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// check it
			Assert.AreEqual(4, keyTerm1.OccurrencesOS.Count);

			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("words", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);

			IChkRef ref4 = keyTerm1.OccurrencesOS[3];
			IWfiWordform wordform4 = ref4.RenderingRA;
			Assert.AreEqual("words", wordform4.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref4.Status);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the AssignVernacularEquivalent method when one of the automatically-defined
		/// key terms is the last reference of a verse bridge.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AssignVernacularEquivalent_AtVerseBridgeEnd()
		{
			CheckDisposed();

			// Add para with verse bridge at end of first section.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = m_scrInMemoryCache.AddParaToMockedSectionContent(section.Hvo,
				ScrStyleNames.NormalParagraph);
			m_scrInMemoryCache.AddRunToMockedPara(para, "6-8", ScrStyleNames.VerseNumber);
			m_scrInMemoryCache.AddRunToMockedPara(para, "This verse bridge also has some words.", null);
			section.AdjustReferences();

			// select the key term "words" and its second reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 1);

			// Create references of MAT 1:8 for the key term
			IChkRef newRef4 = new ChkRef();
			keyTerm1.OccurrencesOS.Append(newRef4);
			newRef4.Ref = 40001008;

			// Select the word "words" in MAT 1:2 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 83, 89);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();

			// check it
			Assert.AreEqual(4, keyTerm1.OccurrencesOS.Count);

			IChkRef ref1 = keyTerm1.OccurrencesOS[0];
			IWfiWordform wordform1 = ref1.RenderingRA;
			Assert.AreEqual("words", wordform1.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref1.Status);

			IChkRef ref2 = keyTerm1.OccurrencesOS[1];
			IWfiWordform wordform2 = ref2.RenderingRA;
			Assert.AreEqual("words", wordform2.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.Assigned, ref2.Status);

			IChkRef ref3 = keyTerm1.OccurrencesOS[2];
			Assert.AreEqual(null, ref3.RenderingRA);
			Assert.AreEqual(KeyTermRenderingStatus.Unassigned, ref3.Status);

			IChkRef ref4 = keyTerm1.OccurrencesOS[3];
			IWfiWordform wordform4 = ref4.RenderingRA;
			Assert.AreEqual("words", wordform4.Form.VernacularDefaultWritingSystem);
			Assert.AreEqual(KeyTermRenderingStatus.AutoAssigned, ref4.Status);
		}

		#region UpdateKeyTermEquivalents (TE-4164)

		/// <summary>
		/// Case: Check a rescan doesn't make any changes after non-keyterm-rendering text changes.
		/// </summary>
		[Test]
		public void UpdateKeyTermEquivalents_RescanWithNoChanges()
		{
			CheckDisposed();

			AssignVernacularEquivalentForWordsKeytermInMatthew1_1();
			AssignAlternativeRenderingForWordsKeytermInMatthew();

			IChkTerm keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents();

			// check that everything is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[1]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[3]);
		}

		/// <summary>
		/// Case: User adds an implicit keyterm rendering
		/// Check for a new implicit assignment
		/// </summary>
		[Test]
		public void UpdateKeyTermEquivalents_ChangesForUnassignedOccurrence()
		{
			CheckDisposed();
			AssignVernacularEquivalentForWordsKeytermInMatthew1_1();
			// add keyterm to second verse of text.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = (StTxtPara)section.ContentOA.ParagraphsOS[0];
			// Modify verse three as follows:
			// "Verse three does not have the key term in it."
			// "Verse three does have the words key term in it now."
			TextSelInfo tsiBeforeEdit;
			TextSelInfo tsiAfterEdit;
			ReplaceVerseText(para, 40001003, "Verse three does have the words key term in it now.",
				out tsiBeforeEdit, out tsiAfterEdit);
			IChkTerm keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			// check that we changed the status of the third occurrence
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[2]);
			// check that status of other renderings have not changed
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[1]);
		}

		/// <summary>
		/// Case 2a: User radically changes keyterm rendering
		///		Check that a "Assigned" rendering is now "Missing"
		/// Case 2b. User restores a keyterm rendering.
		///		Check that a "Missing" rendering in now "Assigned"
		/// Case 2c: User unradically changes an explicitly assigned keyterm rendering
		///		Check for a new implicit rendering assignment
		/// </summary>
		[Test]
		public void UpdateKeyTermEquivalents_ChangesToExplicitlyAssignedOccurrence()
		{
			CheckDisposed();
			// establish "words" as a rendering for "words" keyterm.
			AssignVernacularEquivalentForWordsKeytermInMatthew1_1();
			// establish "conversation" as an alternative rendering for "words" keyterm.
			AssignAlternativeRenderingForWordsKeytermInMatthew();
			// Change Assigned keyterm rendering to an alternative rendering.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = (StTxtPara)section.ContentOA.ParagraphsOS[0];

			// Case 2a: User radically changes the text containing the keyterm rendering
			// Modify verse 1 as follows:
			// "The beginning of Matthew has a conversation."
			// "The beginning of Matthew has a sentence."
			TextSelInfo tsiBeforeEdit;
			TextSelInfo tsiAfterEdit;
			ReplaceVerseText(para, 40001001, "The beginning of Matthew has a sentence.",
				out tsiBeforeEdit, out tsiAfterEdit);
			IChkTerm keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			// Check that a "Assigned" rendering is now "Missing"
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Missing,
				keyTerm_words.OccurrencesOS[0]);
			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[1]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[3]);
			// Case 2b. User restores a keyterm rendering.
			// "The beginning of Matthew has a sentence."
			// "The beginning of Matthew has some words."
			ReplaceVerseText(para, 40001001, "The beginning of Matthew has some words.",
				out tsiBeforeEdit, out tsiAfterEdit);
			keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			// Check that a "Missing" rendering is now "Assigned"
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[1]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[3]);

			// Case 2c: User unradically changes the text for an explicitly assigned keyterm rendering
			// Modify verse 1 as follows:
			// "The beginning of Matthew has some words."
			// "The beginning of Matthew has a conversation."
			ReplaceVerseText(para, 40001001, "The beginning of Matthew has a conversation.",
				out tsiBeforeEdit, out tsiAfterEdit);
			keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			// Check for the new implicit rendering assignment
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[0]);
			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[1]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[3]);
		}

		/// <summary>
		/// Case 3a: User unradically changes an implicit keyterm rendering
		///		Check for a new implicit rendering assignment
		/// Case 3b: User radically changes an implicit keyterm rendering
		///		Check that the implicit assignment is now removed.
		/// </summary>
		[Test]
		public void UpdateKeyTermEquivalents_ChangesToImplicitlyAssignedOccurrence()
		{
			CheckDisposed();
			// establish "words" as a rendering for "words" keyterm.
			AssignVernacularEquivalentForWordsKeytermInMatthew1_1();
			// establish "conversation" as an alternative rendering for "words" keyterm.
			AssignAlternativeRenderingForWordsKeytermInMatthew();
			// Change Assigned keyterm rendering to an alternative rendering.
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = (StTxtPara)section.ContentOA.ParagraphsOS[0];

			// Case 3a: User unradically changes an implicit keyterm rendering
			// Modify verse 2 as follows:
			// "Verse two has even more stuff in it with words. And the word possible.""
			// "Verse two has even more stuff in it with conversation. And the word possible."
			TextSelInfo tsiBeforeEdit;
			TextSelInfo tsiAfterEdit;
			ReplaceVerseText(para, 40001002, "Verse two has even more stuff in it with conversation. And the word possible.",
				out tsiBeforeEdit, out tsiAfterEdit);
			IChkTerm keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			// Check for a new implicit rendering assignment
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.AutoAssigned,
				keyTerm_words.OccurrencesOS[1]);
			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[3]);

			// Case 3b: User radically changes an implicit keyterm rendering

			// Case 2b. User restores a keyterm rendering.
			// "Verse two has even more stuff in it with conversation. And the word possible."
			// "Verse two has even more stuff in it with sentences. And the word possible."
			ReplaceVerseText(para, 40001002, "Verse two has even more stuff in it with sentences. And the word possible.",
				out tsiBeforeEdit, out tsiAfterEdit);
			keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			//	Check that the implicit assignment is now removed.
			// check that everything else is still as we expect it
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[1]);
			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			CheckRenderingAndStatus("conversation", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[3]);
		}

		/// <summary>
		/// User changes the text to match a possible rendering, but has previously marked
		/// the status as Ignored.
		/// </summary>
		[Test]
		public void UpdateKeyTermEquivalents_ChangesToExplicitlyIgnoredOccurrence()
		{
			CheckDisposed();
			// establish "words" as a rendering for "words" keyterm.
			AssignVernacularEquivalentForWordsKeytermInMatthew1_1();
			// Explicitly Ignore assignment in verse 2.
			SelectTermAndOccurrence("words", 1);
			m_ktVwWrapper.IgnoreSpecifyingVernacularEquivalent();
			// Change Assigned keyterm rendering to an alternative rendering.
			IChkTerm keyTerm_words = SelectTerm("words");
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = (StTxtPara)section.ContentOA.ParagraphsOS[0];
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Ignored,
				keyTerm_words.OccurrencesOS[1]);

			// Case 2a: User radically changes the text containing the keyterm rendering
			// Modify verse 1 as follows:
			// "Verse two has even more stuff in it with words. And the word possible."
			// "Verse two has even more stuff in it with a sentence. And the word possible."
			TextSelInfo tsiBeforeEdit;
			TextSelInfo tsiAfterEdit;
			ReplaceVerseText(para, 40001002, "Verse two has even more stuff in it with a sentence. And the word possible.",
				out tsiBeforeEdit, out tsiAfterEdit);
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);
			// Check that a "Ignored" rendering is still "Ignored"
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Ignored,
				keyTerm_words.OccurrencesOS[1]);

			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
			// Case 2b. User restores a keyterm rendering.
			// "The beginning of Matthew has a sentence."
			// "The beginning of Matthew has some words."
			ReplaceVerseText(para, 40001002, "Verse two has even more stuff in it with words. And the word possible.",
				out tsiBeforeEdit, out tsiAfterEdit);
			keyTerm_words = SelectTerm("words");
			m_ktVwWrapper.UpdateKeyTermEquivalents(tsiBeforeEdit);

			// Check that a "Ignored" rendering is still "Ignored"
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Ignored,
				keyTerm_words.OccurrencesOS[1]);

			// check that everything else is still as we expect it
			CheckRenderingAndStatus("words", KeyTermRenderingStatus.Assigned,
				keyTerm_words.OccurrencesOS[0]);
			CheckRenderingAndStatus(null, KeyTermRenderingStatus.Unassigned,
				keyTerm_words.OccurrencesOS[2]);
		}

		/// <summary>
		/// Assigns "words" in the draft of Matthew 1:1 to be a vernacular equivalent for "words" keyterm.
		/// </summary>
		private void AssignVernacularEquivalentForWordsKeytermInMatthew1_1()
		{
			SelectWordsKeytermInMatthew1_1();

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();
		}

		private void SelectWordsKeytermInMatthew1_1()
		{
			// Select the word "words" in MAT 1:1 in the draft view
			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, 36, 41);

			// select the key term "words" and its first reference in the key terms view
			IChkTerm keyTerm1 = SelectTermAndOccurrence("words", 0);
		}

		/// <summary>
		/// Setup "conversation" as an alternative vernacular equivalent to "words"
		/// </summary>
		private void AssignAlternativeRenderingForWordsKeytermInMatthew()
		{
			IScrSection section = m_book.SectionsOS[0];
			StTxtPara para = (StTxtPara)section.ContentOA.ParagraphsOS[0];
			AppendRunWithAlternativeVernacularEquivalentForKeyTerm(para, "words", "conversation", "6", 40001006);
		}

		private void AppendRunWithAlternativeVernacularEquivalentForKeyTerm(StTxtPara para, string sKeyTerm, string alternative, string verseNumber, int bcv)
		{
			m_scrInMemoryCache.AddRunToMockedPara(para, verseNumber, ScrStyleNames.VerseNumber);
			int ichMinNewRun = para.Contents.Length;
			m_scrInMemoryCache.AddRunToMockedPara(para, String.Format("This verse has a {0}.", alternative), null);
			int ichMinAlternative = ichMinNewRun + "This verse has a ".Length;
			IScrSection section = (para.Owner as CmObject).Owner as IScrSection;
			section.AdjustReferences();
			IChkTerm keyTerm = SelectTerm(sKeyTerm);
			// Create references for the key term
			IChkRef crefTarget = CreateChkRef(keyTerm, bcv);
			// select the key term and its first reference in the key terms view
			// lookup the reference, load its renderings, select the occurrence in the renderings control
			// assume we're adding the new reference at the end of the current renderings.
			m_ktVwWrapper.RenderingsControl.LoadRenderingsForKeyTerm(keyTerm.Hvo, null);
			int iOccurrenceInRenderingControl = m_ktVwWrapper.RenderingsControl.ReferenceCount - 1;
			SetCurrentLineInRenderingCtrl(iOccurrenceInRenderingControl);

			m_draftView.TeEditingHelper.SelectRangeOfChars(0, 0, 0, ichMinAlternative, ichMinAlternative + alternative.Length);

			// assign it
			m_ktVwWrapper.AssignVernacularEquivalent();
		}

		private void ReplaceVerseText(StTxtPara para, int bcvToReplace, string replacement,
			out TextSelInfo tsiBeforeEdit, out TextSelInfo tsiAfterEdit)
		{
			(m_draftView.EditingHelper as TeEditingHelper).SelectVerseText((new ScrReference(bcvToReplace, Paratext.ScrVers.English)), null);
			tsiBeforeEdit = new TextSelInfo(m_draftView.RootBox);
			// para.Contents.Text.IndexOf("Verse three does not have the key term in it.")
			int ichMin = tsiBeforeEdit.IchAnchor;
			int ichLim = tsiBeforeEdit.IchEnd;
			m_scrInMemoryCache.ModifyRunAt(para, ichMin, ichLim, replacement);
			// not sure if this is necessary, since we aren't altering reference structure, just their locations.
			IScrSection section = (para.Owner as CmObject).Owner as IScrSection;
			section.AdjustReferences();
			(m_draftView.EditingHelper as TeEditingHelper).SelectVerseText((new ScrReference(bcvToReplace, Paratext.ScrVers.English)), null);
			tsiAfterEdit = new TextSelInfo(m_draftView.RootBox);
		}

		private static void CheckRenderingAndStatus(string expectedRendering, KeyTermRenderingStatus expectedStatus,
			IChkRef chkRef)
		{
			IWfiWordform wordform = chkRef.RenderingRA;
			if (expectedRendering == null)
				Assert.AreEqual(null, wordform);
			else
			{
				Assert.IsNotNull(wordform);
				Assert.AreEqual(expectedRendering, wordform.Form.VernacularDefaultWritingSystem);
			}
			Assert.AreEqual(expectedStatus, chkRef.Status);
		}

		#endregion UpdateKeyTermEquivalents (TE-4164)


		#endregion
	}
}
