// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2004, SIL International. All Rights Reserved.
// <copyright from='2004' to='2004' company='SIL International'>
//		Copyright (c) 2004, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: HeaderFooterVc.cs
// Responsibility: TE Team
//
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.Common.RootSites;

namespace SIL.FieldWorks.Common.PrintLayout
{
	/// -----------------------------------------------------------------------------------------
	/// <summary>
	/// General-purpose view constructor for laying out headers and footers as three-cell tables
	/// </summary>
	/// -----------------------------------------------------------------------------------------
	public class HeaderFooterVc: VwBaseVc
	{
		#region data members
		/// <summary></summary>
		public static readonly Guid PageNumberGuid = new Guid("644DF48A-3B60-45f4-80C7-739BE6E56A96");
		/// <summary></summary>
		public static readonly Guid FirstReferenceGuid = new Guid("397F43AE-E2B2-4f20-928A-1DF193C07674");
		/// <summary></summary>
		public static readonly Guid LastReferenceGuid = new Guid("85EE15C6-0799-46c6-8769-F9B3CE313AE2");
		/// <summary></summary>
		public static readonly Guid TotalPagesGuid = new Guid("E0EF9EDA-E4E2-4fcf-8720-5BC361BCE110");
		/// <summary></summary>
		public static readonly Guid PrintDateGuid = new Guid("C4556A21-41A8-4675-A74D-59B2C1A7E2B8");
		/// <summary></summary>
		public static readonly Guid DivisionNameGuid = new Guid("2277B85F-47BB-45c9-BC7A-7232E26E901C");
		/// <summary></summary>
		public static readonly Guid PublicationTitleGuid = new Guid("C8136D98-6957-43bd-BEA9-7DCE35200900");
		/// <summary></summary>
		public static readonly Guid PageReferenceGuid = new Guid("8978089A-8969-424e-AE54-B94C554F882D");
		/// <summary></summary>
		public static readonly Guid ProjectNameGuid = new Guid("5610D086-635F-4ae2-8E85-A95896F3D62D");
		/// <summary></summary>
		public static readonly Guid BookNameGuid = new Guid("48C0E5E3-C909-42e1-8F82-3489E3DE96FA");

		/// <summary></summary>
		public const int kfragPageHeaderFooter = 9191;
		/// <summary></summary>
		protected ISilDataAccess m_sda;
		/// <summary></summary>
		protected IPageInfo m_page;
		/// <summary>FDO Cache</summary>
		protected FdoCache m_cache;

		/// <summary>Stores the print date/time for this header/footer so it will remain
		/// consistent across all pages</summary>
		private DateTime m_printDateTime;
		/// <summary>Indicates whether the writing system is right-to-left</summary>
		protected bool m_rightToLeft;
		/// <summary>true to adjust column sizes based on data</summary>
		protected bool m_autoAdjustColumns;
		#endregion

		#region constructor & initialization
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="page">Page info</param>
		/// <param name="wsDefault">default writing system</param>
		/// <param name="printDateTime">Printing date/time</param>
		/// <param name="cache">The cache.</param>
		/// ------------------------------------------------------------------------------------
		public HeaderFooterVc(IPageInfo page, int wsDefault, DateTime printDateTime,
			FdoCache cache)
		{
			m_page = page;
			m_wsDefault = wsDefault;
			m_printDateTime = printDateTime;
			m_cache = cache;
			LgWritingSystem defWs = new LgWritingSystem(m_cache, m_wsDefault);
			RightToLeft = defWs.RightToLeft;
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
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if (disposing)
			{
				// Dispose managed resources here.
			}

			// Dispose unmanaged resources here, whether disposing is true or false.
			m_page = null;
			m_sda = null;

			base.Dispose(disposing);
		}
		#endregion IDisposable override

		///  ----------------------------------------------------------------------------------------
		/// <summary>
		/// Set the data access
		/// </summary>
		/// <param name="sda">data access</param>
		///  ----------------------------------------------------------------------------------------
		public void SetDa(ISilDataAccess sda)
		{
			CheckDisposed();

			m_sda = sda;
		}
		#endregion

