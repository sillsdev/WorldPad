/*--------------------------------------------------------------------*//*:Ignore this sentence.
Copyright (C) 2003 SIL International. All rights reserved.

Distributable under the terms of either the Common Public License or the
GNU Lesser General Public License, as specified in the LICENSING.txt file.

File: TestVwSelection.h
Responsibility:
Last reviewed:

	Unit tests for the VwSelection class.
-------------------------------------------------------------------------------*//*:End Ignore*/
#ifndef TESTVWSELECTION_H_INCLUDED
#define TESTVWSELECTION_H_INCLUDED

#pragma once

#include "testViews.h"

namespace TestViews
{
#define khvoOrigPara1 998
#define khvoOrigPara2 999
#define khvoOrigPara3 997
#define khvoOrigPara4 996
#define khvoOrigPara5 995
#define khvoTransPara1 994

	class DummyVwTextSelection: public VwTextSelection
	{
	public:
		DummyVwTextSelection(VwParagraphBox * pvpbox, int ichAnchor, int ichEnd,
			bool fAssocPrevious) : VwTextSelection(pvpbox, ichAnchor, ichEnd, fAssocPrevious)
		{
		}

		Rect Bounds()
		{
			Rect tmp = m_rcBounds;
			m_rcBounds.Clear();
			return tmp;
		}
	};

	DEFINE_COM_PTR(DummyVwTextSelection);

	const int kflidWfics = 87;
	const int kflidWficForm = 88;
	const int kflidWficGloss = 89;
	const int kflidWficMorphs = 90;
	const int kflidMorphForm = 91;
	const int kflidMorphGloss = 92;
	const int kflidFt = 93;
	// Simple interlinear text.
	class DummyInterlinVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the paragraphs.
				pvwenv->AddObjVecItems(kflidStText_Paragraphs, this, 7);
				break;
			case 7: // StTxtPara, display wfics in a paragraph
				pvwenv->OpenParagraph();
				pvwenv->AddObjVecItems(kflidWfics, this, 8);
				pvwenv->CloseParagraph();
				pvwenv->AddStringProp(kflidFt, this);
				break;
			case 8: // Wfic, display form, morphs, gloss.
				pvwenv->OpenInnerPile();
				pvwenv->AddStringProp(kflidWficForm, NULL);
				pvwenv->OpenParagraph();
				pvwenv->AddObjVecItems(kflidWficMorphs, this, 9);
				pvwenv->CloseParagraph();
				pvwenv->AddStringProp(kflidWficGloss, NULL);
				pvwenv->CloseInnerPile();
				break;
			case 9: // morph, display form, gloss.
				pvwenv->OpenInnerPile();
				pvwenv->AddStringProp(kflidMorphForm, NULL);
				pvwenv->AddStringProp(kflidMorphGloss, NULL);
				pvwenv->CloseInnerPile();
				break;
			}
			return S_OK;
		}
	};

	// Simplified display of an StText: just show the paragraph contents.
	class SimpleStTextVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the paragraphs.
				pvwenv->AddObjVecItems(kflidStText_Paragraphs, this, 3);
				break;
			case 3: // StTxtPara, display contents
				pvwenv->AddStringProp(kflidStTxtPara_Contents, NULL);
				break;
			}
			return S_OK;
		}
	};

	// Simplified display of an StText: just show the paragraph contents in a table
	class TableStTextVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the paragraphs.
				pvwenv->AddObjVecItems(kflidStText_Paragraphs, this, 3);
				break;
			case 3: // StTxtPara, display contents
				{
					VwLength vlTable; // we use this to specify that the table takes 100% of the width.
					vlTable.nVal = 10000;
					vlTable.unit = kunPercent100;

					VwLength vlColumn;
					vlColumn.nVal = 10000;
					vlColumn.unit = kunPercent100;
					pvwenv->OpenTable(1, // One columns.
						vlTable, // Table uses 100% of available width.
						0, // Border thickness.
						kvaLeft, // Default alignment.
						kvfpVoid, // No border.
						kvrlNone,
						0, //No space between cells.
						0, //No padding inside cells.
						false); // multi-column select
					pvwenv->MakeColumns(1, vlColumn);
					pvwenv->OpenTableBody();
					pvwenv->OpenTableRow();

					// Display paragraph in the first cell
					pvwenv->OpenTableCell(1,1);
					pvwenv->AddStringProp(kflidStTxtPara_Contents, NULL);
					pvwenv->CloseTableCell();
					pvwenv->CloseTableRow();
					pvwenv->CloseTableBody();
					pvwenv->CloseTable();
					break;
				}
			case 4: // another root; display the paragraphs.
				pvwenv->AddObjVecItems(kflidStText_Paragraphs, this, 5);
				break;
			case 5: // StTxtPara, display contents in one column and BT in second column
				{
					VwLength vlTable; // we use this to specify that the table takes 100% of the width.
					vlTable.nVal = 10000;
					vlTable.unit = kunPercent100;

					VwLength vlColumn;
					vlColumn.nVal = 10000;
					vlColumn.unit = kunPercent100;
					pvwenv->OpenTable(2, // Two columns.
						vlTable, // Table uses 100% of available width.
						0, // Border thickness.
						kvaLeft, // Default alignment.
						kvfpVoid, // No border.
						kvrlNone,
						0, //No space between cells.
						0, //No padding inside cells.
						false); // multi-column select
					pvwenv->MakeColumns(1, vlColumn);
					pvwenv->OpenTableBody();
					pvwenv->OpenTableRow();

					// Display paragraph in the first cell
					pvwenv->OpenTableCell(1,1);
					pvwenv->AddStringProp(kflidStTxtPara_Contents, NULL);
					pvwenv->CloseTableCell();

					// Display BT in the second cell
					pvwenv->OpenTableCell(1,1);
					pvwenv->AddStringProp(kflidCmTranslation_Translation, NULL);
					pvwenv->CloseTableCell();

					pvwenv->CloseTableRow();
					pvwenv->CloseTableBody();
					pvwenv->CloseTable();
					break;
				}
			}
			return S_OK;
		}
	};

	// Display a literal
	class LitVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the literal in a paragraph.
				pvwenv->OpenParagraph();
				pvwenv->AddString(m_qtssLit1);
				pvwenv->CloseParagraph();
				break;

			case 2: // the root; display literals in two separate paragraphs.
				pvwenv->AddObj(hvo, this, 1);
				pvwenv->OpenParagraph();
				pvwenv->AddString(m_qtssLit2);
				pvwenv->CloseParagraph();
				break;
			}
			return S_OK;
		}
		LitVc(ITsString * ptssLit)
		{
			m_qtssLit1 = ptssLit;
		}
		LitVc(ITsString * ptssLit1, ITsString * ptssLit2)
		{
			m_qtssLit1 = ptssLit1;
			m_qtssLit2 = ptssLit2;
		}
		ITsStringPtr m_qtssLit1, m_qtssLit2;
	};

	class SimpleParaVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the literal in a paragraph with the style name supplied.
				pvwenv->put_StringProperty(ktptNamedStyle, m_stuParaStyleName.Bstr());
				pvwenv->OpenParagraph();
				pvwenv->AddString(m_qtssLit1);
				pvwenv->CloseParagraph();
				break;
			}
			return S_OK;
		}
		SimpleParaVc(ITsString * ptssLit, StrUni stuStyleName)
		{
			m_qtssLit1 = ptssLit;
			m_stuParaStyleName = stuStyleName;
		}
		ITsStringPtr m_qtssLit1, m_qtssLit2;
		StrUni m_stuParaStyleName;
	};


