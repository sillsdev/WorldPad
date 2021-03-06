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
// File: PublicationControlTests.cs
// Responsibility: TE Team
// ---------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using NMock;
using NMock.Dynamic;
using NUnit.Framework;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;

namespace SIL.FieldWorks.Common.PrintLayout
{
	internal class DummyVwLayoutStream : IVwLayoutStream, IVwRootBox
	{
		#region IVwLayoutStream Members

		public int ColumnHeight(int iColumn)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int ColumnOverlapWithPrevious(int icol)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void CommitLayoutObjects(int hPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DiscardPage(int hPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void LayoutObj(IVwGraphics _vg, int dxsAvailWidth, int ihvoRoot, int cvsli, SelLevInfo[] _rgvsli, int hPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void LayoutPage(IVwGraphics _vg, int dxsAvailWidth, int dysAvailHeight, ref int _ysStartThisPage, int hPage, int nColumns, out int _dysUsedHeight, out int _ysStartNextPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection PageBoundary(int hPage, bool fEnd)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int PageHeight(int hPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int PagePostion(int hPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RollbackLayoutObjects(int hPage)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetManager(IVwLayoutManager _lm)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsInPageAbove(int x, int y, int ysBottomOfPage, SIL.FieldWorks.Common.COMInterfaces.IVwGraphics pvg,
			out int left, out int right)
		{
			left = right = 0;
			return y < ysBottomOfPage; // trivial and perhaps not used?
		}

		#endregion

		#region IVwRootBox Members

		public void Activate(VwSelectionState vss)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void AddSelChngListener(IEventListener _el)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Close()
		{
		}

		public ISilDataAccess DataAccess
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public bool DoSpellCheckStep()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DelSelChngListener(IEventListener _el)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Deserialize(System.Runtime.InteropServices.ComTypes.IStream _strm)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DestroySelection()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DrawRoot(IVwGraphics _vg, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst, bool fDrawSel)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DrawRoot2(IVwGraphics _vg, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst, bool fDrawSel, int ysTop, int dysHeight)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DrawingErrors()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void FlashInsertionPoint()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void GetRootObject(out int _hvo, out IVwViewConstructor _pvwvc, out int _frag, out IVwStylesheet _pss)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public object GetRootVariant()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int GetTotalPrintPages(IVwPrintContext _vpc, IAdvInd3 _advi3)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int Height
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public void InitializePrinting(IVwPrintContext _vpc)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsDirty()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Layout(IVwGraphics _vg, int dxsAvailWidth)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool LoseFocus()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeRangeSelection(IVwSelection _selAnchor, IVwSelection _selEnd, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeSelAt(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeSelInBox(IVwSelection _selInit, bool fEndPoint, int iLevel, int iBox, bool fInitial, bool fRange, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeSelInObj(int ihvoRoot, int cvsli, SelLevInfo[] _rgvsli, int tag, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeSimpleSel(bool fInitial, bool fEdit, bool fRange, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeTextSelInObj(int ihvoRoot, int cvsli, SelLevInfo[] _rgvsli, int cvsliEnd, SelLevInfo[] _rgvsliEnd, bool fInitial, bool fEdit, bool fRange, bool fWholeObj, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection MakeTextSelection(int ihvoRoot, int cvlsi, SelLevInfo[] _rgvsli, int tagTextProp, int cpropPrevious, int ichAnchor, int ichEnd, int ws, bool fAssocPrev, int ihvoEnd, ITsTextProps _ttpIns, bool fInstall)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void MouseDblClk(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void MouseDown(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void MouseDownExtended(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void MouseMoveDrag(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void MouseUp(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void OnChar(int chw)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int OnExtendedKey(int chw, VwShiftStatus ss, int nFlags)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void OnStylesheetChange()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void OnSysChar(int chw)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void OnTyping(IVwGraphics _vg, string bstrInput, int cchBackspace, int cchDelForward, ushort chFirst, ref int _wsPending)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwOverlay Overlay
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public VwPrepDrawResult PrepareToDraw(IVwGraphics _vg, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Print(IVwPrintContext _vpc, IAdvInd3 _advi3)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void PrintSinglePage(IVwPrintContext _vpc, int nPageNo)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void PropChanged(int hvo, int tag, int ivMin, int cvIns, int cvDel)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Reconstruct()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RequestObjCharDeleteNotification(IVwNotifyObjCharDeletion _nocd)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwSelection Selection
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public VwSelectionState SelectionState
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public void Serialize(System.Runtime.InteropServices.ComTypes.IStream _strm)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetKeyboardForWs(IWritingSystem _ws, ref string _bstrActiveKeymanKbd, ref int _nActiveLangId, ref int _hklActive, ref bool _fSelectLangPending)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetRootObject(int hvo, IVwViewConstructor _vwvc, int frag, IVwStylesheet _ss)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetRootObjects(int[] _rghvo, IVwViewConstructor[] _rgpvwvc, int[] _rgfrag, IVwStylesheet _ss, int chvo)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetRootString(ITsString _tss, IVwStylesheet _ss, IVwViewConstructor _vwvc, int frag)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetRootVariant(object v, IVwStylesheet _ss, IVwViewConstructor _vwvc, int frag)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetSite(IVwRootSite _vrs)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetTableColWidths(VwLength[] _rgvlen, int cvlen)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVwRootSite Site
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public IVwStylesheet Stylesheet
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public int Width
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public void WriteWpx(System.Runtime.InteropServices.ComTypes.IStream _strm)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int XdPos
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool get_IsClickInObject(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst, out int _odt)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool get_IsClickInOverlayTag(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc1, SIL.FieldWorks.Common.Utils.Rect rcDst1, out int _iGuid, out string _bstrGuids, out SIL.FieldWorks.Common.Utils.Rect _rcTag, out SIL.FieldWorks.Common.Utils.Rect _rcAllTags, out bool _fOpeningTag)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool get_IsClickInText(int xd, int yd, SIL.FieldWorks.Common.Utils.Rect rcSrc, SIL.FieldWorks.Common.Utils.Rect rcDst)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int MaxParasToScan
		{
			get { throw new Exception("The MaxParasToScan property is not implemented."); }
			set { throw new Exception("The MaxParasToScan property is not implemented."); }
		}

		public bool IsCompositionInProgress
		{
			get { throw new NotImplementedException("The method or operation is not implemented."); }
		}
		#endregion
	}

	internal class GoofyDivision : DummyDivision
	{
		internal int m_overrideHeight = 0;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:GoofyDivision"/> class.
		/// </summary>
		/// <param name="stream">The main layout stream.</param>
		/// ------------------------------------------------------------------------------------
		public GoofyDivision(DummyVwLayoutStream stream) : base(null, 1)
		{
			m_mainLayoutStream = stream;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a current best estimate of the total height of the data in the main and any
		/// subordinate views, given the specified width in pixels. When all pages have been
		/// laid out at the given width, this should return the actual height.
		/// </summary>
		/// <param name="dxpWidth">Width of one column</param>
		/// ------------------------------------------------------------------------------------
		public override int EstimateHeight(int dxpWidth)
		{
			return m_overrideHeight;
		}


		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Add the page element.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="dysSpaceUsedOnPage">The amount of vertical space taken up by this
		/// element on this page.</param>
		/// <param name="currentColumn">The 1-based index of the current column.</param>
		/// <param name="numberColumns">The total number columns for the stream.</param>
		/// <param name="leftMargin">The left margin.</param>
		/// <param name="offsetFromTopOfDiv">The offset from top of division.</param>
		/// <param name="columnHeight">The height of the column.</param>
		/// ------------------------------------------------------------------------------------
		internal void CallAddElement(Page page, int dysSpaceUsedOnPage,
			int currentColumn, int numberColumns, int leftMargin,
			int offsetFromTopOfDiv, int columnHeight)
		{
			base.AddElement(page, dysSpaceUsedOnPage, currentColumn, numberColumns, leftMargin,
				offsetFromTopOfDiv, columnHeight, 0);
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class PublicationControlTests
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test ability to adjust the scroll range based on expanding lazy boxes.
		/// This tests the scenario where the very first lazy box is being expanded during
		/// layout before any elements have been added to any pages.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AdjustScrollRange_GrowFirstDivOnPage_NoElements()
		{
			DummyVwLayoutStream stream = new DummyVwLayoutStream();
			GoofyDivision divMgr = new GoofyDivision(stream);
			DummyPublication pubCtrl = new DummyPublication(null, divMgr, DateTime.Now);
			pubCtrl.PageHeight = 72000 * 6;
			divMgr.m_overrideHeight = divMgr.AvailablePageHeightInPrinterPixels;
			pubCtrl.CreatePages();
			Assert.AreEqual(2, pubCtrl.PageCount);
			Assert.AreEqual(0, pubCtrl.Pages[0].PageElements.Count);
			Assert.AreEqual(0, pubCtrl.Pages[1].PageElements.Count);
			Point origScrollPos = pubCtrl.ScrollPosition;
			Size origScrollSize = pubCtrl.AutoScrollMinSize;
			pubCtrl.AdjustScrollRange(stream, 0, 0, 300, 0);
			Assert.AreEqual(2, pubCtrl.PageCount);
			Assert.AreEqual(origScrollPos, pubCtrl.ScrollPosition);
			Assert.AreEqual(origScrollSize, pubCtrl.AutoScrollMinSize);
			Assert.AreEqual(0, pubCtrl.Pages[0].PageElements.Count);
			Assert.AreEqual(0, pubCtrl.Pages[1].PageElements.Count);
			Assert.IsTrue(pubCtrl.Pages[0].NeedsLayout);
			Assert.IsTrue(pubCtrl.Pages[1].NeedsLayout);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test ability to adjust the scroll range based on expanding lazy boxes.
		/// This tests the scenario where an additional lazy box is being expanded, causing the
		/// size of the stream (on page 1) to grow such that the offset on the following page is
		/// increased.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AdjustScrollRange_GrowFirstDivOnPage()
		{
			DummyVwLayoutStream stream = new DummyVwLayoutStream();
			GoofyDivision divMgr = new GoofyDivision(stream);
			DummyPublication pubCtrl = new DummyPublication(null, divMgr, DateTime.Now);
			pubCtrl.PageHeight = 72000 * 6;
			divMgr.m_overrideHeight = divMgr.AvailablePageHeightInPrinterPixels;
			pubCtrl.CreatePages();
			Assert.AreEqual(2, pubCtrl.PageCount);
			divMgr.CallAddElement(pubCtrl.Pages[0], divMgr.m_overrideHeight - 20,
				1, 1, divMgr.InsideMarginInPrinterPixels, 20, divMgr.AvailablePageHeightInPrinterPixels);
			int origTopOfPageTwo = divMgr.m_overrideHeight - 20;
			divMgr.CallAddElement(pubCtrl.Pages[1], 20, 1, 1, divMgr.OutsideMarginInPrinterPixels,
				origTopOfPageTwo, divMgr.AvailablePageHeightInPrinterPixels);
			Point origScrollPos = pubCtrl.ScrollPosition;
			Size origScrollSize = pubCtrl.AutoScrollMinSize;
			pubCtrl.AdjustScrollRange(stream, 0, 0, 300, origTopOfPageTwo - 2000);
			Assert.AreEqual(2, pubCtrl.PageCount);
			Assert.AreEqual(origScrollPos, pubCtrl.ScrollPosition);
			Assert.AreEqual(origScrollSize, pubCtrl.AutoScrollMinSize);
			Assert.AreEqual(20, pubCtrl.Pages[0].PageElements[0].OffsetToTopPageBoundary);
			Assert.AreEqual(origTopOfPageTwo + 300, pubCtrl.Pages[1].PageElements[0].OffsetToTopPageBoundary);
			Assert.IsFalse(pubCtrl.Pages[0].NeedsLayout);
			Assert.IsTrue(pubCtrl.Pages[1].NeedsLayout);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test ability to adjust the scroll range based on expanding lazy boxes.
		/// This tests the scenario where a lazy box is being expanded, causing the
		/// size of the stream to shrink so much that a page is deleted.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AdjustScrollRange_ShrinkFirstDivOnPage()
		{
			DummyVwLayoutStream stream = new DummyVwLayoutStream();
			GoofyDivision divMgr = new GoofyDivision(stream);
			DummyPublication pubCtrl = new DummyPublication(null, divMgr, DateTime.Now);
			pubCtrl.PageHeight = 72000 * 6;
			divMgr.m_overrideHeight = divMgr.AvailablePageHeightInPrinterPixels * 2;
			pubCtrl.CreatePages();
			Assert.AreEqual(3, pubCtrl.PageCount);
			Page pg1 = pubCtrl.Pages[0];
			Page pg2 = pubCtrl.Pages[1];
			Page pg3 = pubCtrl.Pages[2];
			divMgr.CallAddElement(pg1, divMgr.AvailablePageHeightInPrinterPixels - 20,
				1, 1, divMgr.InsideMarginInPrinterPixels, 20, divMgr.AvailablePageHeightInPrinterPixels);
			int origTopOfPageTwo = divMgr.AvailablePageHeightInPrinterPixels - 40;
			divMgr.CallAddElement(pg2, divMgr.AvailablePageHeightInPrinterPixels - 20,
				1, 1, divMgr.OutsideMarginInPrinterPixels, origTopOfPageTwo, divMgr.AvailablePageHeightInPrinterPixels);
			int origTopOfPageThree = divMgr.m_overrideHeight - 60;
			divMgr.CallAddElement(pg3, 40, 1, 1, divMgr.InsideMarginInPrinterPixels,
				origTopOfPageThree, divMgr.AvailablePageHeightInPrinterPixels);
			foreach (Page pg in pubCtrl.Pages)
				Assert.AreEqual(pg.PageElements[0].OffsetToTopPageBoundary, pg.OffsetFromTopOfDiv(divMgr));
			Point origScrollPos = pubCtrl.ScrollPosition;
			Size origScrollSize = pubCtrl.AutoScrollMinSize;
			// The following should cause the original page 2 to be deleted and adjust the top of
			// the original page 3 (now page 2) to be the same as the original top of page 2.
			pubCtrl.AdjustScrollRange(stream, 0, 0, -divMgr.AvailablePageHeightInPrinterPixels,
				origTopOfPageTwo - 2000);
			Assert.AreEqual(2, pubCtrl.PageCount);
			Assert.AreEqual(origScrollPos, pubCtrl.ScrollPosition);
			Assert.AreEqual(origScrollSize.Height * 2 / 3, pubCtrl.AutoScrollMinSize.Height);
			Assert.AreEqual(20, pg1.PageElements[0].OffsetToTopPageBoundary);
			Assert.AreEqual(pg3, pubCtrl.Pages[1]);
			Assert.AreEqual(origTopOfPageThree - divMgr.AvailablePageHeightInPrinterPixels,
				pg3.PageElements[0].OffsetToTopPageBoundary);
			Assert.AreEqual(40, pg3.PageElements[0].LocationOnPage.Height);
			Assert.IsFalse(pg1.NeedsLayout);
			Assert.IsTrue(pg3.NeedsLayout);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test ability to adjust the scroll range based on expanding lazy boxes.
		/// This tests the scenario where the first lazy box in the second division (on page 2)
		/// is being expanded (real size > original estimate). This should not affect the
		/// offset into that stream. It should add an additional page.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AdjustScrollRange_GrowSecondDivOnPageTwo()
		{
			DummyVwLayoutStream stream1 = new DummyVwLayoutStream();
			DummyVwLayoutStream stream2 = new DummyVwLayoutStream();
			GoofyDivision divMgr1 = new GoofyDivision(stream1);
			GoofyDivision divMgr2 = new GoofyDivision(stream2);
			DummyPublication pubCtrl = new DummyPublication(null, divMgr1, DateTime.Now);
			pubCtrl.PageHeight = 72000 * 6;
			pubCtrl.AddDivision(divMgr2);
			divMgr1.m_overrideHeight = divMgr1.AvailablePageHeightInPrinterPixels;
			divMgr2.m_overrideHeight = divMgr2.AvailablePageHeightInPrinterPixels;
			divMgr2.StartAt = DivisionStartOption.Continuous;
			pubCtrl.CreatePages();
			Assert.AreEqual(3, pubCtrl.PageCount);
			divMgr1.CallAddElement(pubCtrl.Pages[0], divMgr1.m_overrideHeight - 20, 1, 1,
				divMgr1.InsideMarginInPrinterPixels, 20, divMgr1.AvailablePageHeightInPrinterPixels);
			int origTopOfPageTwoDiv1 = divMgr1.m_overrideHeight - 20;
			divMgr1.CallAddElement(pubCtrl.Pages[1], 20, 1, 1, divMgr1.OutsideMarginInPrinterPixels,
				origTopOfPageTwoDiv1, divMgr1.AvailablePageHeightInPrinterPixels);
			Page origPg3 = pubCtrl.Pages[2];
			int origTopOfPageThreeDiv2 = origPg3.OffsetFromTopOfDiv(divMgr2);
			Point origScrollPos = pubCtrl.ScrollPosition;
			Size origScrollSize = pubCtrl.AutoScrollMinSize;
			pubCtrl.AdjustScrollRange(stream2, 0, 0, divMgr2.AvailablePageHeightInPrinterPixels, 0);
			Assert.AreEqual(4, pubCtrl.PageCount);
			Assert.IsFalse(pubCtrl.Pages[0].NeedsLayout);
			Assert.IsFalse(pubCtrl.Pages[1].NeedsLayout,
				"This test simulates a call to AdjustScrollRange that happens in the middle of laying out this page, so it should not get flagged as needing layout.");
			Assert.IsTrue(pubCtrl.Pages[2].NeedsLayout);
			Assert.IsTrue(pubCtrl.Pages[3].NeedsLayout);
			Assert.AreEqual(origScrollPos, pubCtrl.ScrollPosition);
			Assert.AreEqual(origScrollSize.Height * 4 / 3, pubCtrl.AutoScrollMinSize.Height);
			Assert.AreEqual(1, pubCtrl.Pages[0].PageElements.Count);
			Assert.AreEqual(20, pubCtrl.Pages[0].PageElements[0].OffsetToTopPageBoundary);
			Assert.AreEqual(1, pubCtrl.Pages[1].PageElements.Count);
			Assert.AreEqual(origTopOfPageTwoDiv1, pubCtrl.Pages[1].OffsetFromTopOfDiv(divMgr1));
			Assert.AreEqual(0, pubCtrl.Pages[1].OffsetFromTopOfDiv(divMgr2));
			Assert.AreEqual(origPg3, pubCtrl.Pages[3],
				"A page should have been inserted between the original page 2 and page 3.");
			Assert.AreEqual(0, pubCtrl.Pages[2].PageElements.Count);
			Assert.AreEqual(divMgr2.AvailablePageHeightInPrinterPixels,
				pubCtrl.Pages[2].OffsetFromTopOfDiv(divMgr2),
				"Inserted page's offset from top of division 2 should be one ideal page from the top.");
			Assert.AreEqual(0, pubCtrl.Pages[3].PageElements.Count);
			Assert.AreEqual(origTopOfPageThreeDiv2 + divMgr2.AvailablePageHeightInPrinterPixels,
				pubCtrl.Pages[3].OffsetFromTopOfDiv(divMgr2));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test ability to adjust the scroll range based on expanding lazy boxes in the
		/// subordinate stream.
		/// This tests the scenario where an additional lazy box is being expanded, causing the
		/// size of the footnote stream (on page 1) to grow such that the offset on the
		/// following page is increased. This should NOT change the value of
		/// m_ypOffsetFromTopOfDiv for the page.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void AdjustScrollRange_GrowFootnoteStream()
		{
			DummyVwLayoutStream stream = new DummyVwLayoutStream();
			DummyVwLayoutStream footnoteStream = new DummyVwLayoutStream();
			GoofyDivision divMgr = new GoofyDivision(stream);
			DummyPublication pubCtrl = new DummyPublication(null, divMgr, DateTime.Now);
			pubCtrl.PageHeight = 72000 * 6;
			divMgr.m_overrideHeight = divMgr.AvailablePageHeightInPrinterPixels;
			pubCtrl.CreatePages();
			Assert.AreEqual(2, pubCtrl.PageCount);
			divMgr.CallAddElement(pubCtrl.Pages[0], divMgr.m_overrideHeight - 100,
				1, 1, divMgr.InsideMarginInPrinterPixels, 20, divMgr.AvailablePageHeightInPrinterPixels);
			int origTopOfPageTwo = divMgr.m_overrideHeight - 100;
			((DummyPage)pubCtrl.Pages[0]).CallAddPageElement(divMgr, footnoteStream, false,
				new Rectangle(divMgr.InsideMarginInPrinterPixels, divMgr.m_overrideHeight - 100, divMgr.AvailablePageWidthInPrinterPixels, 100),
				0, false, 1, 1, 0, 100, false, true);
			divMgr.CallAddElement(pubCtrl.Pages[1], 100, 1, 1, divMgr.OutsideMarginInPrinterPixels,
				origTopOfPageTwo, divMgr.AvailablePageHeightInPrinterPixels);
			((DummyPage)pubCtrl.Pages[1]).CallAddPageElement(divMgr, footnoteStream, false,
				new Rectangle(divMgr.InsideMarginInPrinterPixels, divMgr.m_overrideHeight - 100, divMgr.AvailablePageWidthInPrinterPixels, 100),
				101, false, 1, 1, 0, 100, false, true);
			Point origScrollPos = pubCtrl.ScrollPosition;
			Size origScrollSize = pubCtrl.AutoScrollMinSize;
			pubCtrl.AdjustScrollRange(footnoteStream, 0, 0, 300, 95);
			Assert.AreEqual(2, pubCtrl.PageCount);
			Assert.AreEqual(origScrollPos, pubCtrl.ScrollPosition);
			Assert.AreEqual(origScrollSize, pubCtrl.AutoScrollMinSize);
			Assert.AreEqual(0, pubCtrl.Pages[0].PageElements[1].OffsetToTopPageBoundary);
			Assert.AreEqual(401, pubCtrl.Pages[1].PageElements[1].OffsetToTopPageBoundary);
			Assert.AreEqual(20, pubCtrl.Pages[0].OffsetFromTopOfDiv(divMgr));
			Assert.AreEqual(origTopOfPageTwo, pubCtrl.Pages[1].OffsetFromTopOfDiv(divMgr));
		}

	}
}