		#region Methods and properties you need to override if you want it to work the way you want
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the page number
		/// </summary>
		/// <returns>An ITsString with the page number</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString PageNumber
		{
			get
			{
				CheckDisposed();

				ITsStrBldr strBuilder = TsStrBldrClass.Create();
				ITsPropsBldr propsBldr = TsPropsBldrClass.Create();
				// ENHANCE: support script page numbers
				propsBldr.SetIntPropValues((int)FwTextPropType.ktptWs,
					(int)FwTextPropVar.ktpvDefault,
					m_sda.WritingSystemFactory.UserWs);
				strBuilder.Replace(0, 0, m_page.PageNumber.ToString(), propsBldr.GetTextProps());
				//m_sda.WritingSystemFactory.Shutdown();
				return strBuilder.GetString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the first reference on the page
		/// </summary>
		/// <returns>An ITsString with the first reference on the page</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString FirstReference
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the last reference on the page
		/// </summary>
		/// <returns>An ITsString with the last reference for the page</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString LastReference
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the publication title
		/// </summary>
		/// <returns>An ITsString with the publicatin title</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString PublicationTitle
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the division name
		/// </summary>
		/// <returns>An ITsString with the division name</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString DivisionName
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the total number of pages
		/// </summary>
		/// <returns>An ITsString with the total number of pages</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString TotalPages
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a string that represents the "reference" of the contents of the page (e.g.,
		/// in TE, this would be somethinglike "Mark 2,3").
		/// </summary>
		/// <returns>null</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString PageReference
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the print date for the publication
		/// </summary>
		/// <returns>An ITsString with the print date</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString PrintDate
		{
			get
			{
				CheckDisposed();

				ITsStrBldr strBuilder = TsStrBldrClass.Create();
				ITsPropsBldr propsBldr = TsPropsBldrClass.Create();
				propsBldr.SetIntPropValues((int)FwTextPropType.ktptWs,
					(int)FwTextPropVar.ktpvDefault,
					m_sda.WritingSystemFactory.UserWs);
				strBuilder.Replace(0, 0, m_printDateTime.ToShortDateString(), propsBldr.GetTextProps());
				return strBuilder.GetString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the project name
		/// </summary>
		/// <returns>the language project name</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString ProjectName
		{
			get
			{
				CheckDisposed();

				ITsStrBldr strBuilder = TsStrBldrClass.Create();
				ITsPropsBldr propsBldr = TsPropsBldrClass.Create();
				propsBldr.SetIntPropValues((int)FwTextPropType.ktptWs,
					(int)FwTextPropVar.ktpvDefault,
					m_sda.WritingSystemFactory.UserWs);
				string projectName = m_cache.LangProject.Name.UserDefaultWritingSystem;
				strBuilder.Replace(0, 0, projectName, propsBldr.GetTextProps());
				return strBuilder.GetString();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the book name
		/// </summary>
		/// <returns>the book name as defined by the subclass</returns>
		/// ------------------------------------------------------------------------------------
		public virtual ITsString BookName
		{
			get
			{
				CheckDisposed();
				return null;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the header/footer paragraph style name
		/// </summary>
		/// <remarks>Override this if your application needs to apply special consistent
		/// formatting to all the header and footer elements</remarks>
		/// <returns>style name</returns>
		/// ------------------------------------------------------------------------------------
		protected virtual string HeaderFooterParaStyle
		{
			get { return null; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets/sets whether the writing system is right-to-left.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool RightToLeft
		{
			get
			{
				return m_rightToLeft;
			}
			set
			{
				m_rightToLeft = value;
			}
		}
		#endregion

		#region overridden IVwViewConstructor Members
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get the context-sensitive string for the given guid. Usually this information comes
		/// from the page info.
		/// </summary>
		/// <param name="bstrGuid">String representation of a guid that can be interpreted as
		/// a context-sensitive type of info</param>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public override ITsString GetStrForGuid(string bstrGuid)
		{
			CheckDisposed();

			Guid guid = MiscUtils.GetGuidFromObjData(bstrGuid);

			ITsString tss = null;
			if (guid == HeaderFooterVc.PageNumberGuid)
				tss = PageNumber;
			else if (guid == HeaderFooterVc.FirstReferenceGuid)
				tss = FirstReference;
			else if (guid == HeaderFooterVc.LastReferenceGuid)
				tss = LastReference;
			else if (guid == HeaderFooterVc.PublicationTitleGuid)
				tss = PublicationTitle;
			else if (guid == HeaderFooterVc.PrintDateGuid)
				tss = PrintDate;
			else if (guid == HeaderFooterVc.PageReferenceGuid)
				tss = PageReference;
			else if (guid == HeaderFooterVc.TotalPagesGuid)
				tss = TotalPages;
			else if (guid == HeaderFooterVc.DivisionNameGuid)
				tss = DivisionName;
			else if (guid == HeaderFooterVc.ProjectNameGuid)
				tss = ProjectName;
			else if (guid == HeaderFooterVc.BookNameGuid)
				tss = BookName;

			if (tss == null)
			{
				ITsStrBldr strBuilder = TsStrBldrClass.Create();
				strBuilder.SetIntPropValues(0, 0, (int)FwTextPropType.ktptWs, 0, m_wsDefault);
				tss = strBuilder.GetString();
			}
			return tss;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Display the header or footer as a table containing three cells (left-, center-, and
		/// right-aligned), each with a TS String taken from the given PubHeader
		/// </summary>
		/// <param name="vwenv"></param>
		/// <param name="hvo">id of a PubHeader object</param>
		/// <param name="frag">Constant (ignored)</param>
		/// ------------------------------------------------------------------------------------
		public override void Display(IVwEnv vwenv, int hvo, int frag)
		{
			CheckDisposed();

			// If there is only data in the outside columns, then make a two column
			// table. If there is only data in the center column then make a 1 column
			// table. Otherwise make it three columns.
			int columnCount = 3;
			if (m_autoAdjustColumns)
			{
				if (!DataInOutsideColumns(hvo))
					columnCount = 1;
				else if (!DataInCenterColumn(hvo))
					columnCount = 2;
			}

			VwLength width;
			width.unit = VwUnit.kunPercent100;
			width.nVal = 10000;
			vwenv.OpenTable(columnCount, width, 0, VwAlignment.kvaLeft,
				VwFramePosition.kvfpVoid, VwRule.kvrlNone, 0, 0, false);
			switch(columnCount)
			{
				case 1: width.nVal = 10000; break;
				case 2: width.nVal = 5000; break;
				case 3: width.nVal = 3333; break;
			}
			vwenv.MakeColumns(columnCount, width);
			vwenv.OpenTableBody();
			vwenv.OpenTableRow();

			// Left Column
			if (columnCount > 1)
				AddColumn(vwenv, FwTextAlign.ktalLeft, LeftElementFlid);

			// Center Column
			if (columnCount != 2)
				AddColumn(vwenv, FwTextAlign.ktalCenter, PubHeader.PubHeaderTags.kflidCenteredText);

			// Right Column
			if (columnCount > 1)
				AddColumn(vwenv, FwTextAlign.ktalRight, RightElementFlid);

			vwenv.CloseTableRow();
			vwenv.CloseTableBody();
			vwenv.CloseTable();
		}
		#endregion

		#region Private methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines if there is data in the center column
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool DataInCenterColumn(int hvo)
		{
			ITsString centerString = m_cache.GetTsStringProperty(hvo,
				(int)PubHeader.PubHeaderTags.kflidCenteredText);

			return (centerString != null && centerString.Length > 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Determines if there is data in either of the outside columns
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool DataInOutsideColumns(int hvo)
		{
			ITsString insideString = m_cache.GetTsStringProperty(hvo,
				(int)PubHeader.PubHeaderTags.kflidInsideAlignedText);
			ITsString outsideString = m_cache.GetTsStringProperty(hvo,
				(int)PubHeader.PubHeaderTags.kflidOutsideAlignedText);

			return (insideString != null && insideString.Length > 0) ||
				(outsideString != null && outsideString.Length > 0);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Add a column to the header/footer
		/// </summary>
		/// <param name="vwenv"></param>
		/// <param name="align"></param>
		/// <param name="flid"></param>
		/// ------------------------------------------------------------------------------------
		private void AddColumn(IVwEnv vwenv, FwTextAlign align, PubHeader.PubHeaderTags flid)
		{
			vwenv.OpenTableCell(1, 1);
			if (HeaderFooterParaStyle != null && HeaderFooterParaStyle != string.Empty)
			{
				vwenv.set_StringProperty((int)FwTextPropType.ktptNamedStyle,
					HeaderFooterParaStyle);
			}
			vwenv.set_IntProperty((int)FwTextPropType.ktptAlign,
				(int)FwTextPropVar.ktpvEnum, (int)align);
			vwenv.OpenMappedPara();
			vwenv.AddStringProp((int)flid, this);
			vwenv.CloseParagraph();
			vwenv.CloseTableCell();
		}
		#endregion

		#region page-based properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets information about the current page.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IPageInfo CurrentPage
		{
			get
			{
				CheckDisposed();
				return m_page;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the element that should be in the left cell of this header or footer. This is
		/// either the inside or outside property (flid) of the PubHeader, depending on the page
		/// number and the binding of the publication.
		/// </summary>
		/// <remarks>ENHANCE: Add support for top-bound publications. For now, if we ever had
		/// a top-bound pub (which there's no UI for), the header/footer elements
		/// would have the same layout as left-bound.</remarks>
		/// ------------------------------------------------------------------------------------
		public PubHeader.PubHeaderTags LeftElementFlid
		{
			get
			{
				CheckDisposed();

				// If odd page or simplex binding, right element is outside
				if (m_page.PageNumber % 2 == 1 || m_page.SheetLayout == MultiPageLayout.Simplex)
				{
					return m_page.PublicationBindingSide == BindingSide.Right ?
						PubHeader.PubHeaderTags.kflidOutsideAlignedText :
						PubHeader.PubHeaderTags.kflidInsideAlignedText;
				}
				else // If even page with duplex binding, right element is inside
				{
					return m_page.PublicationBindingSide == BindingSide.Right ?
						PubHeader.PubHeaderTags.kflidInsideAlignedText :
						PubHeader.PubHeaderTags.kflidOutsideAlignedText;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the element that should be in the right cell of this header or footer. This is
		/// either the inside or outside property (flid) of the PubHeader, depending on the page
		/// number and the binding of the publication.
		/// </summary>
		/// <remarks>ENHANCE: Add support for top-bound publications. For now, if we ever had
		/// a top-bound pub (which there's no UI for), the header/footer elements
		/// would have the same layout as left-bound.</remarks>
		/// ------------------------------------------------------------------------------------
		public PubHeader.PubHeaderTags RightElementFlid
		{
			get
			{
				CheckDisposed();

				// If odd page or simplex binding, right element is inside
				if (m_page.PageNumber % 2 == 1 || m_page.SheetLayout == MultiPageLayout.Simplex)
				{
					return m_page.PublicationBindingSide == BindingSide.Right ?
						PubHeader.PubHeaderTags.kflidInsideAlignedText :
						PubHeader.PubHeaderTags.kflidOutsideAlignedText;
				}
				else // If even page with duplex binding, right element is outside
				{
					return m_page.PublicationBindingSide == BindingSide.Right ?
						PubHeader.PubHeaderTags.kflidOutsideAlignedText :
						PubHeader.PubHeaderTags.kflidInsideAlignedText;
				}
			}
		}
		#endregion
	}
}