#define kflidProp1 800
#define kflidProp2 801
	// Display prop1, a literal, then prop2
	class StringLitStringVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the two properties sep by literal.
				pvwenv->OpenParagraph();
				pvwenv->AddStringProp(kflidProp1, NULL);
				pvwenv->AddString(m_qtssLit);
				pvwenv->AddStringProp(kflidProp2, NULL);
				pvwenv->CloseParagraph();
				break;

			}
			return S_OK;
		}
		StringLitStringVc(ITsString * ptssLit)
		{
			m_qtssLit = ptssLit;
		}
		ITsStringPtr m_qtssLit;
	};

	// Display a single paragraph made up of multiple strings.
	class ComplexParaVc : public DummyBaseVc
	{
	public:
		STDMETHOD(Display)(IVwEnv* pvwenv, HVO hvo, int frag)
		{
			switch(frag)
			{
			case 1: // the root; display the properties in order.
				pvwenv->OpenParagraph();
				for (int iflid = 0; iflid < m_cflid; ++iflid)
					pvwenv->AddStringProp(m_rgflid[iflid], NULL);
				pvwenv->CloseParagraph();
				break;

			}
			return S_OK;
		}
		ComplexParaVc(int * rgflid, int cflid)
		{
			m_rgflid = rgflid;
			m_cflid = cflid;
		}
		int * m_rgflid;
		int m_cflid;
	};

	class TestVwTextSelection : public unitpp::suite
	{
		VwTextSelectionPtr m_qzvwsel;
		IVwCacheDaPtr m_qcda;
		ISilDataAccessPtr m_qsda;
		ITsStrFactoryPtr m_qtsf;
		IVwViewConstructorPtr m_qvc;
		VwRootBoxPtr m_qrootb;
		IVwGraphicsWin32Ptr m_qvg32;
		HDC m_hdc;
		DummyRootSitePtr m_qdrs;
		Rect m_rcSrc;

		// Verify with unitpp::asserts that the run at ich in ptss is a pszText, with
		// the expected type and data.
		void VerifyOrcData(ITsString * ptss, int ich, const OLECHAR * pszText,
			const OLECHAR * pszData, OLECHAR chType)
		{
			int cchText = wcslen(pszText);
			ITsTextPropsPtr qttpOrc;
			TsRunInfo tri;
			ptss->FetchRunInfoAt(ich, &tri, &qttpOrc);
			unitpp::assert_eq("length of run", cchText, tri.ichLim - tri.ichMin);

			OLECHAR rgch[100]; // Big enough for any actual tests we currently do.
			ptss->FetchChars(ich, ich + cchText, rgch);
			unitpp::assert_true("found chars in string", wcsncmp(rgch, pszText, cchText) == 0);
			SmartBstr sbstrOrcProps;
			qttpOrc->GetStrPropValue(ktptObjData, &sbstrOrcProps);
			unitpp::assert_eq("got expected data type", chType, sbstrOrcProps.Chars()[0]);
			unitpp::assert_true("got expected object rep",
				wcscmp(sbstrOrcProps.Chars() + 1, pszData) == 0);
		}

		// Create the test view for testEmbeddedObjects() and testBoxInfo1().
		void CreateSimplestView()
		{
			ITsStringPtr qtss;
			// Create test data in a temporary cache.
			// First make some generic objects.
			// Now make one string, the contents of paragraph 1.
			StrUni stuPara1(L"This is the first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			// Insert a couple of ORCs with GUID properties.
			ITsStrBldrPtr qtsb;
			qtss->GetBldr(&qtsb);
			StrUni stuOrc(L"\xfffc");
			ITsPropsBldrPtr qtpb;
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->SetIntPropValues(ktptWs, ktpvDefault, g_wsEng);
			OLECHAR rgchProps[9];
			rgchProps[0] = kodtOwnNameGuidHot;
			memcpy(rgchProps + 1, &g_GuidForTextRepOfObj1, 16);
			StrUni stuProps1(rgchProps, 9);
			qtpb->SetStrPropValue(ktptObjData, stuProps1.Bstr());
			ITsTextPropsPtr qttpGuid1;
			qtpb->GetTextProps(&qttpGuid1);
			qtsb->Replace(5, 5, stuOrc.Bstr(), qttpGuid1);

			memcpy(rgchProps + 1, &g_GuidForTextRepOfObj2, 16);
			StrUni stuProps2(rgchProps, 9);
			qtpb->SetStrPropValue(ktptObjData, stuProps2.Bstr());
			ITsTextPropsPtr qttpGuid2;
			qtpb->GetTextProps(&qttpGuid2);
			qtsb->Replace(10, 10, stuOrc.Bstr(), qttpGuid2);

			qtsb->GetString(&qtss);

			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make it the paragraphs of an StText.
			HVO hvoRoot = 101;
			HVO hvoPara = khvoOrigPara1;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, &hvoPara, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
		}

		// Tests that clearing the typing properties clears the object data for
		// kodtOwnNameGuidHot type objects while keeping the other properties
		void testCleanPropertiesForTyping_kodtOwnNameGuidHot()
		{
			int ws;
			int var;
			CreateSimplestView();
			VwTextSelectionPtr qtsel;
			// create a dummy selection to do our testing on
			m_qrootb->MakeSimpleSel(true, true, false, true, (IVwSelection**)&qtsel);

			// Create some properties and set them on the selection
			ITsPropsBldrPtr qtpb;
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->SetIntPropValues(ktptWs, ktpvDefault, g_wsEng);
			OLECHAR rgchProps[9];
			rgchProps[0] = kodtOwnNameGuidHot;
			memcpy(rgchProps + 1, &g_GuidForTextRepOfObj1, 16);
			StrUni stuProps1(rgchProps, 9);
			qtpb->SetStrPropValue(ktptObjData, stuProps1.Bstr());
			ITsTextPropsPtr qttp;
			qtpb->GetTextProps(&qttp);
			qtsel->SetInsertionProps(qttp);

			// Test clearing obj data for kodtOwnNameGuidHot. This should clear the
			// obj properties and leave the WS properties
			qtsel->CleanPropertiesForTyping();
			SmartBstr sbstr;
			qtsel->m_qttp->GetStrPropValue(ktptObjData, &sbstr);
			unitpp::assert_true("Object data for a kodtOwnNameGuidHot should get cleared",
				sbstr == NULL);
			qtsel->m_qttp->GetIntPropValues(ktptWs, &var, &ws);
			unitpp::assert_eq("We should still have a writing system in the properties",
				g_wsEng, ws);
		}

		// Tests that clearing the typing properties does not clear the object data for
		// kodtExternalPathName type objects
		void testCleanPropertiesForTyping_kodtExternalPathName()
		{
			int ws;
			int var;
			CreateSimplestView();
			VwTextSelectionPtr qtsel;
			// create a dummy selection to do our testing on
			m_qrootb->MakeSimpleSel(true, true, false, true, (IVwSelection**)&qtsel);

			// Create some properties and set them on the selection
			ITsPropsBldrPtr qtpb;
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->SetIntPropValues(ktptWs, ktpvDefault, g_wsEng);
			OLECHAR rgchProps[9];
			rgchProps[0] = kodtExternalPathName;
			memcpy(rgchProps + 1, &g_GuidForTextRepOfObj1, 16);
			StrUni stuProps1(rgchProps, 9);
			qtpb->SetStrPropValue(ktptObjData, stuProps1.Bstr());
			ITsTextPropsPtr qttp;
			qtpb->GetTextProps(&qttp);
			qtsel->SetInsertionProps(qttp);

			// Test clearing obj data for kodtExternalPathName. This should not clear the
			// obj properties or the WS properties
			qtsel->CleanPropertiesForTyping();
			SmartBstr sbstr;
			qtsel->m_qttp->GetStrPropValue(ktptObjData, &sbstr);
			unitpp::assert_true("Object data for a kodtExternalPathName should not get cleared",
				sbstr.Equals(stuProps1));
			qtsel->m_qttp->GetIntPropValues(ktptWs, &var, &ws);
			unitpp::assert_eq("We should still have a writing system in the properties",
				g_wsEng, ws);
		}

		// Test that GetSelectionString detects ORC characters with kodtOwnNameGuidHot
		// and substitutes what TextRepOfObj returns. Also that ReplaceWithTsString
		// detects ORC with kodtEmbeddedObject and substitutes what MakeObjFromText
		// returns.
		void testEmbeddedObjects()
		{
			CreateSimplestView();

			IVwSelectionPtr qselTemp;
			// Select all of first paragraph
			m_qrootb->MakeSimpleSel(true, true, true, true, &qselTemp);
			// Get the selection string
			ITsStringPtr qtssClip;
			qselTemp->GetSelectionString(&qtssClip, L" ");
			VerifyOrcData(qtssClip, 5, L"XX", g_pszFakeObjTextRep1, kodtEmbeddedObjectData);
			VerifyOrcData(qtssClip, 11, L"XX", g_pszFakeObjTextRep2, kodtEmbeddedObjectData);
			// Now paste the string back in, at the start.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			qselTemp->ReplaceWithTsString(qtssClip);
			ITsStringPtr qtssResult;
			// make null-terminated copies of guids.
			StrUni stuGuid1((OLECHAR *)&g_GuidForMakeObjFromText1, 8);
			StrUni stuGuid2((OLECHAR *)&g_GuidForMakeObjFromText2, 8);
			ComBool f;
			m_qrootb->Selection()->Commit(&f); // So we can retrieve current text from cache.
			HVO hvoFirstPara;
			HVO hvoRoot = 101;
			m_qsda->get_VecItem(hvoRoot, kflidStText_Paragraphs, 0, &hvoFirstPara);
			m_qsda->get_StringProp(hvoFirstPara, kflidStTxtPara_Contents, &qtssResult);
			VerifyOrcData(qtssResult, 5, L"\xfffc", stuGuid1.Chars(), kodtOwnNameGuidHot);
			VerifyOrcData(qtssResult, 10, L"\xfffc", stuGuid2.Chars(), kodtOwnNameGuidHot);
		}

		// Tests that pasting at the beginning of a paragraph keeps the BT with the
		// original paragraph
		void testPasteAtBeginningOfPara()
		{
			ITsStringPtr qtss;
			// Create test data in a temporary cache.
			// First make some generic objects.
			// Now make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is the first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);
			// Attach a dummy translation to para 1
			m_qcda->CacheObjProp(khvoOrigPara1, kflidStTxtPara_Translations, khvoTransPara1);
			StrUni stuPara2(L"This is the second test paragraph");
			m_qtsf->MakeString(stuPara2.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara2, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[2] = {khvoOrigPara1, khvoOrigPara2};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 2);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);

			// Put insertion point at the beginning of the view
			IVwSelectionPtr qselTemp;
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			VwTextSelection pvwTextSel;

			// Simulate a paste
			StrUni stuPaste(L"This is the pasted Text\r\n");
			m_qtsf->MakeString(stuPaste.Bstr(), g_wsEng, &qtss);
			m_qzvwsel->ReplaceWithTsString(qtss);

			HVO hvoPara2;
			HVO trans2;
			m_qsda->get_VecItem(hvoRoot, kflidStText_Paragraphs, 1, &hvoPara2);
			unitpp::assert_true("didn't get a paragraph", hvoPara2);
			m_qsda->get_ObjectProp(hvoPara2, kflidStTxtPara_Translations, &trans2);

			unitpp::assert_eq("second paragraph should have the original translation",
				khvoTransPara1, trans2);
		}

		void DoPasteTest(OLECHAR * pszInput, OLECHAR * pszPaste, int ichSel, OLECHAR * pszResult)
		{
			HVO hvoRoot = 101;
			ITsStringPtr qtss;
			// Create test data in a temporary cache.
			StrUni stuPara1(pszInput);
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(hvoRoot, kflidStTxtPara_Contents, qtss);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStTxtPara2, NULL);

			// Put insertion point at the beginning of the view
			IVwSelectionPtr qselTemp;
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			m_qzvwsel->m_ichAnchor = m_qzvwsel->m_ichEnd = ichSel;

			// Simulate a paste
			StrUni stuPaste(pszPaste);
			m_qtsf->MakeString(stuPaste.Bstr(), g_wsEng, &qtss);
			m_qzvwsel->ReplaceWithTsString(qtss);
			ComBool fOk;
			m_qzvwsel->Commit(&fOk);

			ITsStringPtr qtssResult;
			m_qsda->get_StringProp(hvoRoot, kflidStTxtPara_Contents, &qtssResult);
			SmartBstr bstrResult;
			qtssResult->get_Text(&bstrResult);
			unitpp::assert_true( "result of paste", wcscmp(bstrResult.Chars(),
				pszResult) == 0);
		}

		// Tests pastes involving various kinds of newlines where destination is NOT an StText.
		// Newlines get converted to spaces.
		void testPasteNewlinesInNonText()
		{
			OLECHAR * pszInput = L"This is the input text";
			OLECHAR * pszPaste1 = L"This is the pasted Text\r\n";
			// Start, mid, end, with crlf at end of input.
			DoPasteTest(pszInput, pszPaste1, 0, L"This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste1, 4, L"ThisThis is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste1, wcslen(pszInput), L"This is the input textThis is the pasted Text");

			// Same with just cr
			OLECHAR * pszPaste2 = L"This is the pasted Text\r";
			DoPasteTest(pszInput, pszPaste2, 0, L"This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste2, 4, L"ThisThis is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste2, wcslen(pszInput), L"This is the input textThis is the pasted Text");

			// And with linefeed
			OLECHAR * pszPaste3 = L"This is the pasted Text\n";
			DoPasteTest(pszInput, pszPaste3, 0, L"This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste3, 4, L"ThisThis is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste3, wcslen(pszInput), L"This is the input textThis is the pasted Text");

			// Similar sequence, with newline in middle of pasted text. It gets turned into a space, so we get the exact same answers.
			OLECHAR * pszPaste4 = L"This is the\r\npasted Text";
			DoPasteTest(pszInput, pszPaste4, 0, L"This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste4, 4, L"ThisThis is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste4, wcslen(pszInput), L"This is the input textThis is the pasted Text");
			OLECHAR * pszPaste5 = L"This is the\rpasted Text";
			DoPasteTest(pszInput, pszPaste5, 0, L"This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste5, 4, L"ThisThis is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste5, wcslen(pszInput), L"This is the input textThis is the pasted Text");
			OLECHAR * pszPaste6 = L"This is the\npasted Text";
			DoPasteTest(pszInput, pszPaste6, 0, L"This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste6, 4, L"ThisThis is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste6, wcslen(pszInput), L"This is the input textThis is the pasted Text");

			// Similar sequence, with newline at start of pasted text. It gets turned into a space, so we get an extra space.
			OLECHAR * pszPaste7 = L"\r\nThis is the pasted Text";
			DoPasteTest(pszInput, pszPaste7, 0, L" This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste7, 4, L"This This is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste7, wcslen(pszInput), L"This is the input text This is the pasted Text");
			OLECHAR * pszPaste8 = L"\rThis is the pasted Text";
			DoPasteTest(pszInput, pszPaste8, 0, L" This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste8, 4, L"This This is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste8, wcslen(pszInput), L"This is the input text This is the pasted Text");
			OLECHAR * pszPaste9 = L"\nThis is the pasted Text";
			DoPasteTest(pszInput, pszPaste9, 0, L" This is the pasted TextThis is the input text");
			DoPasteTest(pszInput, pszPaste9, 4, L"This This is the pasted Text is the input text");
			DoPasteTest(pszInput, pszPaste9, wcslen(pszInput), L"This is the input text This is the pasted Text");

			// Try a few special cases with multiple newlines.
			OLECHAR * pszPaste10 = L"This\r\nis\rthe\npasted Text\r\n\n\n\r\r\r\n";
			DoPasteTest(pszInput, pszPaste10, 0, L"This is the pasted TextThis is the input text");
			OLECHAR * pszPaste11 = L"\n\rThis\r\n\r\nis\r\rthe\n\npasted\n\r\nText\n\r";
			DoPasteTest(pszInput, pszPaste11, 0, L"  This  is  the  pasted  TextThis is the input text");
		}


		void testTypeEnter()
		{
			ITsStringPtr qtss;
			// Create test data in a temporary cache.
			// First make some generic objects.
			// Now make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is the first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);
			// Attach a dummy translation to para 1
			m_qcda->CacheObjProp(khvoOrigPara1, kflidStTxtPara_Translations, khvoTransPara1);
			StrUni stuPara2(L"This is the second test paragraph");
			m_qtsf->MakeString(stuPara2.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara2, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[2] = {khvoOrigPara1, khvoOrigPara2};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 2);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);

			//VwPropertyStorePtr qvwps = NewObj VwPropertyStore;
			wchar chw = (wchar)13;
			int encPending = -1;	// bogus
			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			VwTextSelection pvwTextSel;

			// Simulate an Enter key being pressed
			m_qzvwsel->OnTyping(m_qvg32, &chw, 1, 0, 0, chw, &encPending);
			int chvoPara;
			m_qsda->get_VecSize(hvoRoot, kflidStText_Paragraphs, &chvoPara);
			unitpp::assert_true("Should have three paragraphs now", chvoPara == 3);
			// Check that we are in the second paragraph now.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at the beginning of (new) second para", ich == 0);
			unitpp::assert_true("New Para 2 should be the original para 1",
				hvoPara == khvoOrigPara1);
			HVO hvoTranslation;
			m_qsda->get_ObjectProp(hvoPara, kflidStTxtPara_Translations, &hvoTranslation);
			unitpp::assert_true("CmTranslation should still belong to original para 1",
				hvoTranslation == khvoTransPara1);
			int cchw;
			const OLECHAR * pwrgch;
			qtss->LockText(&pwrgch, &cchw);
			unitpp::assert_true("New para should not be empty", cchw > 0);
			unitpp::assert_true(
				"New second para should contain contents of original first para",
				wcscmp(pwrgch, stuPara1.Chars()) == 0);
			qtss->UnlockText(pwrgch);

			// Move to the first para and check that it isn't either of the original ones
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			qtss->get_Length(&cchw);
			unitpp::assert_true("New (first) para should be empty", cchw == 0);
			unitpp::assert_true("New (first) para should have a different hvo from the originals",
				hvoPara != khvoOrigPara1 && hvoPara != khvoOrigPara2);

			// Move to the last para and check that it is still 2
			m_qrootb->MakeSimpleSel(false, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Last para's HVO should not have changed",
				hvoPara == khvoOrigPara2);
		}

		// This also tests some of the arrow key behavior.
		void testTypeShiftEnter()
		{
			ITsStringPtr qtss;
			// Now make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is the first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);
			StrUni stuPara2(L"This is the second test paragraph");
			m_qtsf->MakeString(stuPara2.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara2, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[2] = {khvoOrigPara1, khvoOrigPara2};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 2);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testTypeShiftEnter Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr);
			unitpp::assert_true("PrepareToDraw succeeded", hr == S_OK);

			wchar chw = kchwHardLineBreak;
			int encPending = -1;	// bogus
			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view, then move it into the middle of
			// the first paragraph.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = 10;
			m_qzvwsel->m_ichAnchor = 10;

			// Simulate a Shift-Enter key being pressed TWICE!
			m_qzvwsel->OnTyping(m_qvg32, &chw, 1, 0, 0, chw, &encPending);
			m_qzvwsel->OnTyping(m_qvg32, &chw, 1, 0, 0, chw, &encPending);
			int chvoPara;
			m_qsda->get_VecSize(hvoRoot, kflidStText_Paragraphs, &chvoPara);
			unitpp::assert_true("Should have two paragraphs still", chvoPara == 2);

			// Check that we are in the first paragraph still.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			// TODO: Maybe this test could be more explicit.
			unitpp::assert_true(
				"Should be at the beginning of the third line of the first para",
				ich == 12);
			unitpp::assert_true("First para's HVO should not have changed",
				hvoPara == khvoOrigPara1);
			int cchw;
			const OLECHAR * pwrgch;
			qtss->LockText(&pwrgch, &cchw);
			unitpp::assert_true("Modified para should not be empty", cchw > 0);
			StrUni stuMod = stuPara1;
			StrUni stuHardLineBreakChar(&chw, 1);
			stuMod.Replace(10, 10, stuHardLineBreakChar);
			stuMod.Replace(10, 10, stuHardLineBreakChar);
			int nDiff = wcscmp(pwrgch, stuMod.Chars());
			qtss->UnlockText(pwrgch); // do this before the assert
			unitpp::assert_true(
				"Should have prepended two U+2028's to contents of original first para.",
				nDiff == 0);

			// Attempt to go up a line. This should change the selection to be on the blank
			// line of the para.
			hr = m_qrootb->OnExtendedKey(kecUpArrowKey, kfssNone, 0);
			unitpp::assert_eq("OnExtendedKey(kecUpArrowKey...) did not succeed", S_OK, hr);
			m_qrootb->get_Selection((IVwSelection**)&m_qzvwsel);
			unitpp::assert_true("Non-null text selection after OnExtendedKey",
				dynamic_cast<VwTextSelection *>(m_qzvwsel.Ptr()));
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_eq("Should still be in first para", khvoOrigPara1, hvoPara);
			unitpp::assert_eq("Should be on the second (blank) line of the first para",
				11, ich);

			// Attempt to go up one more line. This should change selection to be at the
			// beginning of the para.
			hr = m_qrootb->OnExtendedKey(kecUpArrowKey, kfssNone, 0);
			unitpp::assert_eq("OnExtendedKey(kecUpArrowKey...) did not succeed", S_OK, hr);

			m_qrootb->get_Selection((IVwSelection**)&m_qzvwsel);
			unitpp::assert_true("Non-null text selection after OnExtendedKey",
				dynamic_cast<VwTextSelection *>(m_qzvwsel.Ptr()));
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_eq("Should still be in first para [2]", khvoOrigPara1, hvoPara);
			unitpp::assert_eq("Should be at the beginning of the first line of the first para",
				0, ich);

			// Move the selection close to the hard line breaks, and then start stepping
			// forward, backward, and sideways.  Well, maybe not sideways...
			m_qzvwsel->m_ichEnd = 9;
			m_qzvwsel->m_ichAnchor = 9;
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 10", 10, m_qzvwsel->m_ichEnd);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 11", 11, m_qzvwsel->m_ichEnd);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 12", 12, m_qzvwsel->m_ichEnd);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 13", 13, m_qzvwsel->m_ichEnd);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 14", 14, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 13", 13, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 12", 12, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 11", 11, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 10", 10, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 9", 9, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 8", 8, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 7", 7, m_qzvwsel->m_ichEnd);

			// Some more tests while we're here to check paragraph boundary handling.
			VwParagraphBox * pvpboxOrig = m_qzvwsel->m_pvpbox;
			m_qzvwsel->m_ichEnd = cchw - 2;
			m_qzvwsel->m_ichAnchor = cchw - 2;
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal cchw - 1",
				cchw - 1, m_qzvwsel->m_ichEnd);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal cchw",
				cchw, m_qzvwsel->m_ichEnd);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 0",
				0, m_qzvwsel->m_ichEnd);
			unitpp::assert_true("forward to the second paragraph",
				pvpboxOrig != m_qzvwsel->m_pvpbox);
			m_qzvwsel->ForwardArrow(m_qvg32);
			unitpp::assert_eq("forward: ichEnd should equal 1",
				1, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal 0",
				0, m_qzvwsel->m_ichEnd);
			m_qzvwsel->BackwardArrow(m_qvg32);
			unitpp::assert_eq("backward: ichEnd should equal cchw",
				cchw, m_qzvwsel->m_ichEnd);
			unitpp::assert_true("back to the first paragraph",
				pvpboxOrig == m_qzvwsel->m_pvpbox);

			// Test backspacing over hard line breaks.
			m_qzvwsel->m_ichEnd = 12;
			m_qzvwsel->m_ichAnchor = 12;
			const OLECHAR chBack = kscBackspace;
			int wsPending = g_wsFrn;
			m_qzvwsel->OnTyping(m_qvg32, NULL, 0, 1, 0, chBack, &wsPending);
			unitpp::assert_eq("backspace: ichEnd should equal 11", 11, m_qzvwsel->m_ichEnd);
		}

		//
		void TypeCtrlBackspaceWorker(int ichBegin, int ichAfter, StrUni sTextAfter,
			bool fBackspace)
		{
			ITsStringPtr qtss;
			// Now make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is a test. test two");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlBackspace Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlBackspace PrepareToDraw succeeded", hr == S_OK);


			int encPending = -1;	// bogus
			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view, then move it into the middle of
			// the first paragraph.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = ichBegin;
			m_qzvwsel->m_ichAnchor = ichBegin;

			// Simulate a Ctrl-Backspace

			if (fBackspace)
			{
				wchar chw = static_cast<wchar>(8); // 8 is a backspace in ASCII
				m_qzvwsel->OnTyping(m_qvg32, &chw, 1, -1, 0, chw, &encPending);
			}
			else // Delete character
			{
				wchar chw = static_cast<wchar>(127); // 127 is a delete in ASCII
				m_qzvwsel->OnTyping(m_qvg32, &chw, 1, 0, -1, chw, &encPending);
			}

			int chvoPara;
			m_qsda->get_VecSize(hvoRoot, kflidStText_Paragraphs, &chvoPara);
			unitpp::assert_true("Should have a paragraph still", chvoPara == 1);

			// Check that we are in the first paragraph still.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			int cchw;
			const OLECHAR * pwrgch;
			qtss->LockText(&pwrgch, &cchw);
			unitpp::assert_true("Modified para should not be empty", cchw > 0);

			int nDiff = wcscmp(pwrgch, sTextAfter.Chars());
			qtss->UnlockText(pwrgch); // do this before the assert
			unitpp::assert_true("Deletion result is not correct", nDiff == 0);

			unitpp::assert_true("Should be at the end of the first word", ich == ichAfter);
		}

		/***************************************************************************************
			Next several tests use 'true' to test Ctrl+backspace and 'false' for Ctrl+delete
		***************************************************************************************/

		// Tests the ctrl+backspace behavior at the end of a word
		void testTypeCtrlBackspace_EndOfWord()
		{
			TypeCtrlBackspaceWorker(7, 4, "This a test. test two", true);
		}

		// Tests the ctrl+backspace behavior at the beginning of a word
		void testTypeCtrlBackspace_BeginningOfWord()
		{
			TypeCtrlBackspaceWorker(8, 5, "This a test. test two", true);
		}

		// Tests the ctrl+backspace behavior in the middle of a word
		void testTypeCtrlBackspace_MiddleOfWord()
		{
			TypeCtrlBackspaceWorker(6, 5, "This s a test. test two", true);
		}

		// Tests the ctrl+backspace after a period
		void testTypeCtrlBackspace_AfterPunct()
		{
			TypeCtrlBackspaceWorker(15, 14, "This is a test test two", true);
		}

		// Tests the ctrl+backspace before a period
		void testTypeCtrlBackspace_BeforePunct()
		{
			TypeCtrlBackspaceWorker(14, 9, "This is a. test two", true);
		}

		// Tests the ctrl+backspace just after the first word. It's debatable whether we should delete the space, too.
		void testTypeCtrlBackspace_AfterFirstWord()
		{
			TypeCtrlBackspaceWorker(4, 0, " is a test. test two", true);
		}
		// Tests the ctrl+backspace at the end of the whole paragraph. Makes sure we don't have problems
		// trying to get properties of following character, and also that we can skip the preceding
		// space without a following character.
		void testTypeCtrlBackspace_AtEnd()
		{
			int len = strlen("This is a test. test two");
			TypeCtrlBackspaceWorker(len, len - 4, "This is a test. test", true);
		}

		// Tests the ctrl+delete behavior in the end of a word
		void testTypeCtrlDelete_EndOfWord()
		{
			TypeCtrlBackspaceWorker(7, 7, "This is test. test two", false);
		}

		// Tests the ctrl+delete behavior in the beginning of a word
		void testTypeCtrlDelete_BeginningOfWord()
		{
			TypeCtrlBackspaceWorker(5, 5, "This a test. test two", false);
		}

		// Tests the ctrl+delete behavior in the middle of a word
		void testTypeCtrlDelete_MiddleOfWord()
		{
			TypeCtrlBackspaceWorker(6, 6, "This i a test. test two", false);
		}

		// Tests the ctrl+delete behavior after a period
		void testTypeCtrlDelete_AfterPunct()
		{
			TypeCtrlBackspaceWorker(15, 15, "This is a test. two", false);
		}

		// Tests the ctrl+delete behavior before a period
		void testTypeCtrlDelete_BeforePunct()
		{
			TypeCtrlBackspaceWorker(14, 14, "This is a test test two", false);
		}

		// Tests the ctrl+backspace behavior when a run ends
		void testTypeCtrlBackspace_WithinRun()
		{
			ITsStringPtr qtss;
			ITsStrBldrPtr qtsb;
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;
			// create run props
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->SetIntPropValues(ktptWs, ktpvDefault, g_wsFrn);
			qtpb->GetTextProps(&pttp);
			// create string
			StrUni stuPara1(L"This is a test. test two");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			qtss->GetBldr(&qtsb);
			int bldrLength;
			qtsb->get_Length(&bldrLength);
			qtsb->ReplaceRgch(bldrLength, bldrLength, L"100", 3, pttp);
			qtsb->GetString(&qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlBackspace Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlBackspace PrepareToDraw succeeded", hr == S_OK);

			int encPending = -1;	// bogus
			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view, then move it into the middle of
			// the first paragraph.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = 26;
			m_qzvwsel->m_ichAnchor = 26;

			// Simulate a Ctrl-Backspace
			wchar chw = static_cast<wchar>(8); // 8 is a backspace in ASCII
			m_qzvwsel->OnTyping(m_qvg32, &chw, 1, -1, 0, chw, &encPending);

			int chvoPara;
			m_qsda->get_VecSize(hvoRoot, kflidStText_Paragraphs, &chvoPara);
			unitpp::assert_true("Should have a paragraph still", chvoPara == 1);

			// Check that we are in the first paragraph still.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			int cchw;
			const OLECHAR * pwrgch;
			qtss->LockText(&pwrgch, &cchw);
			unitpp::assert_true("Modified para should not be empty", cchw > 0);

			int nDiff = wcscmp(pwrgch, L"This is a test. test two0");
			qtss->UnlockText(pwrgch); // do this before the assert
			unitpp::assert_true("Deletion result is not correct", nDiff == 0);

			unitpp::assert_true("Should be at the end of the first word", ich == 24);
		}

		// Tests the ctrl+delete behavior when a run ends
		void testTypeCtrlDelete_WithinRun()
		{
			ITsStringPtr qtss;
			ITsStrBldr * qtsb;
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;
			// create run props
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->SetIntPropValues(ktptWs, ktpvDefault, g_wsFrn);
			qtpb->GetTextProps(&pttp);
			// create string
			StrUni stuPara1(L"This is a test. test two");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			qtss->GetBldr(&qtsb);
			qtsb->ReplaceRgch(0, 0, L"100", 3, pttp);
			qtsb->GetString(&qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlBackspace Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlBackspace PrepareToDraw succeeded", hr == S_OK);

			int encPending = -1;	// bogus
			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view, then move it one character into
			// the first paragraph (in the first run).
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = 1;
			m_qzvwsel->m_ichAnchor = 1;

			// Simulate a Ctrl-Delete
			wchar chw = static_cast<wchar>(127); // 127 is a delete in ASCII
			m_qzvwsel->OnTyping(m_qvg32, &chw, 1, 0, -1, chw, &encPending);

			int chvoPara;
			m_qsda->get_VecSize(hvoRoot, kflidStText_Paragraphs, &chvoPara);
			unitpp::assert_true("Should have a paragraph still", chvoPara == 1);

			// Check that we are in the first paragraph still.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			int cchw;
			const OLECHAR * pwrgch;
			qtss->LockText(&pwrgch, &cchw);
			unitpp::assert_true("Modified para should not be empty", cchw > 0);

			int nDiff = wcscmp(pwrgch, L"1This is a test. test two");
			qtss->UnlockText(pwrgch); // do this before the assert
			unitpp::assert_true("Deletion result is not correct", nDiff == 0);

			unitpp::assert_true("Should be at the end of the first word", ich == 1);
		}

		// Tests the ctrl+right arrow behavior when a word has two writing systems.
		void testTypeCtrlRightArrow_TwoWS()
		{
			ITsStringPtr qtss1;
			ITsStringPtr qtss2;
			ITsStrBldr * ptsb;

			// create string with two writing systems
			StrUni stuWs1(L"WsOne");
			StrUni stuWs2(L"WsTwo");
			m_qtsf->MakeString(stuWs1.Bstr(), g_wsEng, &qtss1);
			m_qtsf->MakeString(stuWs2.Bstr(), g_wsFrn, &qtss2);
			int tss1Length;
			qtss1->get_Length(&tss1Length);
			qtss1->GetBldr(&ptsb);
			ptsb->ReplaceTsString(tss1Length, tss1Length, qtss2);
			ptsb->GetString(&qtss1);
			qtss1->get_Length(&tss1Length);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1);

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlRightArrow_TwoWS Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlRightArrow_TwoWS PrepareToDraw succeeded", hr == S_OK);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);

			// Simulate a Ctrl-Right Arrow
			m_qzvwsel->ControlForwardArrow(m_qvg32);

			// Check that we are at the writing system break.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss1, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at writing system break", ich == 5);
		}

		// Tests the ctrl+right arrow behavior when two words have two writing systems.
		// The first writing system continues over the space between the two words.
		void testTypeCtrlRightArrow_ThreeWordsTwoWS_a()
		{
			ITsStringPtr qtss1;
			ITsStringPtr qtss2;
			ITsStrBldr * ptsb;

			// create string with two writing systems
			StrUni stuWs1(L"WsOne ");
			StrUni stuWs2(L"WsTwo next word");
			m_qtsf->MakeString(stuWs1.Bstr(), g_wsEng, &qtss1);
			m_qtsf->MakeString(stuWs2.Bstr(), g_wsFrn, &qtss2);
			int tss1Length;
			qtss1->get_Length(&tss1Length);
			qtss1->GetBldr(&ptsb);
			ptsb->ReplaceTsString(tss1Length, tss1Length, qtss2);
			ptsb->GetString(&qtss1);
			qtss1->get_Length(&tss1Length);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1);

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlRightArrow_ThreeWordsTwoWS_a Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlRightArrow_ThreeWordsTwoWS_a PrepareToDraw succeeded", hr == S_OK);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);

			// Simulate a Ctrl-Right Arrow
			m_qzvwsel->ControlForwardArrow(m_qvg32);

			// Check that we are at start of the second word.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss1, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at writing system break", ich == 6);
		}

		// Tests the ctrl+right arrow behavior when two words have two writing systems.
		// The first writing system ends just before the space between the two words.
		void testTypeCtrlRightArrow_TwoWordsTwoWS_b()
		{
			ITsStringPtr qtss1;
			ITsStringPtr qtss2;
			ITsStrBldr * ptsb;

			// create string with two writing systems
			StrUni stuWs1(L"WsOne");
			StrUni stuWs2(L" WsTwo");
			m_qtsf->MakeString(stuWs1.Bstr(), g_wsEng, &qtss1);
			m_qtsf->MakeString(stuWs2.Bstr(), g_wsFrn, &qtss2);
			int tss1Length;
			qtss1->get_Length(&tss1Length);
			qtss1->GetBldr(&ptsb);
			ptsb->ReplaceTsString(tss1Length, tss1Length, qtss2);
			ptsb->GetString(&qtss1);
			qtss1->get_Length(&tss1Length);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1);

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlRightArrow_TwoWordsTwoWS_b Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlRightArrow_TwoWordsTwoWS_b PrepareToDraw succeeded", hr == S_OK);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);

			// Simulate a Ctrl-Right Arrow
			m_qzvwsel->ControlForwardArrow(m_qvg32);

			// Check that we are at the start of the second word.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss1, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at writing system break", ich == 6);
		}

		// Tests the ctrl+left arrow behavior when a word has two writing systems.
		void testTypeCtrlLeftArrow_TwoWS()
		{
			ITsStringPtr qtss1;
			ITsStringPtr qtss2;
			ITsStrBldr * ptsb;

			// create string with two writing systems
			StrUni stuWs1(L"WsOne");
			StrUni stuWs2(L"WsTwo");
			m_qtsf->MakeString(stuWs1.Bstr(), g_wsEng, &qtss1);
			m_qtsf->MakeString(stuWs2.Bstr(), g_wsFrn, &qtss2);
			int tss1Length;
			qtss1->get_Length(&tss1Length);
			qtss1->GetBldr(&ptsb);
			ptsb->ReplaceTsString(tss1Length, tss1Length, qtss2);
			ptsb->GetString(&qtss1);
			qtss1->get_Length(&tss1Length);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1);

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlLeftArrow_TwoWS Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlLeftArrow_TwoWS PrepareToDraw succeeded", hr == S_OK);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the end of the view.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = tss1Length;
			m_qzvwsel->m_ichAnchor = tss1Length;

			// Simulate a Ctrl-Left Arrow
			m_qzvwsel->ControlBackwardArrow(m_qvg32);

			// Check that we are at the writing system break.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss1, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at writing system break", ich == 5);
		}

		// Tests the ctrl+left arrow behavior when a word has two writing systems.
		// The first writing system continues over the space between the two words.
		void testTypeCtrlLeftArrow_TwoWordTwoWS_a()
		{
			ITsStringPtr qtss1;
			ITsStringPtr qtss2;
			ITsStrBldr * ptsb;

			// create string with two writing systems
			StrUni stuWs1(L"WsOne ");
			StrUni stuWs2(L"WsTwo");
			m_qtsf->MakeString(stuWs1.Bstr(), g_wsEng, &qtss1);
			m_qtsf->MakeString(stuWs2.Bstr(), g_wsFrn, &qtss2);
			int tss1Length;
			qtss1->get_Length(&tss1Length);
			qtss1->GetBldr(&ptsb);
			ptsb->ReplaceTsString(tss1Length, tss1Length, qtss2);
			ptsb->GetString(&qtss1);
			qtss1->get_Length(&tss1Length);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1);

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlLeftArrow_TwoWordTwoWS_a Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlLeftArrow_TwoWordTwoWS_a PrepareToDraw succeeded", hr == S_OK);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the end of the view.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = tss1Length;
			m_qzvwsel->m_ichAnchor = tss1Length;

			// Simulate a Ctrl-Left Arrow
			m_qzvwsel->ControlBackwardArrow(m_qvg32);

			// Check that we are at the start of the second word.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss1, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at writing system break", ich == 6);
		}

		// Tests the ctrl+left arrow behavior when a word has two writing systems.
		// The first writing system ends just before the space between the two words.
		void testTypeCtrlLeftArrow_TwoWordTwoWS_b()
		{
			ITsStringPtr qtss1;
			ITsStringPtr qtss2;
			ITsStrBldr * ptsb;

			// create string with two writing systems
			StrUni stuWs1(L"WsOne");
			StrUni stuWs2(L" WsTwo");
			m_qtsf->MakeString(stuWs1.Bstr(), g_wsEng, &qtss1);
			m_qtsf->MakeString(stuWs2.Bstr(), g_wsFrn, &qtss2);
			int tss1Length;
			qtss1->get_Length(&tss1Length);
			qtss1->GetBldr(&ptsb);
			ptsb->ReplaceTsString(tss1Length, tss1Length, qtss2);
			ptsb->GetString(&qtss1);
			qtss1->get_Length(&tss1Length);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1);

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr;
			CheckHr(hr = m_qrootb->Layout(m_qvg32, 300));
			unitpp::assert_eq("testTypeCtrlLeftArrow_TwoWordTwoWS_b Layout succeeded", S_OK, hr);
			VwPrepDrawResult xpdr;
			CheckHr(hr = m_qrootb->PrepareToDraw(m_qvg32, m_rcSrc, m_rcSrc, &xpdr));
			unitpp::assert_true("testTypeCtrlLeftArrow_TwoWordTwoWS_b PrepareToDraw succeeded", hr == S_OK);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the end of the view.
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			unitpp::assert_eq("ichEnd == 0", 0, m_qzvwsel->m_ichEnd);
			unitpp::assert_eq("ichAnchor == 0", 0, m_qzvwsel->m_ichAnchor);
			m_qzvwsel->m_ichEnd = tss1Length;
			m_qzvwsel->m_ichAnchor = tss1Length;

			// Simulate a Ctrl-Left Arrow
			m_qzvwsel->ControlBackwardArrow(m_qvg32);

			// Check that we are at the start of the second word.
			int ich;
			ComBool fAssocPrev;
			PropTag tag;
			int ws;
			HVO hvoPara;
			m_qzvwsel->TextSelInfo(false, &qtss1, &ich, &fAssocPrev, &hvoPara, &tag, &ws);
			unitpp::assert_true("Should be at writing system break", ich == 6);
		}

		// Produce a new TsString from the old one.
		void SetTextProp(ITsString * ptss, int ichMin, int ichLim, int tpt, int ttv, int val,
			ITsString ** pptss)
		{
			ITsStrBldrPtr qtsb;
			ptss->GetBldr(&qtsb);
			qtsb->SetIntPropValues(ichMin, ichLim, tpt, ttv, val);
			qtsb->GetString(pptss);
		}

		void VerifyFontSize(ITsTextProps * pttp, int valExpected, char * pchMsg)
		{
			int var, val;
			HRESULT hr = pttp->GetIntPropValues(ktptFontSize, &var, &val);
			unitpp::assert_eq("GetIntPropValues succeeded", S_OK, hr);
			StrAnsi staMsg;
			staMsg.Format("wrong variation for font size %s", pchMsg);
			unitpp::assert_eq(staMsg.Chars(), ktpvMilliPoint, var);
			staMsg.Format("wrong value for font size %s", pchMsg);
			unitpp::assert_eq(staMsg.Chars(), valExpected, val);
		}

		/*--------------------------------------------------------------------------------------
			Make the specified selection. Return it if ppsel is non-NULL.
		--------------------------------------------------------------------------------------*/
		void MakeSelection(int ihvoAnchor, int ichAnchor, int ichEnd, bool fAssocPrev,
			bool fInstall, VwTextSelection ** ppsel, int ihvoEnd = -1)
		{
			if (ihvoEnd == ihvoAnchor)
				ihvoEnd = -1;
			VwSelLevInfo vsli;
			vsli.tag = kflidStText_Paragraphs;
			vsli.cpropPrevious = 0; // first occurrence of that property
			vsli.ihvo = ihvoAnchor;
			IVwSelectionPtr qsel;
			m_qrootb->MakeTextSelection(0, // first top-level target
				1, // VwSelLevInfo object
				&vsli,
				kflidStTxtPara_Contents,
				0, // first occurrence of contents.
				ichAnchor, ichEnd,
				0, // ws doesn't matter, not multilingual.
				fAssocPrev,
				ihvoEnd, // end in same paragraph.
				NULL, // not overriding default props to type.
				fInstall, // go ahead and install it if true.
				ppsel ? &qsel : NULL);
			if (ppsel)
			{
				// convert to VwTextSelection
				*ppsel = dynamic_cast<VwTextSelection *>(qsel.Detach());
			}
		}


		// Actually only designed to test some aspects of AllSelEndInfo, to do with recent
		// changes to get it to return text props even for a range.
		void testAllSelEndInfo()
		{
			ITsStringPtr qtss;
			ITsStringPtr qtssT;
			// Make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is the first test paragraph");
			int cch1 = stuPara1.Length();
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			SetTextProp(qtss, 0, 7, ktptFontSize, ktpvMilliPoint, 20000, &qtssT);
			SetTextProp(qtssT, cch1-3, cch1, ktptFontSize, ktpvMilliPoint, 30000, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);
			StrUni stuPara2(L"This is the second test paragraph");
			m_qtsf->MakeString(stuPara2.Bstr(), g_wsEng, &qtss);
			int cch2 = stuPara2.Length();
			SetTextProp(qtss, 0, 7, ktptFontSize, ktpvMilliPoint, 40000, &qtssT);
			SetTextProp(qtssT, cch2-3, cch2, ktptFontSize, ktpvMilliPoint, 50000, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara2, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[2] = {khvoOrigPara1, khvoOrigPara2};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 2);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testAllSelEndInfo Layout succeeded", S_OK, hr);

			IVwSelectionPtr qselTemp;
			// Put insertion point at the beginning of the view
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);

			// Call AllSelEndInfo
			int ihvoRoot;
			VwSelLevInfo rgvsli[1];
			PropTag tagTextProp;
			int cpropPrevious;
			int ich;
			int ws;
			ComBool fAssocPrev;
			ITsTextPropsPtr qttpIns;
			hr = qselTemp->AllSelEndInfo(true, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked for IP at start(end)", S_OK, hr);
			VerifyFontSize(qttpIns, 20000, "IP at start (end)");
			hr = qselTemp->AllSelEndInfo(false, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked for IP at start(begin)", S_OK, hr);
			VerifyFontSize(qttpIns, 20000, "IP at start (begin)");

			m_qrootb->MakeSimpleSel(false, true, false, true, &qselTemp);
			hr = qselTemp->AllSelEndInfo(true, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked for IP at end(end)", S_OK, hr);
			VerifyFontSize(qttpIns, 50000, "IP at end (end)");
			hr = qselTemp->AllSelEndInfo(false, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked for IP at start(begin)", S_OK, hr);
			VerifyFontSize(qttpIns, 50000, "IP at end (begin)");

			// Select all the first paragraph.
			VwTextSelectionPtr qselRange;
			MakeSelection(0, 0, cch1, true, false, &qselRange);
			hr = qselRange->AllSelEndInfo(true, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked end of first para", S_OK, hr);
			VerifyFontSize(qttpIns, 30000, "end of first para");
			hr = qselRange->AllSelEndInfo(false, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked start of first para", S_OK, hr);
			VerifyFontSize(qttpIns, 20000, "start of first para");

			// Select from near start of second para backwards into first.
			MakeSelection(1, 2, cch1-2, true, false, &qselRange, 0);
			hr = qselRange->AllSelEndInfo(true, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked 2nd back to first", S_OK, hr);
			VerifyFontSize(qttpIns, 30000, "2nd back to first end");
			hr = qselRange->AllSelEndInfo(false, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked start of first para", S_OK, hr);
			VerifyFontSize(qttpIns, 40000, "2nd back to first begin");

			// Pathological selection from end of first to start of second
			MakeSelection(0, cch1, 0, true, false, &qselRange, 1);
			hr = qselRange->AllSelEndInfo(true, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked pathological", S_OK, hr);
			// We currently get a null result, because there's no selected text.
			// I'm not sure that's the best answer, so don't verify it.
			//VerifyFontSize(qttpIns, 40000, "pathological end");
			hr = qselRange->AllSelEndInfo(false, &ihvoRoot,
				1, rgvsli, &tagTextProp, &cpropPrevious,
				&ich, &ws, &fAssocPrev, &qttpIns);
			unitpp::assert_eq("AllSelEndInfo worked pathological begin", S_OK, hr);
			//VerifyFontSize(qttpIns, 30000, "pathological begin");
		}

		void VerifyParaContents(int ipara, const OLECHAR * pch)
		{
			VwBox * pbox = m_qrootb->FirstRealBox();
			for (int i = 0; i < ipara; i++)
			{
				pbox = pbox->NextRealBox();
			}
			VwParagraphBox * pvpbox = dynamic_cast<VwParagraphBox *>(pbox);
			int cch;
			pvpbox->Source()->get_Length(&cch);
			OLECHAR buf[1000]; // enough for any tests in this module
			Assert(cch < 1000);
			pvpbox->Source()->Fetch(0, cch, buf);
			buf[cch] = 0;
			StrAnsi staMsg;
			staMsg.Format("para contents is \n\"%S\"\n instead of \n\"%S\"", buf, pch);
			unitpp::assert_true(staMsg.Chars(), wcscmp(pch, buf) == 0);
		}

		// Create test view for testOnProblemDeletion() and testBoxInfo3().
#define kcSection4 4
		HRESULT CreateBkSecPara()
		{
			// Create test data. We simulate a book with 4 sections containing 1 to 4 paragraphs
			// each.
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcSection4];
			HVO rghvoSec[kcSection4];
			int ipara = 0;
			for (int isec = 0; isec < kcSection4; isec++)
			{
				for (int i = 0; i < isec + 1; i++)
				{
					stuPara.Format(L"This is paragraph %d", ipara++);
					m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
					m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
					rghvoPara[i] = hvoPara;
					hvoPara++;
				}
				m_qcda->CacheVecProp(isec + khvoSecMin, kflidParas, rghvoPara, isec + 1);
				rghvoSec[isec] = isec + khvoSecMin;
			}
			m_qcda->CacheVecProp(khvoBook, kflidSections, rghvoSec, kcSection4);

			m_qvc.Attach(NewObj DummyVcBkSecPara());
			m_qrootb->SetRootObject(khvoBook, m_qvc, kfragBook, NULL);
			return m_qrootb->Layout(m_qvg32, 300);
		}

		void testOnProblemDeletion()
		{
			HRESULT hr = CreateBkSecPara();
			unitpp::assert_eq("testOnProblemDeletion Layout succeeded", S_OK, hr);
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwParagraphBox * pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				5, 3, false, pvpbox2));
			m_qrootb->SetSelection(qsel, false);
			StrUni stuKey(L"b");
			int wspend = -1;
			// Default: replace the selected part of the anchor string, but our routine should
			// be called.
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 0, L'b', &wspend);
			unitpp::assert_eq("First OnTyping failed", S_OK, hr);
			unitpp::assert_eq("First OnTyping problem type", kdptComplexRange,
				m_qdrs->GetAndResetProblemType());
			VerifyParaContents(0, L"This b");
			VerifyParaContents(1, L"This is paragraph 1");

			m_qdrs->SetProbDeleteAction(1); // Typing aborted!
			VwParagraphBox * pvpbox3 = dynamic_cast<VwParagraphBox *>(pvpbox2->NextRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox2,
				5, 3, false, pvpbox3));
			m_qrootb->SetSelection(qsel, false);
			// Root site will abort.
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 0, L'b',	&wspend);
			unitpp::assert_eq("2nd OnTyping failed", S_OK, hr);
			VerifyParaContents(1, L"This is paragraph 1");
			VerifyParaContents(2, L"This is paragraph 2");

			m_qdrs->SetProbDeleteAction(2); // Selection is bizarrely moved to end of doc
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 0, L'b',	&wspend);
			unitpp::assert_eq("3rd OnTyping failed", S_OK, hr);
			VerifyParaContents(1, L"This is paragraph 1");
			VerifyParaContents(2, L"This is paragraph 2");
			// The selection was moved to the end of the last paragraph and the "b" inserted.
			VerifyParaContents(9, L"This is paragraph 9b");

			// Make a selection at the very start of the view and abort backspace.
			m_qdrs->SetProbDeleteAction(1); // Typing aborted!
			m_qrootb->MakeSimpleSel(true, true, false, true, NULL);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("4th OnTyping failed", S_OK, hr);
			unitpp::assert_eq("4th OnTyping problem type", kdptBsAtStartPara,
				m_qdrs->GetAndResetProblemType());
			VerifyParaContents(0, L"This b");

			// Make a selection at the very start of the view and fail backspace, continue with
			// 'b'.
			m_qdrs->SetProbDeleteAction(6); // backspace failed!
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("5th OnTyping failed", S_OK, hr);
			unitpp::assert_eq("5th OnTyping problem type", kdptBsAtStartPara,
				m_qdrs->GetAndResetProblemType());
			VerifyParaContents(0, L"bThis b");

			// Make a selection at the very end of the view and abort del.
			m_qdrs->SetProbDeleteAction(1); // Typing aborted!
			hr = m_qrootb->MakeSimpleSel(false, true, false, true, NULL);
			unitpp::assert_eq("Make sel at end failed", S_OK, hr);
			stuKey = L"c";
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("6th OnTyping failed", S_OK, hr);
			unitpp::assert_eq("6th OnTyping problem type", kdptDelAtEndPara,
				m_qdrs->GetAndResetProblemType());
			VerifyParaContents(9, L"This is paragraph 9b");

			// Make a selection at the very end of the view and fail del, continue with 'c'.
			m_qdrs->SetProbDeleteAction(6); // del failed!
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("7th OnTyping failed", S_OK, hr);
			unitpp::assert_eq("7th OnTyping problem type", kdptDelAtEndPara,
				m_qdrs->GetAndResetProblemType());
			VerifyParaContents(9, L"This is paragraph 9bc");
			// Todo JohnT/EberhardB: more tests for backspace and delete cases.
			// Test for readonly case.
		}

		void CreateTestDataForMultiCharDeletetions()
		{
			// Create test data. We simulate a book with 4 sections containing 1 to 4 paragraphs
			// each.
			ITsStringPtr qtss;
			StrUni stuPara;
			int flidSentence = kflidProp1;
			int rgflidPara[1];
			stuPara.Format(L"This is a" COMBINING_DIAERESIS COMBINING_MACRON L" "
					// These are 2 chars each.
					MUSICAL_SYMBOL_SEMIBREVIS_WHITE MUSICAL_SYMBOL_COMBINING_STEM
					L" sentence %d.", 1);
			m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoBook, flidSentence, qtss);
			rgflidPara[0] = flidSentence;
			++flidSentence;

			m_qvc.Attach(NewObj ComplexParaVc(rgflidPara, 1));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("Layout failed", S_OK, hr);
			//VerifyParaContents(0,
			//	L"This is a" COMBINING_DIAERESIS COMBINING_MACRON L" "
			//	MUSICAL_SYMBOL_SEMIBREVIS_WHITE MUSICAL_SYMBOL_COMBINING_STEM L" sentence 1.");
		}

		void testMultipleCharDeletions_First20Chars()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();

			// Delete the first 20 chars of the paragraph.
			int wspend = -1;
			m_qrootb->MakeSimpleSel(true, true, false, true, NULL);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 20, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 0, 20, NUL,...) failed", S_OK, hr);
			VerifyParaContents(0, L" 1.");
		}

		void testMultipleCharDeletions_BackspaceLast20Chars()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();

			int wspend = -1;
			// Delete (backspace over) the last 20 chars of the paragraph.
			m_qrootb->MakeSimpleSel(false, true, false, true, NULL);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 20, 0, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 20, 0, NUL, ...) failed", S_OK, hr);
			VerifyParaContents(0, L"This i");
		}

		void testMultipleCharDeletions_DelEndsInSurrogate()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();
			int wspend = -1;

			// Delete 11 characters (which will end up in the middle of a surrogate pair)
			m_qrootb->MakeSimpleSel(true, true, false, true, NULL);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 11, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 0, 11, NUL,...) [2] failed", S_OK, hr);
			VerifyParaContents(0, L" sentence 1.");
		}

		void testMultipleCharDeletions_BackspaceAtBeginning()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();
			int wspend = -1;

			// Try backspacing at the beginning of the paragraph.
			m_qrootb->MakeSimpleSel(true, true, false, true, NULL);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 2, 0, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 2, 0, NUL,...) [2] failed", S_OK, hr);
			VerifyParaContents(0,
				L"This is a" COMBINING_DIAERESIS COMBINING_MACRON L" "
				MUSICAL_SYMBOL_SEMIBREVIS_WHITE MUSICAL_SYMBOL_COMBINING_STEM L" sentence 1.");
		}

		void testMultipleCharDeletions_BackspaceEndsInSurrogate()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();

			int wspend;
			// Delete (backspace over) the last 13 chars of the doc.
			m_qrootb->MakeSimpleSel(false, true, false, true, NULL);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 13, 0, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 13, 0, NUL,...) [3] failed", S_OK, hr);
			VerifyParaContents(0,
				L"This is a" COMBINING_DIAERESIS COMBINING_MACRON L" "
				MUSICAL_SYMBOL_SEMIBREVIS_WHITE);
		}

		void testMultipleCharDeletions_DeleteAtEnd()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();
			int wspend = -1;

			// Try deleting at the end of the paragraph.
			m_qrootb->MakeSimpleSel(false, true, false, true, NULL);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 2, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 0, 2, NUL,...) [4] failed", S_OK, hr);
			VerifyParaContents(0,
				L"This is a" COMBINING_DIAERESIS COMBINING_MACRON L" "
				MUSICAL_SYMBOL_SEMIBREVIS_WHITE MUSICAL_SYMBOL_COMBINING_STEM L" sentence 1.");
		}

		void testMultipleCharDeletions_BackspaceAndDeleteInMiddle()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();
			int wspend = -1;

			// Try backspacing and deleting from the middle of the paragraph to the beginning.
			VwParagraphBox * pvpbox = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox, 12, 12, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 5, 2, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 5, 2, NUL,...) [5] failed", S_OK, hr);
			VerifyParaContents(0, L"This issentence 1.");
		}

		void testMultipleCharDeletions_BackspaceAndDeleteInMiddleMore()
		{
			// More tests for backspace and delete cases.
			CreateTestDataForMultiCharDeletetions();
			int wspend = -1;

			// Try deleting and backspacing from the middle of the paragraph to the end.
			// 7 puts it just after 'This is': two for the semibrevis (surrogate pair),
			// one each for space, a, special, special, space. These 7 wchars count as only 4
			// for del forward: space, a-with-two-marks, space, semibrevis.
			VwParagraphBox * pvpbox = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox, 7, 7, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			HRESULT hr = m_qrootb->OnTyping(m_qvg32, NULL, 2, 4, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 2, 4, NUL,...) [5] failed", S_OK, hr);
			VerifyParaContents(0, L"This  sentence 1.");
		}

		// Tests pressing the delete key when the IP is in front of the base character (TE-6382)
		void testDeleteInFrontOfBaseCharacter()
		{
			// Create test data
			ITsStringPtr qtss;
			ITsStringPtr qtssT;
			// Make a string which will be the content of our paragraph.
			StrUni stuPara1(L"This is a" COMBINING_DIAERESIS COMBINING_MACRON
					L" first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("Layout failed", S_OK, hr);
			int wspend = -1;

			// Delete in front of the first "a"
			MakeSelection(0, 8, 8, true, true, NULL);
			hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 1, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 0, 1, \\0, ...) failed", S_OK, hr);
			VerifyParaContents(0, L"This is  first test paragraph");
		}

		// Tests pressing the delete key when the IP is between the base character and the
		// diacritic (TE-6382)
		void testDeleteInFrontOfDiacritic()
		{
			// Create test data
			ITsStringPtr qtss;
			ITsStringPtr qtssT;
			// Make a string which will be the content of our paragraph.
			StrUni stuPara1(L"This is a" COMBINING_DIAERESIS COMBINING_MACRON
					L" first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("Layout failed", S_OK, hr);
			int wspend = -1;

			// Delete after the first "a"
			IVwSelectionPtr qsel;
			MakeSelection(0, 9, 9, true, true, NULL);
			hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 1, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 0, 1, \\0, ...) failed", S_OK, hr);
			VerifyParaContents(0, L"This is a first test paragraph");
		}

		// Tests pressing the delete key when the IP is between two diacritics (TE-6382)
		void testDeleteBetweenDiacritics()
		{
			// Create test data
			ITsStringPtr qtss;
			ITsStringPtr qtssT;
			// Make a string which will be the content of our paragraph.
			StrUni stuPara1(L"This is a" COMBINING_DIAERESIS COMBINING_MACRON
					L" first test paragraph");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);

			// Now make them the paragraphs of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("Layout failed", S_OK, hr);
			int wspend = -1;

			// Delete after the first diacritic
			IVwSelectionPtr qsel;
			MakeSelection(0, 10, 10, true, true, NULL);
			hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 1, L'\0', &wspend);
			unitpp::assert_eq("OnTyping(..., NULL, 0, 1, \\0, ...) failed", S_OK, hr);
			VerifyParaContents(0, L"This is a" COMBINING_DIAERESIS L" first test paragraph");
		}

		// Create test view for testStTextEditing() and testBoxInfo2().
#define kcparaStText 4
		HRESULT CreateSimpleStText()
		{
			// Create test data. We simulate a book with 4 sections containing 1 to 4 paragraphs
			// each.
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcparaStText];
			for (int ipara = 0; ipara < kcparaStText; ipara++)
			{
				stuPara.Format(L"This is paragraph %d", ipara);
				m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
				rghvoPara[ipara] = hvoPara;
				hvoPara++;
			}
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, kcparaStText);

			// For this test, support the interface that gives us advance notification.
			m_qdrs->m_fSupportAboutToDelete = true;

			m_qvc.Attach(NewObj SimpleStTextVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			return m_qrootb->Layout(m_qvg32, 300);
		}

		void testStTextEditing()
		{
			HRESULT hr = CreateSimpleStText();
			unitpp::assert_eq("testStTextEditing Layout succeeded", S_OK, hr);
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwParagraphBox * pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox1, 5, 4, false, pvpbox2));
			m_qrootb->SetSelection(qsel, false);
			StrUni stuKey(L"b");
			int wspend = -1;
			// Default: replace the range by merging the paragraphs; OnProblemDeletion should
			// not be called.
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 0, L'b', &wspend);
			unitpp::assert_eq("First OnTyping failed", S_OK, hr);
			unitpp::assert_eq("First OnTyping problem type", -1,
				m_qdrs->GetAndResetProblemType());
			StrUni stuPara1b(L"This b is paragraph 1");
			VerifyParaContents(0, stuPara1b.Chars());
			m_qdrs->VerifyAboutToDelete(m_qrootb, khvoParaMin + 1, khvoBook,
				kflidStText_Paragraphs, 1, false);

			// Try typing a delete at the end of the line, followed by a b.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				stuPara1b.Length(), stuPara1b.Length(), false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("Second OnTyping failed", S_OK, hr);
			unitpp::assert_eq("Second OnTyping problem type", -1,
				m_qdrs->GetAndResetProblemType());
			StrUni stuPara1c(L"This b is paragraph 1bThis is paragraph 2");
			VerifyParaContents(0, stuPara1c.Chars());
			// This time we destroyed the third original paragraph (but, at the time, it was at
			// index 1, because we deleted the old second paragraph earlier).
			m_qdrs->VerifyAboutToDelete(m_qrootb, khvoParaMin + 2, khvoBook,
				kflidStText_Paragraphs, 1, false);

			// Try typing a backspace at the beginning of the line, followed by a b.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox2,
				0, 0, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("Third OnTyping failed", S_OK, hr);
			unitpp::assert_eq("Third OnTyping problem type", -1,
				m_qdrs->GetAndResetProblemType());
			StrUni stuPara1d(L"This b is paragraph 1bThis is paragraph 2bThis is paragraph 3");
			VerifyParaContents(0, stuPara1d.Chars());
			// Clobbered the third paragraph.
			m_qdrs->VerifyAboutToDelete(m_qrootb, khvoParaMin + 3, khvoBook,
				kflidStText_Paragraphs, 1, false);
		}

		void testDelObjNotificationsRange()
		{
			// Create test data. We simulate a book with 4 sections containing 1 to 4 paragraphs
			// each.
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcparaStText];
			for (int ipara = 0; ipara < kcparaStText; ipara++)
			{
				stuPara.Format(L"This is paragraph %d", ipara);
				m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
				rghvoPara[ipara] = hvoPara;
				hvoPara++;
			}
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, kcparaStText);

			// For this test, support the interface that gives us advance notification.
			m_qdrs->m_fSupportAboutToDelete = true;

			m_qvc.Attach(NewObj SimpleStTextVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testStTextEditing Layout succeeded", S_OK, hr);
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwParagraphBox * pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			VwParagraphBox * pvpbox3 = dynamic_cast<VwParagraphBox *>(pvpbox2->NextRealBox());
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				5, 4, false, pvpbox3));
			m_qrootb->SetSelection(qsel, false);
			StrUni stuKey(L"b");
			int wspend = -1;
			// Replace the range by merging the paragraphs.
			// Clear in case some other test already run on same site.
			m_qdrs->m_vhvoDeletedItems.Clear();
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 0, L'b', &wspend);
			// Paragraphs 1 and 2 deleted, 1 last.
			m_qdrs->VerifyAboutToDelete(m_qrootb, khvoParaMin + 1, khvoBook,
				kflidStText_Paragraphs, 1, false);
			unitpp::assert_eq("did not get two notifications",
				2, m_qdrs->m_vhvoDeletedItems.Size());
			unitpp::assert_eq("wrong first object deleted",
				khvoParaMin + 2, m_qdrs->m_vhvoDeletedItems[0]);
		}

		// yalola      nihimbilira
		// yalo la     ni himbi li   ra
		// mat	my     I  see   pres sing
		// my.mat      I.am.seeing
		// I see my green mat

		// yalola      nihimbilira
		// yalo la     ni himbi li   ra
		// mat	my     I  see   pres sing
		// my.mat      I.am.seeing
		// I see my green mat

#define khvoWficMin 2500
#define khvoMorphMin 3500


		// create a sequence of citem dummy items with hvos starting at hvoFirst (max 10).
		// make it the value of the property kflidItems of hvoParent.
		// set property kflidStrings of each item to the corresponding string in prgpszStrings
		void SetStrings(int hvoParent, OLECHAR ** prgpszStrings, int citem, int kflidStrings, int kflidItems, int hvoFirst)
		{
			HVO rghvoItem[10];
			ITsStringPtr qtss;

			for (int i = 0; i < citem; i++)
			{
				HVO hvo = hvoFirst + i;
				rghvoItem[i] = hvo;
				m_qtsf->MakeStringRgch(prgpszStrings[i], wcslen(prgpszStrings[i]), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvo, kflidStrings, qtss);
			}
			m_qcda->CacheVecProp(hvoParent, kflidItems, rghvoItem, citem);
		}

		void AssertEqual(char * pszMsg, const OLECHAR * pszExpected, const OLECHAR * pszActual)
		{
			if (wcscmp(pszExpected, pszActual) == 0)
				return;
			StrAnsi msg;
			msg.Format("%s: expected <%S> but got <%S>", pszMsg, pszExpected, pszActual);
			unitpp::assert_true(msg.Chars(), false);
		}

		void testInterlinClipboard()
		{
			// Make a phony interlinear text.
			ITsStringPtr qtss;
			StrUni stuPara;
			HVO rghvoPara[2];
			// We'll have two identical paragraphs
			rghvoPara[0] = rghvoPara[1] = khvoParaMin;
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, 2);

			OLECHAR * pszFt = L"I see my green mat";
			m_qtsf->MakeStringRgch(pszFt, wcslen(pszFt), g_wsEng, &qtss);
			m_qcda->CacheStringProp(khvoParaMin, kflidFt, qtss);

			OLECHAR * rgpszWords[] = {L"yalola", L"nihimbilira"};
			OLECHAR * rgpszWordGlosses[] = {L"my.mat", L"I.am.seeing"};
			OLECHAR * rgpszYalolaMorphs[] = {L"yalo", L"la"};
			OLECHAR * rgpszNihimMorphs[] = {L"ni", L"himbi", L"li", L"ra"};
			OLECHAR * rgpszYalolaGlosses[] = {L"mat", L"my"};
			OLECHAR * rgpszNihimGlosses[] = {L"I", L"see", L"pres", L"sing"};


			HVO rghvoWfic[2];

			for (int iwfic = 0; iwfic < 2; iwfic++)
			{
				HVO hvoWfic = khvoWficMin + iwfic;
				rghvoWfic[iwfic] = hvoWfic;
				m_qtsf->MakeStringRgch(rgpszWords[iwfic], wcslen(rgpszWords[iwfic]), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoWfic, kflidWficForm, qtss);
				m_qtsf->MakeStringRgch(rgpszWordGlosses[iwfic],wcslen(rgpszWordGlosses[iwfic]), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoWfic, kflidWficGloss, qtss);
			}
			m_qcda->CacheVecProp(khvoParaMin, kflidWfics, rghvoWfic, 2);

			SetStrings(rghvoWfic[0], rgpszYalolaMorphs, 2, kflidMorphForm, kflidWficMorphs, khvoMorphMin);
			SetStrings(rghvoWfic[0], rgpszYalolaGlosses, 2, kflidMorphGloss, kflidWficMorphs, khvoMorphMin);
			SetStrings(rghvoWfic[1], rgpszNihimMorphs, 4, kflidMorphForm, kflidWficMorphs, khvoMorphMin + 3);
			SetStrings(rghvoWfic[1], rgpszNihimGlosses, 4, kflidMorphGloss, kflidWficMorphs, khvoMorphMin + 3);

			m_qvc.Attach(NewObj DummyInterlinVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			CheckHr(m_qrootb->Layout(m_qvg32, 300));

			// Select within a single item.
			VwParagraphBox * pvpboxFirstMain = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstBox());
			VwInnerPileBox * pipboxCol1 = dynamic_cast<VwInnerPileBox *>(pvpboxFirstMain->FirstBox());
			VwParagraphBox * pvpboxLine1Col1 = dynamic_cast<VwParagraphBox *>(pipboxCol1->FirstBox());
			VwTextSelectionPtr qsel1;
			qsel1.Attach(NewObj VwTextSelection(pvpboxLine1Col1, 1, 4, false, NULL));
			StrUni sep(L" ");
			CheckHr(qsel1->GetSelectionString(&qtss,  sep.Bstr()));
			SmartBstr sbstr;
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for part of first word", L"alo", sbstr.Chars());

			// Select across multiple paragraphs in the first word.
			VwParagraphBox * pvpboxLine3Col1 = dynamic_cast<VwParagraphBox *>(pvpboxLine1Col1->Next()->Next());
			VwTextSelectionPtr qsel2;
			IVwSelectionPtr qselRange;
			qsel1.Attach(NewObj VwTextSelection(pvpboxLine1Col1, 0, 0, false, NULL));
			qsel2.Attach(NewObj VwTextSelection(pvpboxLine3Col1, 0, 0, false, NULL));
			m_qrootb->MakeRangeSelection(qsel1, qsel2, true, &qselRange);

			// Get the selected text.
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for first bundle", L"yalola\t\r\nyalo\tla\r\nmat\tmy\r\nmy.mat\t\r\n", sbstr.Chars());

			// Should get the same result; trying reverse order and mid-string.
			qsel1.Attach(NewObj VwTextSelection(pvpboxLine3Col1, 2, 2, false, NULL));
			qsel2.Attach(NewObj VwTextSelection(pvpboxLine1Col1, 3, 3, false, NULL));
			m_qrootb->MakeRangeSelection(qsel1, qsel2, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for first bundle reversed", L"yalola\t\r\nyalo\tla\r\nmat\tmy\r\nmy.mat\t\r\n", sbstr.Chars());

			// Now try two bundles
			VwInnerPileBox * pipboxCol2 = dynamic_cast<VwInnerPileBox *>(pipboxCol1->NextOrLazy());
			VwParagraphBox * pvpboxLine1Col2 = dynamic_cast<VwParagraphBox *>(pipboxCol2->FirstBox());
			VwTextSelectionPtr qselLine1Col2;
			qselLine1Col2.Attach(NewObj VwTextSelection(pvpboxLine1Col2, 3, 3, false, NULL));
			m_qrootb->MakeRangeSelection(qsel1, qselLine1Col2, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			// yalola      nihimbilira
			// yalo la     ni himbi li   ra
			// mat	my     I  see   pres sing
			// my.mat      I.am.seeing
			AssertEqual("wrong string for first two bundles",
				L"yalola\t\tnihimbilira\t\t\t\r\n"
				L"yalo\tla\tni\thimbi\tli\tra\r\n"
				L"mat\tmy\tI\tsee\tpres\tsing\r\n"
				L"my.mat\t\tI.am.seeing\t\t\t\r\n",
				sbstr.Chars());

			// Again, try it the other way around.
			m_qrootb->MakeRangeSelection(qselLine1Col2, qsel1, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for first two bundles reversed",
				L"yalola\t\tnihimbilira\t\t\t\r\n"
				L"yalo\tla\tni\thimbi\tli\tra\r\n"
				L"mat\tmy\tI\tsee\tpres\tsing\r\n"
				L"my.mat\t\tI.am.seeing\t\t\t\r\n",
				sbstr.Chars());

			// Now try a selection extending into the FT paragraph from the second wordform. (reusing qselLine1Col2)
			VwParagraphBox * pvpboxFirstFt = dynamic_cast<VwParagraphBox *>(pvpboxFirstMain->NextOrLazy());
			VwTextSelectionPtr qselFt;
			qselFt.Attach(NewObj VwTextSelection(pvpboxFirstFt, 6, 6, false, NULL));
			m_qrootb->MakeRangeSelection(qselLine1Col2, qselFt, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for second bundle and part of FT",
				L"nihimbilira\t\t\t\r\n"
				L"ni\thimbi\tli\tra\r\n"
				L"I\tsee\tpres\tsing\r\n"
				L"I.am.seeing\t\t\t\r\n"
				L"I see ",
				sbstr.Chars());

			// Now from the FT into the second paragraph. Also a chance to try a more deeply nested end point.
			VwParagraphBox * pvpboxThirdMain = dynamic_cast<VwParagraphBox *>(pvpboxFirstFt->NextOrLazy());
			VwInnerPileBox * pipboxCol1P3 = dynamic_cast<VwInnerPileBox *>(pvpboxThirdMain->FirstBox());
			VwParagraphBox * pvpboxLine1Col1P3 = dynamic_cast<VwParagraphBox *>(pipboxCol1P3->FirstBox());
			// This is the containing para for the morphs in the third (second interlinear) paragraph, first pile.
			VwParagraphBox * pvpboxLine2Col1P3Morphs = dynamic_cast<VwParagraphBox *>(pvpboxLine1Col1P3->NextOrLazy());
			VwInnerPileBox * pipboxCol1P3Morph1 = dynamic_cast<VwInnerPileBox *>(pvpboxLine2Col1P3Morphs->FirstBox());
			VwParagraphBox * pvpboxLine2Col1P3 = dynamic_cast<VwParagraphBox *>(pipboxCol1P3Morph1->FirstBox());
			VwTextSelectionPtr qselLine2Col1P3;
			qselLine2Col1P3.Attach(NewObj VwTextSelection(pvpboxLine2Col1P3, 4, 4, false, NULL));
			m_qrootb->MakeRangeSelection(qselFt, qselLine2Col1P3, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for second half of FT and following bundle",
				L"my green mat\r\n"
				L"yalola\t\r\n"
				L"yalo\tla\r\n"
				L"mat\tmy\r\n"
				L"my.mat\t\r\n",
				sbstr.Chars());

			// Including this one to indicate what I intend to happen when the selection is entirely within a single
			// morpheme group. It is conceivable in this case we could just output the morpheme bundles. But for
			// now it is deliberately programmed to output the whole top-level bundle.
			VwParagraphBox * pvpboxLine3Col1P3 = dynamic_cast<VwParagraphBox *>(pvpboxLine2Col1P3->NextOrLazy());
			VwTextSelectionPtr qselLine3Col1P3;
			qselLine3Col1P3.Attach(NewObj VwTextSelection(pvpboxLine3Col1P3, 0, 0, false, NULL));
			m_qrootb->MakeRangeSelection(qselLine2Col1P3, qselLine3Col1P3, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for single bundle from morph selection",
				L"yalola\t\r\n"
				L"yalo\tla\r\n"
				L"mat\tmy\r\n"
				L"my.mat\t\r\n",
				sbstr.Chars());

			// Now try with both ends in an FT.
			VwParagraphBox * pvpboxSecondFt = dynamic_cast<VwParagraphBox *>(pvpboxThirdMain->NextOrLazy());
			VwTextSelectionPtr qselSecondFt;
			qselSecondFt.Attach(NewObj VwTextSelection(pvpboxSecondFt, 8, 8, false, NULL));
			m_qrootb->MakeRangeSelection(qselFt, qselSecondFt, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for part of first FT through part of second",
				L"my green mat\r\n"
				L"yalola\t\tnihimbilira\t\t\t\r\n"
				L"yalo\tla\tni\thimbi\tli\tra\r\n"
				L"mat\tmy\tI\tsee\tpres\tsing\r\n"
				L"my.mat\t\tI.am.seeing\t\t\t\r\n"
				L"I see my",
				sbstr.Chars());

			// And finally with both ends in interlinear, backwards.
			m_qrootb->MakeRangeSelection(qselLine2Col1P3, qselLine1Col2, true, &qselRange);
			CheckHr(qselRange->GetSelectionString(&qtss,  sep.Bstr()));
			qtss->get_Text(&sbstr);
			AssertEqual("wrong string for part of first FT through part of second",
				L"nihimbilira\t\t\t\r\n"
				L"ni\thimbi\tli\tra\r\n"
				L"I\tsee\tpres\tsing\r\n"
				L"I.am.seeing\t\t\t\r\n"
				L"I see my green mat\r\n"
				L"yalola\t\r\n"
				L"yalo\tla\r\n"
				L"mat\tmy\r\n"
				L"my.mat\t\r\n",
				sbstr.Chars());


		}


		// Can't test this anymore since it is done with a ProblemDeletion now. Tests for that
		// are now in TeDllTests.
		// void testMergeParasInTablesOneColumn()


		// Tests getting the paragraph properties from a multiparagraph selection when the
		// paragraphs are contained in a table
		void testGetParaPropsInTables()
		{
			// Create test data. We simulate a book with 4 sections containing 1 to 4 paragraphs
			// each. Paragraphs 0 and 2 are empty
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcparaStText];
			for (int ipara = 0; ipara < kcparaStText; ipara++)
			{
				stuPara.Format(L"This is paragraph %d", ipara);
				if (ipara == 0 || ipara == 2)
					stuPara = L"";
				m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
				rghvoPara[ipara] = hvoPara;
				hvoPara++;
			}
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, kcparaStText);

			m_qvc.Attach(NewObj TableStTextVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testMergeParasInTables Layout didn't succeed", S_OK, hr);

			VwParagraphBox * pvpbox2 = FindParaBoxInsideOf(
				dynamic_cast<VwGroupBox *>(m_qrootb->FirstRealBox()->NextRealBox()));
			VwParagraphBox * pvpbox3 = FindParaBoxInsideOf(
				dynamic_cast<VwGroupBox *>(m_qrootb->FirstRealBox()->NextRealBox()->NextRealBox()));
			// Sel at start of second para to the start of the third para.
			VwTextSelectionPtr qsel1;
			VwTextSelectionPtr qsel2;
			IVwSelectionPtr qselRange;
			qsel1.Attach(NewObj VwTextSelection(pvpbox2, 0, 0, false, NULL));
			qsel2.Attach(NewObj VwTextSelection(pvpbox3, 0, 0, false, NULL));
			m_qrootb->MakeRangeSelection(qsel1, qsel2, true, &qselRange);

			int cttp;
			qselRange->GetParaProps(0, NULL, &cttp);
			unitpp::assert_eq("Wrong number of paragraph properties found", 2, cttp);
		}

		VwParagraphBox * FindParaBoxInsideOf(VwGroupBox * pgrpBox)
		{
			VwParagraphBox * pvpbox = dynamic_cast<VwParagraphBox *>(pgrpBox);
			if (pvpbox)
				return pvpbox;
			while (pgrpBox && pgrpBox->FirstBox())
			{
				pvpbox = dynamic_cast<VwParagraphBox *>(pgrpBox->FirstBox());
				if (pvpbox)
					return pvpbox;
				pgrpBox = dynamic_cast<VwGroupBox *>(pgrpBox->FirstBox());
			}
			return NULL;
		}

		void testMergeEmptyParas()
		{
			// Create test data. We simulate a book with 4 paragraphs each. Paragraphs 0 and 2
			// are empty.
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcparaStText];
			for (int ipara = 0; ipara < kcparaStText; ipara++)
			{
				stuPara.Format(L"This is paragraph %d", ipara);
				if (ipara == 0 || ipara == 2)
					stuPara = L"";
				m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
				rghvoPara[ipara] = hvoPara;
				hvoPara++;
			}
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, kcparaStText);

			// For this test, support the interface that gives us advance notification.
			m_qdrs->m_fSupportAboutToDelete = true;

			m_qvc.Attach(NewObj SimpleStTextVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testStTextEditing Layout succeeded", S_OK, hr);
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwParagraphBox * pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			// Sel at start of second para.
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox2,
				0, 0, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			int wspend = -1;
			// Try typing a backspace at the start of the second paragraph. Because the first
			// paragraph is empty, it is the one that gets deleted.
			hr = m_qrootb->OnTyping(m_qvg32, NULL, 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("First OnTyping failed", S_OK, hr);
			m_qdrs->VerifyAboutToDelete(m_qrootb, khvoParaMin, khvoBook, kflidStText_Paragraphs,
				0, true);

			// Try typing a delete at the end of what's now the second paragraph.Because it's
			// empty, it gets deleted itself.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox2,
				0, 0, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, NULL, 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("Second OnTyping failed", S_OK, hr);
			// This time we destroyed the third original paragraph (but, at the time, it was at
			// index 1, because we deleted the old second paragraph earlier).
			m_qdrs->VerifyAboutToDelete(m_qrootb, khvoParaMin + 2, khvoBook,
				kflidStText_Paragraphs, 1, true);
		}

		void VerifyPropContents(PropTag flid, const OLECHAR * pch)
		{
			ITsStringPtr qtss;
			m_qsda->get_StringProp(khvoBook, flid, &qtss);
			SmartBstr sbstr;
			qtss->get_Text(&sbstr);
			StrAnsi staMsg;
			staMsg.Format("prop %d contents is %S, not %S", flid, sbstr.Chars(), pch);
			unitpp::assert_true(staMsg.Chars(), wcscmp(pch, sbstr.Chars()) == 0);
		}

		void testReadOnlyProps()
		{
			// Create test data. We simulate an object with two string properties, and a display
			// that shows them separated by a literal.
			ITsStringPtr qtss1;
			StrUni stuProp1(L"String 1");
			m_qtsf->MakeString(stuProp1.Bstr(), g_wsEng, &qtss1);
			m_qcda->CacheStringProp(khvoBook, kflidProp1, qtss1);
			StrUni stuProp2(L"String 2");
			ITsStringPtr qtss2;
			m_qtsf->MakeString(stuProp2.Bstr(), g_wsEng, &qtss2);
			m_qcda->CacheStringProp(khvoBook, kflidProp2, qtss2);
			int wspend = -1;

			ITsStringPtr qtssLit;
			StrUni stuLit(L"lit");
			m_qtsf->MakeString(stuLit.Bstr(), g_wsEng, &qtssLit);

			m_qvc.Attach(NewObj StringLitStringVc(qtssLit));

			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testReadOnlyProps Layout succeeded", S_OK, hr);
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwTextSelectionPtr qsel;

			// Try typing a delete at the end of the first string, aborted.
			m_qdrs->SetProbDeleteAction(1); // Typing aborted!
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				stuProp1.Length(), stuProp1.Length(), false, NULL));
			m_qrootb->SetSelection(qsel, false);
			StrUni stuKey(L"b");
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("Del end prop 1 OnTyping failed", S_OK, hr);
			unitpp::assert_eq("Del end prop 1 problem type", kdptDelReadOnly,
				m_qdrs->GetAndResetProblemType());
			ComBool fOk;
			// Commit forces modified prop write to cache (if by accident it was modified).
			qsel->Commit(&fOk);
			VerifyPropContents(kflidProp1, stuProp1.Chars());
			VerifyPropContents(kflidProp2, stuProp2.Chars());
			VerifyParaContents(0, L"String 1litString 2");

			// Try typing a delete at the end of the first string, followed by a b.
			m_qdrs->SetProbDeleteAction(6); // Typing del failed.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				stuProp1.Length(), stuProp1.Length(), false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("Del end prop 1 OnTyping failed", S_OK, hr);
			unitpp::assert_eq("Del end prop 1 problem type", kdptDelReadOnly,
				m_qdrs->GetAndResetProblemType());
			StrUni stuProp1b(L"String 1b");
			qsel->Commit(&fOk); // needed to force modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1b.Chars());
			VerifyPropContents(kflidProp2, stuProp2.Chars());
			VerifyParaContents(0, L"String 1blitString 2");

			// Try typing a backspace at the start of prop2, aborted
			m_qdrs->SetProbDeleteAction(1); // Typing aborted!
			stuKey = L"c";
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			int ichProp2 = stuProp1b.Length() + stuLit.Length();
			qsel.Attach(NewObj VwTextSelection(pvpbox1, ichProp2, ichProp2, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("Bsp start prop 2 failed", S_OK, hr);
			unitpp::assert_eq("Bsp start prop 2 problem type", kdptBsReadOnly,
				m_qdrs->GetAndResetProblemType());
			qsel->Commit(&fOk); // needed to force modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1b.Chars());
			VerifyPropContents(kflidProp2, stuProp2.Chars());
			VerifyParaContents(0, L"String 1blitString 2");

			// Try typing a backspace at the start of prop2, followed by a c
			m_qdrs->SetProbDeleteAction(6); // Typing bs failed.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox1, ichProp2, ichProp2, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("Bsp start prop 2 failed", S_OK, hr);
			unitpp::assert_eq("Bsp start prop 2 problem type", kdptBsReadOnly,
				m_qdrs->GetAndResetProblemType());
			StrUni stuProp2c(L"cString 2");
			qsel->Commit(&fOk); // needed to force modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1b.Chars());
			VerifyPropContents(kflidProp2, stuProp2c.Chars());
			VerifyParaContents(0, L"String 1blitcString 2");

			// Reset to initial state.
			m_qcda->CacheStringProp(khvoBook, kflidProp1, qtss1);
			m_qcda->CacheStringProp(khvoBook, kflidProp2, qtss2);
			m_qrootb->Reconstruct();

			// Try typing a delete at the end of the first string, followed by a b.
			// This time, program the dummy root site to (not very plausibly) move the selection
			// to the start, where the delete can happen, and claim it has already done the
			// backspace.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				stuProp1.Length(), stuProp1.Length(), false, NULL));
			m_qdrs->SetProbDeleteAction(5); // Make new selection at start!
			m_qrootb->SetSelection(qsel, false);
			stuKey = L"b";
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("Del end prop 1 move sel OnTyping failed", S_OK, hr);
			unitpp::assert_eq("Del end prop 1 move sel problem type", kdptDelReadOnly,
				m_qdrs->GetAndResetProblemType());
			StrUni stuProp1e(L"bString 1"); // S kept, b inserted
			m_qrootb->Selection()->Commit(&fOk); // forces modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1e.Chars());
			VerifyPropContents(kflidProp2, stuProp2.Chars());
			VerifyParaContents(0, L"bString 1litString 2");

			// Reset to initial state.
			m_qcda->CacheStringProp(khvoBook, kflidProp1, qtss1);
			m_qcda->CacheStringProp(khvoBook, kflidProp2, qtss2);
			m_qrootb->Reconstruct();

			// Try typing a delete at the end of the first string, followed by a b.
			// This time, program the dummy root site to (not very plausibly) move the selection
			// to the start, where the delete can happen.
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			qsel.Attach(NewObj VwTextSelection(pvpbox1,
				stuProp1.Length(), stuProp1.Length(), false, NULL));
			m_qdrs->SetProbDeleteAction(3); // Make new selection at start!
			m_qrootb->SetSelection(qsel, false);
			stuKey = L"b";
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 0, 1, (OLECHAR)0x7f, &wspend);
			unitpp::assert_eq("Del end prop 1 move sel OnTyping failed", S_OK, hr);
			unitpp::assert_eq("Del end prop 1 move sel problem type", kdptDelReadOnly,
				m_qdrs->GetAndResetProblemType());
			StrUni stuProp1d(L"btring 1"); // S deleted, b inserted
			m_qrootb->Selection()->Commit(&fOk); // forces modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1d.Chars());
			VerifyPropContents(kflidProp2, stuProp2.Chars());
			VerifyParaContents(0, L"btring 1litString 2");

			// Reset to initial state.
			m_qcda->CacheStringProp(khvoBook, kflidProp1, qtss1);
			m_qrootb->Reconstruct();

			// Try typing a backspace at the start of prop2, followed by a c.
			// This time, program the root site to move the selection to the end,
			// where it can happen, and claim to have done the bsp.
			stuKey = L"c";
			// Make new selection at end and claim to have done bsp!
			m_qdrs->SetProbDeleteAction(4);
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			ichProp2 = stuProp1.Length() + stuLit.Length();
			qsel.Attach(NewObj VwTextSelection(pvpbox1, ichProp2, ichProp2, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("Bsp start prop 2 move sel failed", S_OK, hr);
			unitpp::assert_eq("Bsp start prop 2 move sel problem type", kdptBsReadOnly,
				m_qdrs->GetAndResetProblemType());
			StrUni stuProp2g(L"String 2c");
			m_qrootb->Selection()->Commit(&fOk); // forces modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1.Chars());
			VerifyPropContents(kflidProp2, stuProp2g.Chars());
			VerifyParaContents(0, L"String 1litString 2c");

			// Reset to initial state.
			m_qcda->CacheStringProp(khvoBook, kflidProp2, qtss2);
			m_qrootb->Reconstruct();

			// Try typing a backspace at the start of prop2, followed by a c.
			// This time, program the root site to move the selection to the end,
			// where it can happen.
			stuKey = L"c";
			m_qdrs->SetProbDeleteAction(2); // Make new selection at end!
			pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			ichProp2 = stuProp1.Length() + stuLit.Length();
			qsel.Attach(NewObj VwTextSelection(pvpbox1, ichProp2, ichProp2, false, NULL));
			m_qrootb->SetSelection(qsel, false);
			hr = m_qrootb->OnTyping(m_qvg32, stuKey.Bstr(), 1, 0, (OLECHAR)'\b', &wspend);
			unitpp::assert_eq("Bsp start prop 2 move sel failed", S_OK, hr);
			unitpp::assert_eq("Bsp start prop 2 move sel problem type", kdptBsReadOnly,
				m_qdrs->GetAndResetProblemType());
			StrUni stuProp2e(L"String c");
			m_qrootb->Selection()->Commit(&fOk); // forces modified prop write to cache.
			VerifyPropContents(kflidProp1, stuProp1.Chars());
			VerifyPropContents(kflidProp2, stuProp2e.Chars());
			VerifyParaContents(0, L"String 1litString c");
		}

		void testDropCapsPosition_TimesNewRoman()
		{
			// Set up a simple stylesheet
			StrUni stuChapterStyleName(L"Chapter Number");
			ComSmartPtr<VwStylesheet> qvss;
			qvss.Attach(NewObj VwStylesheet());
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuChapterStyleName.Bstr(), NULL, 0, 0, 0, 0, false, false, pttp);

			// Create test data. We just display a literal string, whose first run
			// has "Chapter Number" style (this causes it to be rendered as a drop cap).
			ITsStringPtr qtss;
			// Now make one string, the contents of paragraph 1.
			StrUni stuPara1(L"1");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			ITsStrBldrPtr qtsb;
			qtss->GetBldr(&qtsb);
			qtsb->SetStrPropValue(0, 1, ktptNamedStyle, stuChapterStyleName.Bstr());
			qtsb->GetString(&qtss);

			// Part 1: Establish baseline values for the top of the paragraph and the
			// location of the selection when the chapter number is the only thing in the
			// paragraph.
			m_qvc.Attach(NewObj LitVc(qtss));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded with Chapter number by itself", S_OK, hr);
			int dypHeightWithChapterNumberByItself = m_qrootb->Height();
			IVwSelectionPtr qsel;
			CheckHr(m_qrootb->MakeSimpleSel(false, false, false, true, &qsel));
			HoldGraphics hg(m_qrootb);
			RECT rcSelChapterNumberParaByItself;
			RECT rcSecondary;
			ComBool fSplit;
			ComBool fEndBeforeAnchor;
			CheckHr(qsel->Location(hg.m_qvg, hg.m_rcSrcRoot, hg.m_rcDstRoot,
				&rcSelChapterNumberParaByItself,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			// Part 2: Determine values for the top of the paragraph and the
			// location of the selection when the chapter number is followed by additional text.
			m_qrootb.Clear();
			m_qvc.Clear();
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);

			qtsb->ReplaceRgch(1, 1, L" Wow!", 5, NULL);
			StrUni stuNoStyleName(L"");
			qtsb->SetStrPropValue(1, 6, ktptNamedStyle, stuNoStyleName.Bstr());
			qtsb->GetString(&qtss);

			m_qvc.Attach(NewObj LitVc(qtss));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded with chapter followed by text", S_OK, hr);
			int dypHeightWithChapterNumberPlusText = m_qrootb->Height();
			CheckHr(m_qrootb->MakeSimpleSel(true, false, false, true, &qsel));
			VwTextSelection * ptextsel = dynamic_cast<VwTextSelection *>(qsel.Ptr());
			ptextsel->RightArrow(m_qvg32, true);
			HoldGraphics hg1(m_qrootb);
			RECT rcSelChapterNumberParaWithMoreText;
			CheckHr(qsel->Location(hg1.m_qvg, hg1.m_rcSrcRoot, hg1.m_rcDstRoot,
				&rcSelChapterNumberParaWithMoreText,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			unitpp::assert_eq("Height of rootbox should be the same whether or not text follows chapter number.",
				dypHeightWithChapterNumberByItself, dypHeightWithChapterNumberPlusText);
			unitpp::assert_eq("Insertion point (top) locations should be the same whether or not text follows chapter number.",
				rcSelChapterNumberParaByItself.top, rcSelChapterNumberParaWithMoreText.top);
			unitpp::assert_eq("Insertion point (bottom) locations should be the same whether or not text follows chapter number.",
				rcSelChapterNumberParaByItself.bottom, rcSelChapterNumberParaWithMoreText.bottom);
		}

		void testDropCapsPosition_Charis_ExactLineSpacing() // Jira # is TE-5233
		{
			// Set the publication font for French to Charis SIL
			IWritingSystemPtr qws;
			CheckHr(g_qwsf->get_EngineOrNull(g_wsFrn, &qws));
			StrUni stuCharis(L"Charis SIL");
			qws->put_DefaultBodyFont(stuCharis.Bstr());

			// Set up a simple stylesheet
			StrUni stuChapterStyleName(L"Chapter Number");
			ComSmartPtr<VwStylesheet> qvss;
			qvss.Attach(NewObj VwStylesheet());
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;

			// Add a Chapter Number style
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuChapterStyleName.Bstr(), NULL, 0, 0, 0, 0, false, false, pttp);

			// Add the Paragraph style, which uses <default pub font>
			StrUni stuParagraphStyleName(L"Paragraph");
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			StrUni stuFont(L"<default pub font>");
			qtpb->SetStrPropValue(ktptFontFamily, stuFont.Bstr());
			qtpb->SetIntPropValues(ktptLineHeight, ktpvMilliPoint, -12000); // Exactly 12 pt line spacing
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuParagraphStyleName.Bstr(), NULL, 0, 0, 0, 1, false, false, pttp);

			// Create test data. We just display a literal string of plain text.
			ITsStringPtr qtss;
			// Now make one string, the contents of the paragraph.
			StrUni stuPara1(L"This is the first test paragraph, long enough to wrap and cover at least three whole lines, so that if the drop cap is the right size...");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsFrn, &qtss);
			ITsStrBldrPtr qtsb;
			qtss->GetBldr(&qtsb);
			qtsb->GetString(&qtss);

			// Part 1: Establish baseline values for the size of the paragraph with no drop cap.
			m_qvc.Attach(NewObj SimpleParaVc(qtss, stuParagraphStyleName));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded for plain text", S_OK, hr);
			int dypHeightWithoutChapterNumber = m_qrootb->Height();

			// Part 2: Determine values for the size of the paragraph with a chapter number.
			m_qrootb.Clear();
			m_qvc.Clear();
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);

			qtsb->ReplaceRgch(0, 1, L"3", 1, NULL);
			qtsb->SetStrPropValue(0, 1, ktptNamedStyle, stuChapterStyleName.Bstr());
			qtsb->GetString(&qtss);

			m_qvc.Attach(NewObj SimpleParaVc(qtss, stuParagraphStyleName));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded with chapter followed by text", S_OK, hr);
			int dypHeightWithChapterNumberPlusText = m_qrootb->Height();

			unitpp::assert_eq("Height of rootbox should be the same whether or not there is a chapter number.",
				dypHeightWithoutChapterNumber, dypHeightWithChapterNumberPlusText);
		}

		void ignoreDropCapsPosition_Charis_AtLeastLineSpacing() // Jira # is TE-5244
		{
			// Set the publication font for French to Charis SIL
			IWritingSystemPtr qws;
			CheckHr(g_qwsf->get_EngineOrNull(g_wsFrn, &qws));
			StrUni stuCharis(L"Charis SIL");
			qws->put_DefaultBodyFont(stuCharis.Bstr());

			// Set up a simple stylesheet
			StrUni stuChapterStyleName(L"Chapter Number");
			ComSmartPtr<VwStylesheet> qvss;
			qvss.Attach(NewObj VwStylesheet());
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;

			// Add a Chapter Number style
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuChapterStyleName.Bstr(), NULL, 0, 0, 0, 0, false, false, pttp);

			// Add the Paragraph style, which uses <default pub font>
			StrUni stuParagraphStyleName(L"Paragraph");
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			StrUni stuFont(L"<default pub font>");
			qtpb->SetStrPropValue(ktptFontFamily, stuFont.Bstr());
			qtpb->SetIntPropValues(ktptLineHeight, ktpvMilliPoint, 0); // At least 0 pt line spacing
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuParagraphStyleName.Bstr(), NULL, 0, 0, 0, 1, false, false, pttp);

			// Create test data. We just display a literal string of plain text.
			ITsStringPtr qtss;
			// Now make one string, the contents of the paragraph.
			StrUni stuPara1(L"This is the first test paragraph, long enough to wrap and cover at least three whole lines, so that if the drop cap is the right size...");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsFrn, &qtss);
			ITsStrBldrPtr qtsb;
			qtss->GetBldr(&qtsb);
			qtsb->GetString(&qtss);

			// Part 1: Establish baseline values for the size of the paragraph with no drop cap.
			m_qvc.Attach(NewObj SimpleParaVc(qtss, stuParagraphStyleName));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded for plain text", S_OK, hr);
			int dypHeightWithoutChapterNumber = m_qrootb->Height();

			// Part 2: Determine values for the size of the paragraph with a chapter number.
			m_qrootb.Clear();
			m_qvc.Clear();
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);

			qtsb->ReplaceRgch(0, 1, L"3", 1, NULL);
			qtsb->SetStrPropValue(0, 1, ktptNamedStyle, stuChapterStyleName.Bstr());
			qtsb->GetString(&qtss);

			m_qvc.Attach(NewObj SimpleParaVc(qtss, stuParagraphStyleName));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded with chapter followed by text", S_OK, hr);
			int dypHeightWithChapterNumberPlusText = m_qrootb->Height();

			unitpp::assert_eq("Height of rootbox should be the same whether or not there is a chapter number.",
				dypHeightWithoutChapterNumber, dypHeightWithChapterNumberPlusText);
		}

		void testDropCapsPosition_usingPubFont() // Jira # is TE-5233
		{
			// Set up a simple stylesheet
			StrUni stuChapterStyleName(L"Chapter Number");
			ComSmartPtr<VwStylesheet> qvss;
			qvss.Attach(NewObj VwStylesheet());
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;

			// Add a Chapter Number style
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuChapterStyleName.Bstr(), NULL, 0, 0, 0, 0, false, false, pttp);

			// Add the Paragraph style, which uses <default pub font>
			StrUni stuParagraphStyleName(L"Paragraph");
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			StrUni stuFont(L"<default pub font>");
			qtpb->SetStrPropValue(ktptFontFamily, stuFont.Bstr());
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuParagraphStyleName.Bstr(), NULL, 0, 0, 0, 1, false, false, pttp);

			// Create test data. We just display a literal string, whose only run
			// has "Chapter Number" style (this causes it to be rendered as a drop cap).
			ITsStringPtr qtss;
			// Now make one string, the contents of the paragraph.
			StrUni stuPara1(L"1");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsFrn, &qtss); // This is FRENCH!
			ITsStrBldrPtr qtsb;
			qtss->GetBldr(&qtsb);
			qtsb->SetStrPropValue(0, 1, ktptNamedStyle, stuChapterStyleName.Bstr());
			qtsb->GetString(&qtss);

			// Part 1: Establish baseline values for the top of the paragraph and the
			// location of the selection when <default pub font> for French is the same
			// as the <default pub font> used for English, namely Times New Roman.
			m_qvc.Attach(NewObj SimpleParaVc(qtss, stuParagraphStyleName));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded for <default pub font> = Times New Roman", S_OK, hr);
			int dypHeightWithTimesNewRoman = m_qrootb->Height();
			IVwSelectionPtr qsel;
			CheckHr(m_qrootb->MakeSimpleSel(false, false, false, true, &qsel));
			HoldGraphics hg(m_qrootb);
			RECT rcSelChapterNumberTimesNewRoman;
			RECT rcSecondary;
			ComBool fSplit;
			ComBool fEndBeforeAnchor;
			CheckHr(qsel->Location(hg.m_qvg, hg.m_rcSrcRoot, hg.m_rcDstRoot,
				&rcSelChapterNumberTimesNewRoman,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			// Part 2: Determine values for the top of the paragraph and the
			// location of the selection when  the <default pub font> for
			// English is set to Charis SIL, which is different from the
			// <default pub font> for French (Times New Roman).
			IWritingSystemPtr qws;
			CheckHr(g_qwsf->get_EngineOrNull(g_wsEng, &qws));
			StrUni stuCharis(L"Charis SIL");
			qws->put_DefaultBodyFont(stuCharis.Bstr());

			m_qrootb.Clear();
			m_qvc.Clear();
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);

			m_qvc.Attach(NewObj SimpleParaVc(qtss, stuParagraphStyleName));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, qvss);
			hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testDropCapsPosition Layout succeeded using <default pub font>", S_OK, hr);
			int dypHeightWithEnglishCharis = m_qrootb->Height();
			CheckHr(m_qrootb->MakeSimpleSel(true, false, false, true, &qsel));
			HoldGraphics hg1(m_qrootb);
			RECT rcSelChapterNumberEngCharis;
			CheckHr(qsel->Location(hg1.m_qvg, hg1.m_rcSrcRoot, hg1.m_rcDstRoot,
				&rcSelChapterNumberEngCharis,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			unitpp::assert_eq("Height of rootbox should not change regardless of pub font used for English.",
				dypHeightWithTimesNewRoman, dypHeightWithEnglishCharis);
			unitpp::assert_eq("Insertion point (top) locations should be the same regardless of font.",
				rcSelChapterNumberTimesNewRoman.top, rcSelChapterNumberEngCharis.top);
			unitpp::assert_eq("Insertion point (bottom) locations should not change regardless of pub font used for English.",
				rcSelChapterNumberTimesNewRoman.bottom, rcSelChapterNumberEngCharis.bottom);
		}

		void testDropCapsWithMultipleParagraphs()
		{
			// Set up a simple stylesheet
			StrUni stuChapterStyleName(L"Chapter Number");
			ComSmartPtr<VwStylesheet> qvss;
			qvss.Attach(NewObj VwStylesheet());
			ITsTextProps * pttp;
			ITsPropsBldrPtr qtpb;
			qtpb.CreateInstance(CLSID_TsPropsBldr);
			qtpb->GetTextProps(&pttp);
			qvss->PutStyle(stuChapterStyleName.Bstr(), NULL, 0, 0, 0, 0, false, false, pttp);

			// Part 1: Establish baseline values for the top of the second paragraph and the
			// location of the selection when the first paragraph does NOT begin with a drop-cap
			// chapter number.

			// Create test data. We display two paragraphs with literal strings.
			ITsStringPtr qtss1, qtss2;
			// Make the first string, the contents of paragraph 1.
			StrUni stuPara1(L"1First line");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss1);
			// Make the second string, the contents of paragraph 2.
			StrUni stuPara2(L"Second line");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss2);

			m_qvc.Attach(NewObj LitVc(qtss1, qtss2));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 2, qvss);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 3000); // must be wide enough to fit para contents on one line
			unitpp::assert_eq("testDropCapsWithMultipleParagraphs baseline Layout succeeded", S_OK, hr);
			IVwSelectionPtr qsel;
			CheckHr(m_qrootb->MakeSimpleSel(false, false, false, true, &qsel));
			HoldGraphics hg(m_qrootb);
			RECT rcSelFollowingNoChapterNumberPara;
			RECT rcSecondary;
			ComBool fSplit;
			ComBool fEndBeforeAnchor;
			CheckHr(qsel->Location(hg.m_qvg, hg.m_rcSrcRoot, hg.m_rcDstRoot,
				&rcSelFollowingNoChapterNumberPara,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			// Part 2: Determine values for the top of the second paragraph and the
			// location of the selection when the first paragraph begins with a chapter
			// number.
			ITsStrBldrPtr qtsb;
			qtss1->GetBldr(&qtsb);
			qtsb->SetStrPropValue(0, 1, ktptNamedStyle, stuChapterStyleName.Bstr());
			qtsb->GetString(&qtss1);

			m_qrootb.Clear();
			m_qvc.Clear();
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);

			m_qvc.Attach(NewObj LitVc(qtss1, qtss2));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 2, qvss);
			hr = m_qrootb->Layout(m_qvg32, 3000);  // must be wide enough to fit para contents on one line
			unitpp::assert_eq("testDropCapsWithMultipleParagraphs part 2 Layout succeeded", S_OK, hr);
			CheckHr(m_qrootb->MakeSimpleSel(false, false, false, true, &qsel));
			HoldGraphics hg1(m_qrootb);
			RECT rcSelFollowingChapterNumberPara;
			CheckHr(qsel->Location(hg1.m_qvg, hg1.m_rcSrcRoot, hg1.m_rcDstRoot,
				&rcSelFollowingChapterNumberPara,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			// The ascent computed for the first paragraph may increase slightly to accommodate the drop cap.
			// Figure out by how much.
			VwParagraphBox * pvpbox = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstBox());
			VwBox * pboxmain = pvpbox->FirstBox()->NextOrLazy();
			int dyOffset = pboxmain->Top();
			unitpp::assert_true("Insertion point (top) locations should be nearly the same whether or not text follows para with drop-cap chapter number.",
				rcSelFollowingChapterNumberPara.top - rcSelFollowingNoChapterNumberPara.top == dyOffset);
			unitpp::assert_eq("Insertion point (bottom) locations should be consistent in 2nd para wrapped around DC.",
				rcSelFollowingChapterNumberPara.bottom - rcSelFollowingChapterNumberPara.top,
				rcSelFollowingNoChapterNumberPara.bottom - rcSelFollowingNoChapterNumberPara.top);
			unitpp::assert_true("Insertion point (left) locations should be larger in para following one-line drop cap.",
				rcSelFollowingNoChapterNumberPara.left < rcSelFollowingChapterNumberPara.left);

			// Part 3: second paragraph should not merge if it also has drop cap.
			// This is also a good chance to check that a final paragraph with DC DOES affect the
			// overall view height.
			int dyHeightWithOneDropCap;
			m_qrootb->get_Height(&dyHeightWithOneDropCap); // before we reset everything!

			qtss2->GetBldr(&qtsb);
			qtsb->SetStrPropValue(0, 1, ktptNamedStyle, stuChapterStyleName.Bstr());
			qtsb->GetString(&qtss2);

			m_qrootb.Clear();
			m_qvc.Clear();
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);

			m_qvc.Attach(NewObj LitVc(qtss1, qtss2));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 2, qvss);
			hr = m_qrootb->Layout(m_qvg32, 3000);  // must be wide enough to fit para contents on one line
			unitpp::assert_eq("testDropCapsWithMultipleParagraphs part 3 Layout succeeded", S_OK, hr);
			CheckHr(m_qrootb->MakeSimpleSel(false, false, false, true, &qsel));
			HoldGraphics hg3(m_qrootb);
			RECT rcSelFollowingChapterNumberParaInChapNumberPara;
			CheckHr(qsel->Location(hg3.m_qvg, hg3.m_rcSrcRoot, hg3.m_rcDstRoot,
				&rcSelFollowingChapterNumberParaInChapNumberPara,
				&rcSecondary, &fSplit, &fEndBeforeAnchor));

			unitpp::assert_true("Insertion point (top) locations should be increased in DC para following DC para.",
				rcSelFollowingChapterNumberParaInChapNumberPara.top > rcSelFollowingChapterNumberPara.top);
			unitpp::assert_eq("Insertion point (bottom) locations should be consistent in adjacent DC paras.",
				rcSelFollowingChapterNumberParaInChapNumberPara.bottom - rcSelFollowingChapterNumberParaInChapNumberPara.top,
				rcSelFollowingChapterNumberPara.bottom - rcSelFollowingChapterNumberPara.top);

			int dyHeightWithTwoDropCaps;
			m_qrootb->get_Height(&dyHeightWithTwoDropCaps);
			// An increase of 5 pixels or so could be just increased ascent of the second para because
			// of the drop cap.
			unitpp::assert_true("Root box height should be substantially increased by drop cap in final one-line para.",
				dyHeightWithTwoDropCaps - dyHeightWithOneDropCap > 15);
		}

		void testNonBreakingSpace()
		{
			// Create test data. We display two paragraphs with literal strings.
			ITsStringPtr qtss;
			// Make the first string, the contents of paragraph 1.
			StrUni stuPara1(L"abcdfffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			ITsStrBldrPtr qtsb;
			qtss->GetBldr(&qtsb);
			OLECHAR chNoBrkSpace = 0x00A0;
			CheckHr(qtsb->ReplaceRgch(3, 3, &chNoBrkSpace, 1, NULL));
			CheckHr(qtsb->GetString(&qtss));

			m_qvc.Attach(NewObj LitVc(qtss));
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			CheckHr(m_qrootb->Layout(m_qvg32, 100));
			CheckHr(m_qrootb->Activate(vssEnabled));
			IVwSelectionPtr qsel;
			CheckHr(m_qrootb->MakeSimpleSel(true, false, false, true, &qsel));
			Rect rc = Rect(0, 0, 96, 96);
			//VwTextSelectionPtr qtxtsel;
			int xdPos;
			bool fSuccess = (dynamic_cast<VwTextSelection*>(qsel.Ptr()))->DownArrow(m_qvg32, rc, rc, &xdPos);
			unitpp::assert_true("Down arrow didn't work.", fSuccess);
			int ich;
			CheckHr(qsel->get_ParagraphOffset(false, &ich));
			unitpp::assert_true("Line broke at the non-breaking space.", ich > 5);
		}

		void testExpandToWord()
		{
			ITsStringPtr qtss;
			ITsStringPtr qtssT;
			// Make one string, the contents of a paragraph, containing three words: "M�m�xyz"
			// first composed and then decomposed, and then "MAmaxyz" (no umlauts).
			// Then make it a paragraph of an StText.
			StrUni stuPara1(L"M" L"\x00C4" L"m" L"\x00E4" L"xyz MA" L"\x0308" L"ma" L"\x0308"
				L"xyz MAMaxyz");
			int cch1 = stuPara1.Length();
			m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss);
			SetTextProp(qtss, 0, 7, ktptFontSize, ktpvMilliPoint, 20000, &qtssT);
			SetTextProp(qtssT, cch1-3, cch1, ktptWs, ktpvDefault, g_wsFrn, &qtss);
			m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss);
			HVO rghvo[1] = { khvoOrigPara1 };
			HVO hvoRoot = 101;
			m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1);

			m_qvc.Attach(NewObj DummyParaVc());
			m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testExpandToWord: Layout hr", S_OK, hr);

			// Put insertion point at the beginning of the view
			IVwSelectionPtr qsel;
			hr = m_qrootb->MakeSimpleSel(true, true, false, true, &qsel);
			unitpp::assert_eq("testExpandToWord: first MakeSimpleSel hr", S_OK, hr);
			IVwSelectionPtr qselNew;
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselNew);
			unitpp::assert_eq("testExpandToWord: second MakeSimpleSel hr", S_OK, hr);
			VwTextSelection * pzsel = dynamic_cast<VwTextSelection *>(qsel.Ptr());
			unitpp::assert_true("testExpandToWord: cast qsel to pzsel", pzsel != NULL);
			VwTextSelection * pzselNew = dynamic_cast<VwTextSelection *>(qselNew.Ptr());
			unitpp::assert_true("testExpandToWord: cast qselNew to pzselNew", pzselNew != NULL);
			// Expand the selection to the entire first word.
			bool fT = pzsel->ExpandToWord(pzselNew);
			unitpp::assert_true("first ExpandToWord return value", fT);
			unitpp::assert_eq("first ExpandToWord ichAnchor", 0, pzsel->m_ichAnchor);
			unitpp::assert_eq("first ExpandToWord ichEnd", 7, pzsel->m_ichEnd);

			// Make the selections both an insertion point in the middle of the second word.
			pzsel->m_ichAnchor = 12;
			pzsel->m_ichEnd = 12;
			pzsel->m_ichAnchor2 = -1;
			pzselNew->m_ichAnchor = 12;
			pzselNew->m_ichEnd = 12;
			pzselNew->m_ichAnchor2 = -1;
			// Expand the selection to the entire second word.
			fT = pzsel->ExpandToWord(pzselNew);
			unitpp::assert_true("second ExpandToWord return value", fT);
			unitpp::assert_eq("second ExpandToWord ichAnchor", 8, pzsel->m_ichAnchor);
			unitpp::assert_eq("second ExpandToWord ichEnd", 17, pzsel->m_ichEnd);

			// Make the selections both an insertion point at the end of the paragraph.
			pzsel->m_ichAnchor = stuPara1.Length();
			pzsel->m_ichEnd = stuPara1.Length();
			pzsel->m_ichAnchor2 = -1;
			pzselNew->m_ichAnchor = stuPara1.Length();
			pzselNew->m_ichEnd = stuPara1.Length();
			pzselNew->m_ichAnchor2 = -1;
			// Expand the selection to the entire final word.
			fT = pzsel->ExpandToWord(pzselNew);
			unitpp::assert_true("third ExpandToWord return value", fT);
			unitpp::assert_eq("third ExpandToWord ichAnchor",
				22, pzsel->m_ichAnchor);
			unitpp::assert_eq("third ExpandToWord ichEnd",
				stuPara1.Length(), pzsel->m_ichEnd);
		}

		void VerifyBoxInfo(IVwSelection * psel, bool fEndPoint, const char * pszTag,
			const int kcLevels, const int krgiBox[], const int krgcBoxes[],
			const VwBoxType krgvbt[])
		{
			int iLevel;
			int cLevels;
			int iBox;
			int cBoxes;
			VwBoxType vbt;
			HRESULT hr;
			StrAnsi sta;
			const char * pszFlag = fEndPoint ? "true" : "false";

			hr = psel->get_BoxDepth(fEndPoint, &cLevels);
			sta.Format("%s get_BoxDepth(%s) HRESULT", pszTag, pszFlag);
			unitpp::assert_eq(sta.Chars(), S_OK, hr);
			sta.Format("%s get_BoxDepth(%s) cLevels", pszTag, pszFlag);
			unitpp::assert_eq(sta.Chars(), kcLevels, cLevels);
			for (iLevel = 0; iLevel < cLevels; ++iLevel)
			{
				hr = psel->get_BoxIndex(fEndPoint, iLevel, &iBox);
				sta.Format("%s get_BoxIndex(%s, %d) HRESULT", pszTag, pszFlag, iLevel);
				unitpp::assert_eq(sta.Chars(), S_OK, hr);
				sta.Format("%s get_BoxIndex(%s, %d) iBox", pszTag, pszFlag, iLevel);
				unitpp::assert_eq(sta.Chars(), krgiBox[iLevel], iBox);

				hr = psel->get_BoxCount(fEndPoint, iLevel, &cBoxes);
				sta.Format("%s get_BoxCount(%s, %d) HRESULT", pszTag, pszFlag, iLevel);
				unitpp::assert_eq(sta.Chars(), S_OK, hr);
				sta.Format("%s get_BoxCount(%s, %d) cBoxes", pszTag, pszFlag, iLevel);
				unitpp::assert_eq(sta.Chars(), krgcBoxes[iLevel], cBoxes);

				hr = psel->get_BoxType(fEndPoint, iLevel, &vbt);
				sta.Format("%s get_BoxType(%s, %d) HRESULT", pszTag, pszFlag, iLevel);
				unitpp::assert_eq(sta.Chars(), S_OK, hr);
				sta.Format("%s get_BoxType(%s, %d) vbt", pszTag, pszFlag, iLevel);
				unitpp::assert_eq(sta.Chars(), krgvbt[iLevel], vbt);
			}
		}

		void testBoxInfo1()
		{
			// Create the view and selection used in testEmbeddedObjects().
			CreateSimplestView();
			IVwSelectionPtr qselTemp;
			m_qrootb->MakeSimpleSel(true, true, true, true, &qselTemp);

			static const int krgiBox[2] = { 0, 0 };
			static const int krgcBoxes[2] = { 1, 1 };
			static const VwBoxType krgvbt[2] = { kvbtRoot, kvbtParagraph };
			VerifyBoxInfo(qselTemp, true, "First", 2, krgiBox, krgcBoxes, krgvbt);
			VerifyBoxInfo(qselTemp, false, "First", 2, krgiBox, krgcBoxes, krgvbt);
		}

		void testBoxInfo2()
		{
			HRESULT hr = CreateSimpleStText();
			unitpp::assert_eq("testBoxInfo2 Layout succeeded", S_OK, hr);
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwParagraphBox * pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			VwTextSelectionPtr qsel;
			qsel.Attach(NewObj VwTextSelection(pvpbox1, 5, 4, false, pvpbox2));
			m_qrootb->SetSelection(qsel, false);

			static const int krgiBoxTrue[2] = { 0, 1 };
			static const int krgiBoxFalse[2] = { 0, 0 };
			static const int krgcBoxes[2] = { 1, 4 };
			static const VwBoxType krgvbt[2] = { kvbtRoot, kvbtParagraph };
			VerifyBoxInfo(qsel, true, "Second", 2, krgiBoxTrue, krgcBoxes, krgvbt);
			VerifyBoxInfo(qsel, false, "Second", 2, krgiBoxFalse, krgcBoxes, krgvbt);
		}

		void testBoxInfo3()
		{
			HRESULT hr = CreateBkSecPara();
			unitpp::assert_eq("testBoxInfo3 Layout succeeded", S_OK, hr);

			VwTextSelectionPtr qzsel;
			VwParagraphBox * pvpbox1 = dynamic_cast<VwParagraphBox *>(m_qrootb->FirstRealBox());
			VwParagraphBox * pvpbox2 = dynamic_cast<VwParagraphBox *>(pvpbox1->NextRealBox());
			qzsel.Attach(NewObj VwTextSelection(pvpbox1, 5, 3, false, pvpbox2));
			m_qrootb->SetSelection(qzsel, false);
			static const int krgiBoxATrue[2] = { 0, 1 };
			static const int krgiBoxAFalse[2] = { 0, 0 };
			static const int krgcBoxes[2] = { 1, 4 };
			static const VwBoxType krgvbt[2] = { kvbtRoot, kvbtParagraph };
			VerifyBoxInfo(qzsel, true, "Third(A)", 2, krgiBoxATrue, krgcBoxes, krgvbt);
			VerifyBoxInfo(qzsel, false, "Third(A)", 2, krgiBoxAFalse, krgcBoxes, krgvbt);

			// REVIEW(DamienD): This appears to insert a couple of lazy boxes just before the
			// last (4th) paragraph box.  Is this correct behavior?
			VwParagraphBox * pvpbox3 = dynamic_cast<VwParagraphBox *>(pvpbox2->NextRealBox());
			qzsel.Attach(NewObj VwTextSelection(pvpbox2, 5, 3, false, pvpbox3));
			m_qrootb->SetSelection(qzsel, false);
			static const int krgiBoxBTrue[2] = { 0, 2 };
			static const int krgiBoxBFalse[2] = { 0, 1 };
			static const int krgcBoxesB[2] = { 1, 6 };
			VerifyBoxInfo(qzsel, true, "Third(B)", 2, krgiBoxBTrue, krgcBoxesB, krgvbt);
			VerifyBoxInfo(qzsel, false, "Third(B)", 2, krgiBoxBFalse, krgcBoxesB, krgvbt);

			// Make a selection at the very start of the view.
			IVwSelectionPtr qvwsel;
			hr = m_qrootb->MakeSimpleSel(true, true, false, false, &qvwsel);
			unitpp::assert_eq("testBoxInfo3 making third selection succeeded", S_OK, hr);
			static const int krgiBoxC[2] = { 0, 0 };
			VerifyBoxInfo(qvwsel, true, "Third(C)", 2, krgiBoxC, krgcBoxesB, krgvbt);
			VerifyBoxInfo(qvwsel, false, "Third(C)", 2, krgiBoxC, krgcBoxesB, krgvbt);

			// Make a selection at the very end of the view.
			// REVIEW(JohnT): This appears to insert a couple of lazy boxes just before the
			// last (4th) paragraph box.  Is this correct behavior?
			hr = m_qrootb->MakeSimpleSel(false, true, false, false, &qvwsel);
			unitpp::assert_eq("testBoxInfo3 making fourth selection succeeded", S_OK, hr);
			static const int krgiBoxD[2] = { 0, 6 };
			static const int krgcBoxesD[2] = { 1, 7 };
			VerifyBoxInfo(qvwsel, true, "Third(D)", 2, krgiBoxD, krgcBoxesD, krgvbt);
			VerifyBoxInfo(qvwsel, false, "Third(D)", 2, krgiBoxD, krgcBoxesD, krgvbt);
		}

		// Tests the IsEnabled method when selection state is enabled
		void testIsEnabled_Enabled()
		{
			CreateSimplestView();

			m_qrootb->Activate(vssEnabled);

			IVwSelectionPtr qselTemp;
			// Make IP selection
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);

			ComBool f;
			qselTemp->get_IsEnabled(&f);
			unitpp::assert_true("Selection should report as being enabled", f);

			// Make range selection
			m_qrootb->MakeSimpleSel(true, true, true, true, &qselTemp);

			qselTemp->get_IsEnabled(&f);
			unitpp::assert_true("Selection should report as being enabled", f);
		}

		// Tests the IsEnabled method when selection state is disabled
		void testIsEnabled_Disabled()
		{
			CreateSimplestView();

			m_qrootb->Activate(vssDisabled);

			IVwSelectionPtr qselTemp;
			// Make IP selection
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);

			ComBool f;
			qselTemp->get_IsEnabled(&f);
			unitpp::assert_true("Selection should report as being disabled", !f);

			// Make range selection
			m_qrootb->MakeSimpleSel(true, true, true, true, &qselTemp);

			qselTemp->get_IsEnabled(&f);
			unitpp::assert_true("Selection should report as being disabled", !f);
		}

		// Tests the IsEnabled method when selection state is OutOfFocus
		void testIsEnabled_OutOfFocus()
		{
			CreateSimplestView();

			m_qrootb->Activate(vssOutOfFocus);

			IVwSelectionPtr qselTemp;
			// Make IP selection
			m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp);

			ComBool f;
			qselTemp->get_IsEnabled(&f);
			unitpp::assert_true("Selection should report as being disabled", !f);

			// Make range selection
			m_qrootb->MakeSimpleSel(true, true, true, true, &qselTemp);

			qselTemp->get_IsEnabled(&f);
			unitpp::assert_true("Selection should report as being enabled", f);
		}

		// Tests one-column selections in a table with multiple columns.
		void testOneColumnSelection()
		{
			// Create test data. We simulate a book 4 paragraphs.
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcparaStText];
			for (int ipara = 0; ipara < kcparaStText; ipara++)
			{
				stuPara.Format(L"This is paragraph %d", ipara);
				m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
				// for simplicity, we just reuse the same hvo for the translation, and we also
				// don't set up the connection between StTxtPara and CmTranslation - we make
				// it implicit by using the same hvo.
				m_qcda->CacheStringProp(hvoPara, kflidCmTranslation_Translation, qtss);
				rghvoPara[ipara] = hvoPara;
				hvoPara++;
			}
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, kcparaStText);

			m_qvc.Attach(NewObj TableStTextVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 4, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 300);
			unitpp::assert_eq("testOneColumnSelection Layout didn't succeed", S_OK, hr);

			// Make a selection from first character in first paragraph to third character
			// in second paragraph
			VwParagraphBox * pvpbox1 = FindParaBoxInsideOf(
				dynamic_cast<VwGroupBox *>(m_qrootb->FirstRealBox()->NextRealBox()));
			VwParagraphBox * pvpbox2 = FindParaBoxInsideOf(
				dynamic_cast<VwGroupBox *>(m_qrootb->FirstRealBox()->NextRealBox()->NextRealBox()));
			VwTextSelectionPtr qsel1;
			VwTextSelectionPtr qsel2;
			IVwSelectionPtr qselRange;
			qsel1.Attach(NewObj VwTextSelection(pvpbox1, 0, 0, false, NULL));
			qsel2.Attach(NewObj VwTextSelection(pvpbox2, 3, 3, false, NULL));
			m_qrootb->MakeRangeSelection(qsel1, qsel2, true, &qselRange);

			// Now loop through all the boxes and make sure we only get boxes from the first column
		}

		// Tests drawing a selection
		void testDrawSelection()
		{
			// Create test data. We simulate a book 4 paragraphs.
			ITsStringPtr qtss;
			StrUni stuPara;
			int hvoPara = khvoParaMin;
			HVO rghvoPara[kcparaStText];
			for (int ipara = 0; ipara < kcparaStText; ipara++)
			{
				stuPara.Format(L"This is paragraph %d", ipara);
				m_qtsf->MakeString(stuPara.Bstr(), g_wsEng, &qtss);
				m_qcda->CacheStringProp(hvoPara, kflidStTxtPara_Contents, qtss);
				// for simplicity, we just reuse the same hvo for the translation, and we also
				// don't set up the connection between StTxtPara and CmTranslation - we make
				// it implicit by using the same hvo.
				m_qcda->CacheStringProp(hvoPara, kflidCmTranslation_Translation, qtss);
				rghvoPara[ipara] = hvoPara;
				hvoPara++;
			}
			m_qcda->CacheVecProp(khvoBook, kflidStText_Paragraphs, rghvoPara, kcparaStText);

			m_qvc.Attach(NewObj SimpleStTextVc());
			m_qrootb->SetRootObject(khvoBook, m_qvc, 1, NULL);
			HRESULT hr = m_qrootb->Layout(m_qvg32, 30);
			unitpp::assert_eq("testDrawSelection Layout didn't succeed", S_OK, hr);

			// Draw a range selection of one character somewhere in the paragraph.
			VwParagraphBox * pvpbox1 = FindParaBoxInsideOf(
				dynamic_cast<VwGroupBox *>(m_qrootb->FirstRealBox()->NextRealBox()));

			DummyVwTextSelectionPtr qsel;
			qsel.Attach(NewObj DummyVwTextSelection(pvpbox1, 0, 1, false));

			int nLineHeight = pvpbox1->FirstBox()->Height();

			// Para starts in the middle of the specified range
			qsel->Draw(m_qvg32, true, Rect(0, 0, 1, 1), Rect(0, 0, 1, 1), pvpbox1->TopToTopOfDocument() - 5,
				nLineHeight * 2);
			unitpp::assert_true("testDrawSelection didn't draw selection if para starts in middle of range",
				qsel->Bounds().Height() > 0);

			// Para ends in the middle of the specified range. The range is on the first line, so use
			// the height of the first box to figure where the vertical range should start.
			VwBox * pboxFirst = pvpbox1->FirstBox();
			qsel->Draw(m_qvg32, true, Rect(0, 0, 1, 1), Rect(0, 0, 1, 1),
				pboxFirst->TopToTopOfDocument() + pboxFirst->Height() - 5,
				nLineHeight * 2);
			unitpp::assert_true("testDrawSelection didn't draw selection if para ends in middle of range",
				qsel->Bounds().Height() > 0);

			// Para starts before and ends after the specified range
			qsel->Draw(m_qvg32, true, Rect(0, 0, 1, 1), Rect(0, 0, 1, 1),
				pvpbox1->TopToTopOfDocument() + 5,
				nLineHeight * 2);
			unitpp::assert_true("testDrawSelection didn't draw selection if para starts before and ends after range",
				qsel->Bounds().Height() > 0);
		}

		// Tests that VwTextSelection::ExtendToStringBoundaries expands the selection to cover an
		// entire paragraph consisting of a single editable string property.
		void testExtendToStringBoundaries_Simple()
		{
			ITsStringPtr qtss;
			// Create test data in a temporary cache.
			// First make some generic objects.
			// Now make two strings, the contents of paragraphs 1.
			StrUni stuPara1(L"This is the first test paragraph");
			CheckHr(m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss));
			CheckHr(m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss));

			// Now make it the paragraph of an StText.
			HVO rghvo[1] = {khvoOrigPara1};
			HVO hvoRoot = 101;
			CheckHr(m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 1));

			m_qvc.Attach(NewObj DummyParaVc());
			CheckHr(m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL));

			// Put insertion point at the beginning of the view
			IVwSelectionPtr qselTemp;
			CheckHr(m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp));
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			m_qzvwsel->m_ichAnchor = 10;
			m_qzvwsel->m_ichEnd = 10;
			m_qzvwsel->m_fAssocPrevious = false;

			CheckHr(m_qzvwsel->ExtendToStringBoundaries());

			unitpp::assert_eq("m_ichAnchor should have moved to the start of the paragraph", 0, m_qzvwsel->m_ichAnchor);
			unitpp::assert_eq("m_ichEnd should have moved to the end of the paragraph", stuPara1.Length(), m_qzvwsel->m_ichEnd);
		}

		// Tests that VwTextSelection::ExtendToStringBoundaries expands the selection to cover
		// the correct text segment when the selection's AssocPrevious is false.
		void testExtendToStringBoundaries_StringBoundary_AssocPrevFalse()
		{
			StrUni stuStyleName(L"Style1");
			ITsStringPtr qtss1, qtss2;
			// Create test data in a temporary cache.
			// First make some generic objects.
			// Now make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is para one");
			CheckHr(m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss1));
			CheckHr(m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1));
			StrUni stuPara2(L"This is para two");
			CheckHr(m_qtsf->MakeString(stuPara2.Bstr(), g_wsEng, &qtss2));
			CheckHr(m_qcda->CacheStringProp(khvoOrigPara2, kflidStTxtPara_Contents, qtss2));

			// Now make it the paragraph of an StText.
			HVO rghvo[2] = {khvoOrigPara1, khvoOrigPara2};
			HVO hvoRoot = 101;
			CheckHr(m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 2));

			m_qvc.Attach(NewObj DummySquishedVc());
			CheckHr(m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL));

			// Put insertion point at the beginning of the view
			IVwSelectionPtr qselTemp;
			CheckHr(m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp));
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			m_qzvwsel->m_ichAnchor = stuPara1.Length();
			m_qzvwsel->m_ichEnd = stuPara1.Length();
			m_qzvwsel->m_fAssocPrevious = false;

			CheckHr(m_qzvwsel->ExtendToStringBoundaries());

			unitpp::assert_eq("m_ichAnchor should have moved to the start of the 2nd string (paragraph)",
				stuPara1.Length(), m_qzvwsel->m_ichAnchor);
			unitpp::assert_eq("m_ichEnd should have moved to the end of the 2nd string (paragraph)",
				stuPara1.Length() + stuPara2.Length(), m_qzvwsel->m_ichEnd);
		}

		// Tests that VwTextSelection::ExtendToStringBoundaries expands the selection to cover
		// the correct text segment when the selection's AssocPrevious is true.
		void testExtendToStringBoundaries_StringBoundary_AssocPrevTrue()
		{
			StrUni stuStyleName(L"Style1");
			ITsStringPtr qtss1, qtss2;
			// Create test data in a temporary cache.
			// First make some generic objects.
			// Now make two strings, the contents of paragraphs 1 and 2.
			StrUni stuPara1(L"This is para one");
			CheckHr(m_qtsf->MakeString(stuPara1.Bstr(), g_wsEng, &qtss1));
			CheckHr(m_qcda->CacheStringProp(khvoOrigPara1, kflidStTxtPara_Contents, qtss1));
			StrUni stuPara2(L"This is para two");
			CheckHr(m_qtsf->MakeString(stuPara2.Bstr(), g_wsEng, &qtss2));
			CheckHr(m_qcda->CacheStringProp(khvoOrigPara2, kflidStTxtPara_Contents, qtss2));

			// Now make it the paragraph of an StText.
			HVO rghvo[2] = {khvoOrigPara1, khvoOrigPara2};
			HVO hvoRoot = 101;
			CheckHr(m_qcda->CacheVecProp(hvoRoot, kflidStText_Paragraphs, rghvo, 2));

			m_qvc.Attach(NewObj DummySquishedVc());
			CheckHr(m_qrootb->SetRootObject(hvoRoot, m_qvc, kfragStText, NULL));

			// Put insertion point at the beginning of the view
			IVwSelectionPtr qselTemp;
			CheckHr(m_qrootb->MakeSimpleSel(true, true, false, true, &qselTemp));
			m_qzvwsel = dynamic_cast<VwTextSelection *>(qselTemp.Ptr());
			unitpp::assert_true("Non-null m_qzvwsel after MakeSimpleSel", m_qzvwsel);
			m_qzvwsel->m_ichAnchor = stuPara1.Length();
			m_qzvwsel->m_ichEnd = stuPara1.Length();
			m_qzvwsel->m_fAssocPrevious = true;

			CheckHr(m_qzvwsel->ExtendToStringBoundaries());

			unitpp::assert_eq("m_ichAnchor should have moved to the start of the paragraph", 0, m_qzvwsel->m_ichAnchor);
			unitpp::assert_eq("m_ichEnd should have moved to the end of the first run", stuPara1.Length(), m_qzvwsel->m_ichEnd);
		}

	public:
		TestVwTextSelection();

		virtual void Setup()
		{
			CreateTestWritingSystemFactory();
			m_qcda.CreateInstance(CLSID_VwCacheDa);
			m_qcda->QueryInterface(IID_ISilDataAccess, (void **)&m_qsda);
			m_qsda->putref_WritingSystemFactory(g_qwsf);
			m_qtsf.CreateInstance(CLSID_TsStrFactory);
			// Now make the root box and view constructor and Graphics object.
			VwRootBox::CreateCom(NULL, CLSID_VwRootBox, (void **)&m_qrootb);
			m_hdc = 0;
			m_qvg32.CreateInstance(CLSID_VwGraphicsWin32);
			m_hdc = ::GetDC(NULL);
			m_qvg32->Initialize(m_hdc);
			m_qrootb->putref_DataAccess(m_qsda);
			m_qdrs.Attach(NewObj DummyRootSite());
			m_rcSrc = Rect(0, 0, 96, 96);
			m_qdrs->SetRects(m_rcSrc, m_rcSrc);
			m_qdrs->SetGraphics(m_qvg32);
			m_qrootb->SetSite(m_qdrs);
			m_qdrs->SetRootBox(m_qrootb);
		}
		virtual void Teardown()
		{
			m_qzvwsel.Clear();
			m_qcda.Clear();
			m_qsda.Clear();
			m_qtsf.Clear();
			m_qvc.Clear();
			m_qvg32->ReleaseDC();
			::ReleaseDC(NULL, m_hdc);
			m_qrootb->Close();
			m_qrootb.Clear();
			m_qvg32.Clear();
			m_qdrs.Clear();
			CloseTestWritingSystemFactory();
		}
	};
}

#endif /*TESTVWSELECTION_H_INCLUDED*/

// Local Variables:
// mode:C++
// compile-command:"cmd.exe /e:4096 /c c:\\FW\\Bin\\mkvw-tst.bat DONTRUN"
// End: (These 4 lines are useful to Steve McConnel.)
