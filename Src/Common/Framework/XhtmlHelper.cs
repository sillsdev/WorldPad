using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.LangProj;
using SIL.Utils;

namespace SIL.FieldWorks.Common.Framework
{
	/// <summary>
	/// This class contains method that are useful for XHTML export in both Flex and TE.
	/// </summary>
	public class XhtmlHelper
	{
		/// <summary>
		/// List the possible types of Cascading Stylesheet that we can produce.
		/// </summary>
		public enum CssType
		{
			/// <summary>Dictionary type display</summary>
			Dictionary,
			/// <summary>Scripture type display</summary>
			Scripture
		};
		class ExportStyleInfo : BaseStyleInfo
		{
			public ExportStyleInfo(IStStyle style)
				: base(style)
			{
			}

			public new TriStateBool DirectionIsRightToLeft
			{
				get
				{
					if (m_rtl.ValueIsSet)
						return m_rtl.Value;
					else
						return TriStateBool.triNotSet;
				}
			}

			public bool HasKeepTogether
			{
				get { return m_keepTogether.ValueIsSet; }
			}

			public bool HasKeepWithNext
			{
				get { return m_keepWithNext.ValueIsSet; }
			}

			public bool HasWidowOrphanControl
			{
				get { return m_widowOrphanControl.ValueIsSet; }
			}

			public bool HasAlignment
			{
				get { return m_alignment.ValueIsSet; }
			}

			public bool HasLineSpacing
			{
				get { return m_lineSpacing.ValueIsSet; }
			}

			public bool HasSpaceBefore
			{
				get { return m_spaceBefore.ValueIsSet; }
			}

			public bool HasSpaceAfter
			{
				get { return m_spaceAfter.ValueIsSet; }
			}

			public bool HasFirstLineIndent
			{
				get { return m_firstLineIndent.ValueIsSet; }
			}

			public bool HasLeadingIndent
			{
				get { return m_leadingIndent.ValueIsSet; }
			}

			public bool HasTrailingIndent
			{
				get { return m_trailingIndent.ValueIsSet; }
			}

			public bool HasBorder
			{
				get { return m_border.ValueIsSet; }
			}

			public bool HasBorderColor
			{
				get { return m_borderColor.ValueIsSet; }
			}

			public string InheritsFrom
			{
				get { return m_basedOnStyleName; }
			}
		}

		StyleInfoTable m_styleTable;
		SortedDictionary<string, List<string>> m_dictClassData = new SortedDictionary<string, List<string>>();
		SortedDictionary<string, XmlNode> m_mapCssClassToXnode = new SortedDictionary<string, XmlNode>();
		SortedDictionary<string, List<string>> m_mapCssToStyleEnv = new SortedDictionary<string, List<string>>();

		private TextWriter m_writer;
		private FdoCache m_cache;
		private bool m_fRTL = false;
		private int m_cColumns = 2;		// default for dictionary output.

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="cache"></param>
		public XhtmlHelper(TextWriter w, FdoCache cache)
		{
			m_writer = w;
			m_cache = cache;

			ILgWritingSystem lgws = LgWritingSystem.CreateFromDBObject(cache, cache.DefaultVernWs);
			m_fRTL = lgws.RightToLeft;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="node"></param>
		public void MapCssClassToXmlNode(string cssClass, XmlNode node)
		{
			m_mapCssClassToXnode.Add(cssClass, node);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool MapCssClassToXmlNode(string cssClass, out XmlNode node)
		{
			return m_mapCssClassToXnode.TryGetValue(cssClass, out node);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="envirs"></param>
		public void MapCssToStyleEnv(string cssClass, List<string> envirs)
		{
			m_mapCssToStyleEnv.Add(cssClass, envirs);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="envirs"></param>
		/// <returns></returns>
		public bool MapCssToStyleEnv(string cssClass, out List<string> envirs)
		{
			return m_mapCssToStyleEnv.TryGetValue(cssClass, out envirs);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="rgsLangs"></param>
		public void MapCssToLangs(string cssClass, List<string> rgsLangs)
		{
			m_dictClassData.Add(cssClass, rgsLangs);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="sLang"></param>
		public void MapCssToLang(string cssClass, string sLang)
		{
			List<string> rgsLangs;
			if (!m_dictClassData.TryGetValue(cssClass, out rgsLangs))
			{
				rgsLangs = new List<string>();
				m_dictClassData.Add(cssClass, rgsLangs);
			}
			if (!rgsLangs.Contains(sLang))
				rgsLangs.Add(sLang);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="cssClass"></param>
		/// <param name="rgsLangs"></param>
		/// <returns></returns>
		public bool MapCssToLangs(string cssClass, out List<string> rgsLangs)
		{
			return m_dictClassData.TryGetValue(cssClass, out rgsLangs);
		}

		/// <summary>
		/// Write the initial lines of an XHTML output file.
		/// </summary>
		/// <param name="sOutPath"></param>
		/// <param name="sDescription"></param>
		/// <param name="sBodyClass"></param>
		public void WriteXhtmlHeading(string sOutPath, string sDescription, string sBodyClass)
		{
			string sOutFile = Path.GetFileName(sOutPath);
			m_writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			m_writer.WriteLine("<html xml:lang=\"utf-8\" lang=\"utf-8\">");
			WriteWritingSystemInfo();
			m_writer.WriteLine("<head>");
			m_writer.WriteLine("<title/>");
			m_writer.WriteLine("<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\"/>",
				XmlUtils.MakeSafeXmlAttribute(Path.ChangeExtension(sOutFile, ".css")));
			m_writer.WriteLine("<meta name=\"extLinkRootDir\" content=\"{0}\"/>",
				XmlUtils.MakeSafeXmlAttribute(m_cache.LangProject.ExternalLinkRootDir));
			if (!String.IsNullOrEmpty(sDescription))
				m_writer.WriteLine("<meta name=\"description\" content=\"{0}\"/>", XmlUtils.MakeSafeXmlAttribute(sDescription));
			m_writer.WriteLine("<meta name=\"filename\" content=\"{0}\"/>", XmlUtils.MakeSafeXmlAttribute(sOutFile));
			m_writer.WriteLine("</head>");
			m_writer.WriteLine("<body class=\"{0}\">", sBodyClass);
		}

		/// <summary>
		/// Write the final lines of an XHTML output file.
		/// </summary>
		public void WriteXhtmlEnding()
		{
			//if (m_chCurrent != '\0')
			//    m_writer.WriteLine("</div>");	// for letData
			m_writer.WriteLine("</body>");
			m_writer.WriteLine("</html>");
		}

		private void WriteWritingSystemInfo()
		{
			foreach (ILgWritingSystem lgws in m_cache.LanguageEncodings)
			{
				m_writer.Write("<WritingSystemInfo lang=\"{0}\" dir=\"{1}\"",
					lgws.RFC4646bis, lgws.RightToLeft ? "rtl" : "ltr");
				WriteWsListTag(lgws.Hvo,
					m_cache.LangProject.CurVernWssRS.HvoArray, "vernTag");
				WriteWsListTag(lgws.Hvo,
					m_cache.LangProject.CurAnalysisWssRS.HvoArray, "analTag");
				WriteWsListTag(lgws.Hvo,
					m_cache.LangProject.CurPronunWssRS.HvoArray, "pronTag");
				if (!String.IsNullOrEmpty(lgws.DefaultSerif))
					m_writer.Write(" font=\"{0}\"", XmlUtils.MakeSafeXmlAttribute(lgws.DefaultSerif));
				else if (!String.IsNullOrEmpty(lgws.DefaultBodyFont))
					m_writer.Write(" font=\"{0}\"", XmlUtils.MakeSafeXmlAttribute(lgws.DefaultBodyFont));
				else if (!String.IsNullOrEmpty(lgws.DefaultSansSerif))
					m_writer.Write(" font=\"{0}\"", XmlUtils.MakeSafeXmlAttribute(lgws.DefaultSansSerif));
				else if (!String.IsNullOrEmpty(lgws.DefaultMonospace))
					m_writer.Write(" font=\"{0}\"", XmlUtils.MakeSafeXmlAttribute(lgws.DefaultMonospace));
				m_writer.WriteLine("/>");
			}
		}

		private void WriteWsListTag(int ws, int[] rgws, string sAttr)
		{
			for (int i = 0; i < rgws.Length; ++i)
			{
				if (ws == rgws[i])
				{
					string sTag = String.Empty;
					if (i > 0)
						sTag = String.Format("_L{0}", i + 1);
					m_writer.Write(" {0}=\"{1}\"", sAttr, sTag);
				}
			}
		}

		/// <summary>
		/// Write a Cascading Style Sheet file based on the layouts accumulated in
		/// m_dictCssXnode and the given stylesheet.
		/// </summary>
		/// <param name="sOutputFile"></param>
		/// <param name="vss"></param>
		/// <param name="type"></param>
		/// <param name="pub"></param>
		public void WriteCssFile(string sOutputFile, IVwStylesheet vss, CssType type,
			IPublication pub)
		{
			// Read all the style information from the database.
			ReadStyles(vss);
			// Write the Cascading Style Sheet.
			using (m_writer = new StreamWriter(sOutputFile))
			{
				m_writer.WriteLine("/* Cascading Style Sheet generated by {0} version {1} on {2} {3} */",
					System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
					System.Reflection.Assembly.GetEntryAssembly().GetName().Version,
					DateTime.Now.ToLongDateString(), DateTime.Now.ToShortTimeString());
				m_writer.WriteLine("@page {");
				m_writer.WriteLine("    counter-increment: page;");
				if (pub != null)
				{
					//pub.BaseFontSize;
					//pub.BaseLineSpacing;
					//pub.BindingEdge;
					//pub.GutterLoc;
					//pub.GutterMargin;
					//pub.PageHeight;
					//pub.PageWidth;
					//pub.SheetLayout;
					//pub.SheetsPerSig;
					if (pub.PaperHeight != 0 && pub.PaperWidth != 0)
					{
						m_writer.WriteLine("    height: {0}pt;", ConvertMptToPt(pub.PaperHeight));
						m_writer.WriteLine("    width: {0}pt;", ConvertMptToPt(pub.PaperWidth));
					}
					else if (pub.IsLandscape)
					{
						m_writer.WriteLine("    height: 8.5in;");
						m_writer.WriteLine("    width: 11in;");
					}
					else
					{
						m_writer.WriteLine("    height: 11in;");
						m_writer.WriteLine("    width: 8.5in;");
					}
					if (pub.DivisionsOS.Count > 0)
					{
						m_writer.WriteLine("    margin: {0}pt {1}pt {2}pt {3}pt;",
							ConvertMptToPt(pub.DivisionsOS[0].PageLayoutOA.MarginTop),
							ConvertMptToPt(pub.IsLeftBound ?
								pub.DivisionsOS[0].PageLayoutOA.MarginOutside :
								pub.DivisionsOS[0].PageLayoutOA.MarginInside),
							ConvertMptToPt(pub.DivisionsOS[0].PageLayoutOA.MarginBottom),
							ConvertMptToPt(pub.IsLeftBound ?
								pub.DivisionsOS[0].PageLayoutOA.MarginInside :
								pub.DivisionsOS[0].PageLayoutOA.MarginOutside));
						string sInside = pub.IsLeftBound ? "left" : "right";
						string sOutside = pub.IsLeftBound ? "right" : "left";
						WriteHeaderFooterBlock("top-" + sInside, pub.DivisionsOS[0].HFSetOA.DefaultHeaderOA.InsideAlignedText.UnderlyingTsString);
						WriteHeaderFooterBlock("top-center", pub.DivisionsOS[0].HFSetOA.DefaultHeaderOA.CenteredText.UnderlyingTsString);
						WriteHeaderFooterBlock("top-" + sOutside, pub.DivisionsOS[0].HFSetOA.DefaultHeaderOA.OutsideAlignedText.UnderlyingTsString);
						WriteHeaderFooterBlock("bottom-" + sInside, pub.DivisionsOS[0].HFSetOA.DefaultFooterOA.InsideAlignedText.UnderlyingTsString);
						WriteHeaderFooterBlock("bottom-center", pub.DivisionsOS[0].HFSetOA.DefaultFooterOA.CenteredText.UnderlyingTsString);
						WriteHeaderFooterBlock("bottom-"+sOutside, pub.DivisionsOS[0].HFSetOA.DefaultFooterOA.OutsideAlignedText.UnderlyingTsString);
						//pub.DivisionsOS[0].PageLayoutOA.PosFooter;
						//pub.DivisionsOS[0].PageLayoutOA.PosHeader;
						//pub.DivisionsOS[0].StartAt;
						WriteFootnotesBlock(type);
						if (pub.DivisionsOS[0].DifferentEvenHF)
						{
							m_writer.WriteLine("}");
							if (pub.IsLeftBound)
								m_writer.WriteLine("@page :left {");
							else
								m_writer.WriteLine("@page :right {");
							WriteHeaderFooterBlock("top-" + sInside, pub.DivisionsOS[0].HFSetOA.EvenHeaderOA.OutsideAlignedText.UnderlyingTsString);
							WriteHeaderFooterBlock("top-center", pub.DivisionsOS[0].HFSetOA.EvenHeaderOA.CenteredText.UnderlyingTsString);
							WriteHeaderFooterBlock("top-" + sOutside, pub.DivisionsOS[0].HFSetOA.EvenHeaderOA.InsideAlignedText.UnderlyingTsString);
							WriteHeaderFooterBlock("bottom-" + sInside, pub.DivisionsOS[0].HFSetOA.EvenFooterOA.OutsideAlignedText.UnderlyingTsString);
							WriteHeaderFooterBlock("bottom-center", pub.DivisionsOS[0].HFSetOA.EvenFooterOA.CenteredText.UnderlyingTsString);
							WriteHeaderFooterBlock("bottom-" + sOutside, pub.DivisionsOS[0].HFSetOA.EvenFooterOA.InsideAlignedText.UnderlyingTsString);
						}
						if (pub.DivisionsOS[0].DifferentFirstHF)
						{
							m_writer.WriteLine("}");
							m_writer.WriteLine("@page :first {");
							WriteHeaderFooterBlock("top-" + sInside, pub.DivisionsOS[0].HFSetOA.FirstHeaderOA.InsideAlignedText.UnderlyingTsString);
							WriteHeaderFooterBlock("top-center", pub.DivisionsOS[0].HFSetOA.FirstHeaderOA.CenteredText.UnderlyingTsString);
							WriteHeaderFooterBlock("top-" + sOutside, pub.DivisionsOS[0].HFSetOA.FirstHeaderOA.OutsideAlignedText.UnderlyingTsString);
							WriteHeaderFooterBlock("bottom-" + sInside, pub.DivisionsOS[0].HFSetOA.FirstFooterOA.InsideAlignedText.UnderlyingTsString);
							WriteHeaderFooterBlock("bottom-center", pub.DivisionsOS[0].HFSetOA.FirstFooterOA.CenteredText.UnderlyingTsString);
							WriteHeaderFooterBlock("bottom-" + sOutside, pub.DivisionsOS[0].HFSetOA.FirstFooterOA.OutsideAlignedText.UnderlyingTsString);
						}
						m_cColumns = pub.DivisionsOS[0].NumColumns;
					}
				}
				else if (type == CssType.Dictionary)
				{
					m_writer.WriteLine("    margin: 2cm 2cm 2cm 2cm;");
					m_writer.WriteLine("    @top-left {");
					m_writer.WriteLine("        content: string(guideword, first);");
					WriteSansSerifFontFamilyForWs(m_cache.DefaultVernWs, true);
					m_writer.WriteLine("        font-weight: bold;");
					m_writer.WriteLine("        font-size: 12pt;");
					m_writer.WriteLine("        margin-top: 1em;");
					m_writer.WriteLine("    }");
					m_writer.WriteLine("    @top-center {");
					m_writer.WriteLine("        content: counter(page);");
					m_writer.WriteLine("        margin-top: 1em");
					m_writer.WriteLine("    }");
					m_writer.WriteLine("    @top-right {");
					m_writer.WriteLine("        content: string(guideword, last);");
					WriteSansSerifFontFamilyForWs(m_cache.DefaultVernWs, true);
					m_writer.WriteLine("        font-weight: bold;");
					m_writer.WriteLine("        font-size: 12pt;");
					m_writer.WriteLine("        margin-top: 1em;");
					m_writer.WriteLine("    }");
					WriteFootnotesBlock(type);
					m_writer.WriteLine("}");
					m_writer.WriteLine("@page :first {");
					m_writer.WriteLine("    @top-left { content: ''; }");
					m_writer.WriteLine("    @top-center { content: ''; }");
					m_writer.WriteLine("    @top-right { content: ''; }");
				}
				else if (type == CssType.Scripture)
				{
					m_writer.WriteLine("    margin: 2cm 2cm 2cm 2cm;");
					m_writer.WriteLine("    counter-increment: page;");
					WriteFootnotesBlock(type);
				}
				m_writer.WriteLine("}");
				m_writer.WriteLine();
				if (type == CssType.Dictionary)
				{
					ProcessDictionaryTypeClasses();
				}
				else if (type == CssType.Scripture)
				{
					foreach (string sClass in m_dictClassData.Keys)
						ProcessScriptureCssStyle(sClass, m_dictClassData[sClass]);
				}
				m_writer.Close();
			}
			m_writer = null;
		}

		private void WriteFootnotesBlock(CssType type)
		{
			if (type != CssType.Scripture)
				return;
			m_writer.WriteLine("    @footnotes {");		// Prince XML
			m_writer.WriteLine("        border-top: thin solid black;");
			m_writer.WriteLine("        padding: 0.3em 0;");
			m_writer.WriteLine("        margin-top: 0.6em;");
			m_writer.WriteLine("        margin-left: 2pi;");
			m_writer.WriteLine("    }");
			m_writer.WriteLine("    @footnote {");		// CSS3
			m_writer.WriteLine("        border-top: thin solid black;");
			m_writer.WriteLine("        padding: 0.3em 0;");
			m_writer.WriteLine("        margin-top: 0.6em;");
			m_writer.WriteLine("        margin-left: 2pi;");
			m_writer.WriteLine("    }");
		}

		// it would be nice to get these from HeaderFooterVc, but that DLL depends on this one,
		// so we can't get there from here.  But since these are constant, and this is probably
		// a fixed list that won't change much if at all, ...
		static readonly Guid PageNumberGuid = new Guid("644DF48A-3B60-45f4-80C7-739BE6E56A96");
		static readonly Guid FirstReferenceGuid = new Guid("397F43AE-E2B2-4f20-928A-1DF193C07674");
		static readonly Guid LastReferenceGuid = new Guid("85EE15C6-0799-46c6-8769-F9B3CE313AE2");
		static readonly Guid TotalPagesGuid = new Guid("E0EF9EDA-E4E2-4fcf-8720-5BC361BCE110");
		static readonly Guid PrintDateGuid = new Guid("C4556A21-41A8-4675-A74D-59B2C1A7E2B8");
		static readonly Guid DivisionNameGuid = new Guid("2277B85F-47BB-45c9-BC7A-7232E26E901C");
		static readonly Guid PublicationTitleGuid = new Guid("C8136D98-6957-43bd-BEA9-7DCE35200900");
		static readonly Guid PageReferenceGuid = new Guid("8978089A-8969-424e-AE54-B94C554F882D");
		static readonly Guid ProjectNameGuid = new Guid("5610D086-635F-4ae2-8E85-A95896F3D62D");
		static readonly Guid BookNameGuid = new Guid("48C0E5E3-C909-42e1-8F82-3489E3DE96FA");

		private void WriteHeaderFooterBlock(string sBlockName, ITsString tss)
		{
			if (tss == null)
				return;
			StringBuilder bldr = new StringBuilder();
			int crun = tss.RunCount;
			for (int i = 0; i < crun; ++i)
			{
				string sRun = tss.get_RunText(i);
				if (String.IsNullOrEmpty(sRun))
					continue;
				ITsTextProps ttp = tss.get_Properties(i);
				if (sRun.Length == 1 && sRun[0] == StringUtils.kchObject)
				{
					string objData = ttp.GetStrPropValue((int)FwTextPropType.ktptObjData);
					if (!String.IsNullOrEmpty(objData) && objData[0] == (char)FwObjDataTypes.kodtContextString)
					{
						Guid guid = MiscUtils.GetGuidFromObjData(objData.Substring(1));
						if (guid == PageNumberGuid)
							bldr.Append("counter(page) ");
						else if (guid == FirstReferenceGuid)
							bldr.AppendFormat("string(bookname, first) ' ' string(chapter, first) '{0}' string(verse, first) ",
								m_cache.LangProject.TranslatedScriptureOA.ChapterVerseSepr);
						else if (guid == LastReferenceGuid)
							bldr.AppendFormat("string(bookname, last) ' ' string(chapter, last) '{0}' string(verse, last) ",
								m_cache.LangProject.TranslatedScriptureOA.ChapterVerseSepr);
						else if (guid == TotalPagesGuid)
							bldr.Append("totalpagecount() ");
						else if (guid == PrintDateGuid)
							bldr.AppendFormat("'{0}' ", DateTime.Now.ToShortDateString());
						else if (guid == DivisionNameGuid)
							bldr.Append("string(divisionname) ");
						else if (guid == PublicationTitleGuid)
							bldr.Append("string(title)");
						else if (guid == PageReferenceGuid)
							bldr.Append("string(reference, page) ");
						else if (guid == ProjectNameGuid)
							bldr.AppendFormat("'{0}' ", m_cache.LangProject.Name.BestVernacularAnalysisAlternative.Text);
						else if (guid == BookNameGuid)
							bldr.Append("string(bookname)");
					}
				}
				else
				{
					bldr.AppendFormat("'{0}' ", sRun);
				}
			}
			if (bldr.Length > 0)
			{
				m_writer.WriteLine("    @{0} {{", sBlockName);
				m_writer.WriteLine("        content: {0};", bldr.ToString());
				WriteSansSerifFontFamilyForWs(m_cache.DefaultVernWs, true);
				/*
			font-weight: bold;
			font-size: 12pt;
			margin-top: 1em;
				 */
				m_writer.WriteLine("    }");
			}
			else
			{
				m_writer.WriteLine("    @{0} {{ content: ''; }}", sBlockName);
			}
		}

		private void ProcessScriptureCssStyle(string sClass, List<string> langs)
		{
			switch (sClass)
			{
				case "scrBody":
					WriteCssEmptyDefinition("scrBody");
					break;
				case "scrBook":
					WriteCssScrBook();
					break;
				case "scrBookName":
					WriteCssScrBookName();
					break;
				case "scrIntroSection":
					WriteCssScrIntroSection();
					break;
				case "scrSection":
					WriteCssScrSection();
					break;
				case "picture":
					WriteCssPicture();
					break;
				case "pictureRight":
					WriteCssPictureRight();
					break;
				case "pictureCenter":
					WriteCssPictureCenter();
					break;
				case "pictureCaption":
					WriteCssEmptyDefinition("pictureCaption");
					break;
				case "scrFootnote":
					WriteCssEmptyDefinition("scrFootnote");	// shouldn't be used, but just in case...
					break;
				case "scrFootnoteMarker":
					WriteCssScrFootnoteMarker();
					break;
				default:
					WriteScriptureCssStyle(sClass, langs);
					break;
			}
		}

		private void WriteScriptureCssStyle(string sStyle, List<string> langs)
		{
			string sCssStyleName = GetValidCssClassName(sStyle);
			string sStyleName;
			if (!m_mapCssClassToFwStyle.TryGetValue(sCssStyleName, out sStyleName))
				sStyleName = sStyle.Replace('_', ' ');		// just in case...
			m_writer.WriteLine(".{0} {{", sCssStyleName);
			ExportStyleInfo esi = WriteFontInfoToCss(m_cache.DefaultVernWs, sStyleName, null);
			if (esi != null)
				WriteParaStyleInfoToCss(esi);
			if (sStyleName == "Chapter Number")
			{
				if (m_fRTL)
					m_writer.WriteLine("    float: right;");
				else
					m_writer.WriteLine("    float: left;");
				m_writer.WriteLine("    string-set: chapter content();");
			}
			else if (sStyleName == "Verse Number")
			{
				m_writer.WriteLine("    string-set: verse content();");
			}
			else if (sStyleName == "Note General Paragraph" ||
				sStyleName == "Note Cross-Reference Paragraph")
			{
				m_writer.WriteLine("    display: inline;");
				m_writer.WriteLine("    display: footnote;");
				m_writer.WriteLine("    display: prince-footnote;");
				m_writer.WriteLine("    position: footnote;");
				m_writer.WriteLine("    list-style-position: inside;");
			}
			else if (sStyleName == "columns")
			{
				m_writer.WriteLine("    column-count: 2; -moz-column-count: 2;");
				m_writer.WriteLine("    column-gap: .5cm; -moz-column-gap: .5cm;");

			}
			m_writer.WriteLine("}");
			foreach (string sLang in langs)
			{
				string sWs = LgWritingSystem.ConvertRFC4646ToICU(sLang);
				int ws = m_cache.LanguageWritingSystemFactoryAccessor.GetWsFromStr(sWs);
				if (ws != 0 && ws != m_cache.DefaultVernWs)
				{
					m_writer.WriteLine(".{0}[lang='{1}'] {{", sCssStyleName, sLang);
					esi = WriteFontInfoToCss(ws, sStyleName, null);
					if (esi != null)
						WriteParaStyleInfoToCss(esi);
					m_writer.WriteLine("}");
				}
			}
			if (sStyleName == "Note General Paragraph" ||
				sStyleName == "Note Cross-Reference Paragraph")
			{
				m_writer.WriteLine(".{0}::footnote-call {{", sCssStyleName);
				m_writer.WriteLine("    color: purple;");
				m_writer.WriteLine("    content: attr(title);");
				m_writer.WriteLine("    font-size: 6pt;");
				m_writer.WriteLine("    vertical-align: super;");
				m_writer.WriteLine("    line-height: none;");
				m_writer.WriteLine("}");
				m_writer.WriteLine(".{0}::footnote-marker {{", sCssStyleName);
				m_writer.WriteLine("    font-size: 10pt;");
				m_writer.WriteLine("    font-weight: bold;");
				m_writer.WriteLine("    content: string(chapter) ':' string(verse) ' = ';");
				m_writer.WriteLine("    /* content: string(footnoteMarker); font-size: 6pt; vertical-align: super; color: purple; */");
				m_writer.WriteLine("    text-align: left;");
				m_writer.WriteLine("}");
			}
		}

		private Dictionary<string, string> m_mapFwStyleToCssClass = new Dictionary<string, string>();
		private Dictionary<string, string> m_mapCssClassToFwStyle = new Dictionary<string, string>();
		/// <summary>
		/// Convert a FieldWorks Style name into a valid CSS class name.
		/// </summary>
		public string GetValidCssClassName(string sFwStyle)
		{
			string sCssClassName;
			if (m_mapFwStyleToCssClass.TryGetValue(sFwStyle, out sCssClassName))
				return sCssClassName;
			sCssClassName = sFwStyle.Replace(' ', '_');
			char[] rgchStyleName = sCssClassName.ToCharArray();
			for (int i = 0; i < rgchStyleName.Length; ++i)
			{
				char ch1 = rgchStyleName[i];
				int ch32;
				string sChar;
				if (Surrogates.IsLeadSurrogate(ch1))
				{
					if (++i < rgchStyleName.Length)
					{
						char ch2 = rgchStyleName[i];
						ch32 = Surrogates.Int32FromSurrogates(ch1, ch2);
						sChar = ch1.ToString() + ch2.ToString();
					}
					else
					{
						break;	// invalid data, but we can't fix it!
					}
				}
				else
				{
					ch32 = (int)ch1;
					sChar = ch1.ToString();
				}
				if (ch32 != '_' && !Icu.IsAlphabetic(ch32) && !Icu.IsNumeric(ch32) && !Icu.IsDiacritic(ch32))
				{
					string sCharName;
					if (ch32 == '-')
					{
						sCssClassName = sCssClassName.Replace(sChar, "HYPHEN");
					}
					else
					{
						Icu.UErrorCode error = Icu.UErrorCode.U_ZERO_ERROR;
						Icu.u_CharName(ch32, Icu.UCharNameChoice.U_UNICODE_CHAR_NAME, out sCharName, out error);
						sCharName = sCharName.Replace('-', '_');
						sCharName = sCharName.Replace(' ', '_');
						sCssClassName = sCssClassName.Replace(sChar, sCharName);
					}
				}
			}
			m_mapFwStyleToCssClass.Add(sFwStyle, sCssClassName);
			string sOldStyle;
			if (!m_mapCssClassToFwStyle.TryGetValue(sCssClassName, out sOldStyle))
				m_mapCssClassToFwStyle.Add(sCssClassName, sFwStyle);
			return sCssClassName;
		}

		private void WriteCssScrFootnoteMarker()
		{
			m_writer.WriteLine(".scrFootnoteMarker {");
			m_writer.WriteLine("}");
		}

		private void WriteCssScrIntroSection()
		{
			m_writer.WriteLine(".scrIntroSection {");
			m_writer.WriteLine("    text-align: {0};", m_fRTL ? "right" : "left");
			m_writer.WriteLine("}");
		}

		private void WriteCssScrSection()
		{
			m_writer.WriteLine(".scrSection {");
			if (m_cColumns > 1)
			{
				m_writer.WriteLine("    column-count: {0};   -moz-column-count: {0};", m_cColumns);
				m_writer.WriteLine("    column-gap: 1.5em; -moz-column-gap: 1.5em;");
				m_writer.WriteLine("    column-fill: auto;");
			}
			m_writer.WriteLine("    text-align: {0};", m_fRTL ? "right" : "left");
			m_writer.WriteLine("}");
		}

		private void WriteCssScrBook()
		{
			m_writer.WriteLine(".scrBook {");
			m_writer.WriteLine("    column-count: 1;");
			m_writer.WriteLine("    clear: both;");
			m_writer.WriteLine("}");
		}

		private void WriteCssScrBookName()
		{
			m_writer.WriteLine(".scrBookName {");
			m_writer.WriteLine("    string-set: bookname content();");
			m_writer.WriteLine("    display: none;");
			m_writer.WriteLine("}");
		}

		private void ProcessDictionaryTypeClasses()
		{
			if (m_dictClassData.ContainsKey("dicBody"))
			{
				WriteCssEmptyDefinition("dicBody");
				m_dictClassData.Remove("dicBody");
			}
			if (m_dictClassData.ContainsKey("letHead"))
			{
				WriteCssLetHead();
				m_dictClassData.Remove("letHead");
			}
			if (m_dictClassData.ContainsKey("letter"))
			{
				WriteCssLetter();
				m_dictClassData.Remove("letter");
			}
			if (m_dictClassData.ContainsKey("letData"))
			{
				WriteCssLetData();
				m_dictClassData.Remove("letData");
			}
			if (m_dictClassData.ContainsKey("entry"))
			{
				WriteCssEntry();
				m_dictClassData.Remove("entry");
			}
			if (m_dictClassData.ContainsKey("subentry"))
			{
				WriteCssSubentry();
				m_dictClassData.Remove("subentry");
			}
			if (m_dictClassData.ContainsKey("xhomographnumber"))
			{
				WriteCssXHomographNumber();
				m_dictClassData.Remove("xhomographnumber");
			}
			if (m_dictClassData.ContainsKey("sense"))
			{
				WriteCssSense();
				m_dictClassData.Remove("sense");
			}
			if (m_dictClassData.ContainsKey("xsensenumber"))
			{
				WriteCssXSenseNumber();
				m_dictClassData.Remove("xsensenumber");
			}
			if (m_dictClassData.ContainsKey("pictureRight"))
			{
				WriteCssPictureRight();
				m_dictClassData.Remove("pictureRight");
			}
			if (m_dictClassData.ContainsKey("picture"))
			{
				WriteCssPicture();
				m_dictClassData.Remove("picture");
			}
			if (m_dictClassData.ContainsKey("pictureCaption"))
			{
				WriteCssEmptyDefinition("pictureCaption");
				m_dictClassData.Remove("pictureCaption");
			}
			if (m_dictClassData.ContainsKey("xlanguagetag"))
			{
				WriteCssXLanguageTag();
				m_dictClassData.Remove("xlanguagetag");
			}
			List<string> rgsLangs;
			if (m_dictClassData.TryGetValue("xitem", out rgsLangs))
			{
				WriteCssXItem(rgsLangs);
				m_dictClassData.Remove("xitem");
			}
			m_writer.WriteLine();
			foreach (string sClass in m_dictClassData.Keys)
				ProcessDictionaryCssStyle(sClass);
		}

		private void WriteCssLetHead()
		{
			m_writer.WriteLine(".letHead {");
			m_writer.WriteLine("    column-count: 1;");
			m_writer.WriteLine("    clear: both;");
			m_writer.WriteLine("}");
		}

		private void WriteCssLetter()
		{
			m_writer.WriteLine(".letter {");
			m_writer.WriteLine("    text-align: center;");
			m_writer.WriteLine("    width: 100%;");
			m_writer.WriteLine("    margin-top: 18pt;");
			m_writer.WriteLine("    margin-bottom: 18pt;");
			WriteFontFamilyForWs(m_cache.DefaultVernWs, true);
			m_writer.WriteLine("    font-weight: bold;");
			m_writer.WriteLine("    font-size: 24pt;");
			m_writer.WriteLine("}");
		}

		private void WriteCssLetData()
		{
			m_writer.WriteLine(".letData {");
			m_writer.WriteLine("    column-count: 2;   -moz-column-count: 2;");
			m_writer.WriteLine("    column-gap: 1.5em; -moz-column-gap: 1.5em;");
			m_writer.WriteLine("    column-fill: balance;");
			m_writer.WriteLine("    text-align: {0};", m_fRTL ? "right" : "left");
			m_writer.WriteLine("}");
		}

		private void WriteCssEntry()
		{
			m_writer.WriteLine(".entry {");
			ExportStyleInfo esi = WriteFontInfoToCss(m_cache.DefaultAnalWs, "Dictionary-Normal", "entry");
			WriteParaStyleInfoToCss(esi);
			m_writer.WriteLine("}");
		}

		private void WriteCssSubentry()
		{
			m_writer.WriteLine(".subentry {");
			ExportStyleInfo esi = WriteFontInfoToCss(m_cache.DefaultAnalWs, "Dictionary-Subentry", "subentry");
			WriteParaStyleInfoToCss(esi);
			m_writer.WriteLine("}");
		}

		private void WriteCssXHomographNumber()
		{
			m_writer.WriteLine(".xhomographnumber {");
			m_writer.WriteLine("    font-weight: bold;");
			m_writer.WriteLine("    font-size: 55%;");
			m_writer.WriteLine("    vertical-align: sub;");
			BaseStyleInfo bsi;
			if (m_styleTable.TryGetValue("Dictionary-Normal", out bsi))
			{
				ExportStyleInfo esi = bsi as ExportStyleInfo;
				WriteFontAttr(m_cache.DefaultAnalWs, "font-family", esi, null, true);
				WriteFontAttr(m_cache.DefaultAnalWs, "color", esi, null, true);
				WriteFontAttr(m_cache.DefaultAnalWs, "background-color", esi, null, true);
			}
			m_writer.WriteLine("}");
		}

		private void WriteCssSense()
		{
			m_writer.WriteLine(".sense {");
			m_writer.WriteLine("    font-weight: normal;");
			m_writer.WriteLine("}");
		}

		private void WriteCssXLanguageTag()
		{
			m_writer.WriteLine(".xlanguagetag {");
			if (WriteFontInfoToCss(-1, "Language Code", "xlanguagetag") == null)
			{
				m_writer.WriteLine("    color: green;");
				m_writer.WriteLine("    font-size: x-small;");
			}
			m_writer.WriteLine("}");
		}

		private void WriteCssXItem(List<string> rgsLangs)
		{
			m_writer.WriteLine(".xitem {");
			m_writer.WriteLine("    /* placeholder for adding list separators */");
			m_writer.WriteLine("}");
			foreach (string sLang in rgsLangs)
			{
				string sWs = LgWritingSystem.ConvertRFC4646ToICU(sLang);
				int ws = m_cache.LanguageWritingSystemFactoryAccessor.GetWsFromStr(sWs);
				if (ws != m_cache.DefaultAnalWs && ws != m_cache.DefaultVernWs)
				{
					m_writer.WriteLine(".xitem[lang='{0}'] {{", sLang);
					WriteFontInfoToCss(ws, "Dictionary-Normal", null);
					m_writer.WriteLine("}");
				}
			}
		}

		private void WriteCssXSenseNumber()
		{
			m_writer.WriteLine(".xsensenumber {");
			m_writer.WriteLine("    font-weight: bold;");
			BaseStyleInfo bsi;
			if (m_styleTable.TryGetValue("Dictionary-Normal", out bsi))
			{
				ExportStyleInfo esi = bsi as ExportStyleInfo;
				WriteFontAttr(m_cache.DefaultAnalWs, "font-family", esi, null, true);
				WriteFontAttr(m_cache.DefaultAnalWs, "color", esi, null, true);
				WriteFontAttr(m_cache.DefaultAnalWs, "background-color", esi, null, true);
			}
			m_writer.WriteLine("}");
		}

		private void WriteCssEmptyDefinition(string sClass)
		{
			m_writer.WriteLine(".{0} {{", sClass.Replace(' ', '_'));
			m_writer.WriteLine("}");
		}

		private void WriteCssPicture()
		{
			m_writer.WriteLine(".picture {");
			// limiting height works better than limiting width for multiple pictures
			m_writer.WriteLine("    height: 1.0in;");
			m_writer.WriteLine("}");
		}

		private void WriteCssPictureCenter()
		{
			m_writer.WriteLine(".pictureCenter {");
			m_writer.WriteLine("    float: center;");
			m_writer.WriteLine("    margin: 0pt 0pt 4pt 4pt;");
			m_writer.WriteLine("    padding: 2pt;");
			m_writer.WriteLine("    text-indent: 0pt;");
			m_writer.WriteLine("    text-align: center;");
			m_writer.WriteLine("}");
		}

		private void WriteCssPictureRight()
		{
			m_writer.WriteLine(".pictureRight {");
			m_writer.WriteLine("    float: right;");
			m_writer.WriteLine("    margin: 0pt 0pt 4pt 4pt;");
			m_writer.WriteLine("    padding: 2pt;");
			m_writer.WriteLine("    text-indent: 0pt;");
			m_writer.WriteLine("    text-align: center;");
			m_writer.WriteLine("}");
		}

		private void WriteSansSerifFontFamilyForWs(int ws, bool fWriteDirection)
		{
			ILgWritingSystem lgws = LgWritingSystem.CreateFromDBObject(m_cache, ws);
			if (fWriteDirection)
				m_writer.WriteLine("        direction: {0};", lgws.RightToLeft ? "rtl" : "ltr");
			if (!String.IsNullOrEmpty(lgws.DefaultSansSerif))
				m_writer.WriteLine("        font-family: \"{0}\", sans-serif;   /* default Sans-Serif font */",
					lgws.DefaultSansSerif);
			else if (!String.IsNullOrEmpty(lgws.DefaultBodyFont))
				m_writer.WriteLine("        font-family: \"{0}\", sans-serif;   /* default Body font */",
					lgws.DefaultBodyFont);
			else if (!String.IsNullOrEmpty(lgws.DefaultSerif))
				m_writer.WriteLine("        font-family: \"{0}\", sans-serif;   /* default Serif font */",
					lgws.DefaultSerif);
			else if (!String.IsNullOrEmpty(lgws.DefaultMonospace))
				m_writer.WriteLine("        font-family: \"{0}\", sans-serif;   /* default Monospace font */",
					lgws.DefaultMonospace);
		}

		private void WriteFontFamilyForWs(int ws, bool fWriteDirection)
		{
			ILgWritingSystem lgws = LgWritingSystem.CreateFromDBObject(m_cache, ws);
			if (fWriteDirection)
				m_writer.WriteLine("    direction: {0};", lgws.RightToLeft ? "rtl" : "ltr");
			if (!String.IsNullOrEmpty(lgws.DefaultSerif))
				m_writer.WriteLine("    font-family: \"{0}\", serif;	/* default Serif font */",
					lgws.DefaultSerif);
			else if (!String.IsNullOrEmpty(lgws.DefaultBodyFont))
				m_writer.WriteLine("    font-family: \"{0}\", serif;	/* default Body font */",
					lgws.DefaultBodyFont);
			else if (!String.IsNullOrEmpty(lgws.DefaultSansSerif))
				m_writer.WriteLine("    font-family: \"{0}\", sans-serif;	/* default Sans-Serif font */",
					lgws.DefaultSansSerif);
			else if (!String.IsNullOrEmpty(lgws.DefaultMonospace))
				m_writer.WriteLine("    font-family: \"{0}\", monospace;	/* default Monospace font */",
					lgws.DefaultMonospace);
		}

		private void ReadStyles(IVwStylesheet vss)
		{
			string normalStyleName = vss.GetDefaultBasedOnStyleName();
			m_styleTable = new StyleInfoTable(normalStyleName,
				m_cache.LanguageWritingSystemFactoryAccessor);
			int cStyles = vss.CStyles;
			for (int i = 0; i < cStyles; ++i)
			{
				int hvo = vss.get_NthStyle(i);
				IStStyle sty = StStyle.CreateFromDBObject(m_cache, hvo);
				m_styleTable.Add(sty.Name, new ExportStyleInfo(sty));
			}
		}

		private void ProcessDictionaryCssStyle(string sClass)
		{
			XmlNode xn;
			string sBaseClass;
			if (!m_mapCssClassToXnode.TryGetValue(sClass, out xn))
			{
				int idx = sClass.IndexOf("_L");
				if (idx <= 0)
					return;
				sBaseClass = sClass.Substring(0, idx);
				if (!m_mapCssClassToXnode.TryGetValue(sBaseClass, out xn))
					return;
			}
			else
			{
				sBaseClass = sClass;
			}
			string sBefore = XmlUtils.GetOptionalAttributeValue(xn, "before");
			string sAfter = XmlUtils.GetOptionalAttributeValue(xn, "after");
			string sSep = XmlUtils.GetOptionalAttributeValue(xn, "sep");
			string sStyle = XmlUtils.GetOptionalAttributeValue(xn, "style");
			int ws = 0;
			List<string> rgsLangs;
			string sLang = null;
			if (m_dictClassData.TryGetValue(sClass, out rgsLangs) && rgsLangs.Count > 0)
			{
				sLang = rgsLangs[0];
				ws = m_cache.LanguageWritingSystemFactoryAccessor.GetWsFromStr(
					LgWritingSystem.ConvertRFC4646ToICU(sLang));
			}
			if (ws == 0)
			{
				string sWs = XmlUtils.GetOptionalAttributeValue(xn, "ws", String.Empty);
				if (sWs.StartsWith(LangProject.WsParamLabel))
					sWs = sWs.Substring(LangProject.WsParamLabel.Length);
				string sWsType = XmlUtils.GetOptionalAttributeValue(xn, "wsType");
				switch (sWs)
				{
					case "analysis":
						ws = m_cache.DefaultAnalWs;
						break;
					case "vernacular":
						ws = m_cache.DefaultVernWs;
						break;
					case "pronunciation":
						ws = m_cache.DefaultPronunciationWs;
						break;
					case "user":
						ws = m_cache.DefaultUserWs;
						break;
					case "all analysis":
						ws = m_cache.DefaultAnalWs;
						break;
					case "all vernacular":
						ws = m_cache.DefaultVernWs;
						break;
					case "":
						ws = m_cache.DefaultUserWs;	// need something!
						break;
					default:
						string[] rgsWs = sWs.Split(new char[] { ',' });
						if (rgsWs.Length > 0)
						{
							ws = m_cache.LanguageWritingSystemFactoryAccessor.GetWsFromStr(rgsWs[0]);
						}
						if (ws == 0)
						{
							switch (sWsType)
							{
								case "analysis":
									ws = m_cache.DefaultAnalWs;
									break;
								case "vernacular":
									ws = m_cache.DefaultVernWs;
									break;
								case "pronunciation":
									ws = m_cache.DefaultPronunciationWs;
									break;
								default:
									ws = m_cache.DefaultUserWs;		// need something!
									break;
							}
						}
						break;
				}
			}
			m_writer.WriteLine(".{0} {{", sClass.Replace(' ', '_'));
			if (!String.IsNullOrEmpty(sLang))
			{
				m_writer.Write("    /* lang = '{0}'", sLang);
				for (int i = 1; i < rgsLangs.Count; ++i)
					m_writer.Write(", '{0}'", rgsLangs[i]);
				m_writer.WriteLine(" */");
			}
			List<string> rgsStyles;
			if (!String.IsNullOrEmpty(sStyle))
			{
				m_writer.WriteLine("    /* explicit FieldWorks style = '{0}' */", sStyle);
				WriteFontInfoToCss(ws, sStyle, sBaseClass);
			}
			else if (ws != m_cache.DefaultAnalWs &&
				m_mapCssToStyleEnv.TryGetValue(sBaseClass, out rgsStyles) && rgsStyles.Count > 0)
			{
				m_writer.WriteLine("    /* cascaded environment FieldWorks style = '{0}' */", rgsStyles[0]);
				WriteFontInfoToCss(ws, rgsStyles[0], null);
			}
			string sClassLowered = sClass.ToLowerInvariant();
			if (sClassLowered.Contains("headword") && !sClassLowered.Contains("sub"))
				m_writer.WriteLine("    string-set: guideword content();");
			m_writer.WriteLine("}");
			if (!String.IsNullOrEmpty(sBefore))
				m_writer.WriteLine(".{0}:before {{ content: \"{1}\" }}",
					sClass, EscapeCharsForCss(sBefore));
			if (!String.IsNullOrEmpty(sAfter))
				m_writer.WriteLine(".{0}:after {{ content: \"{1}\" }}",
					sClass, EscapeCharsForCss(sAfter));
			if (!String.IsNullOrEmpty(sSep))
			{
				switch (sClass)
				{
					case "senses":
					case "senses-minor":
					case "senses-sub":
					case "subsenses":
					case "subsenses-minor":
					case "subsenses-sub":
						m_writer.WriteLine(".{0}>.sense + .sense:before {{ content: \"{1}\" }}",
							sClass, EscapeCharsForCss(sSep));
						break;
					default:
						m_writer.WriteLine(".{0}>.xitem + .xitem:before {{ content: \"{1}\" }}",
							sClass, EscapeCharsForCss(sSep));
						break;
				}
			}
		}

		private ExportStyleInfo WriteFontInfoToCss(int ws, string sStyle, string sCss)
		{
			BaseStyleInfo bsi = null;
			if (m_styleTable.TryGetValue(sStyle, out bsi))
			{
				ExportStyleInfo esi = bsi as ExportStyleInfo;
				if (!WriteFontAttr(ws, "font-family", esi, sCss, true) && ws != -1)
				{
					if (!WriteFontAttr(-1, "font-family", esi, sCss, true))
						WriteFontFamilyForWs(ws, false);
				}
				if (!WriteFontAttr(ws, "font-size", esi, sCss, true) && ws != -1)
				{
					if (!WriteFontAttr(-1, "font-size", esi, sCss, true))
					{
						if (sStyle == "Verse Number")
							m_writer.WriteLine("    font-size: smaller;");
						else if (sStyle == "Chapter Number")
							m_writer.WriteLine("    font-size: 200%;");
					}
				}
				if (!WriteFontAttr(ws, "font-weight", esi, sCss, true) && ws != -1)
				{
					if (!WriteFontAttr(-1, "font-weight", esi, sCss, true))
					{
						if (sStyle == "Chapter Number")
							m_writer.WriteLine("    font-weight: bolder;");
					}
				}
				if (!WriteFontAttr(ws, "font-style", esi, sCss, true) && ws != -1)
					WriteFontAttr(-1, "font-style", esi, sCss, true);
				if (!WriteFontAttr(ws, "color", esi, sCss, true) && ws != -1)
					WriteFontAttr(-1, "color", esi, sCss, true);
				if (!WriteFontAttr(ws, "background-color", esi, sCss, true) && ws != -1)
					WriteFontAttr(-1, "background-color", esi, sCss, true);
				if (!WriteFontAttr(ws, "vertical-align", esi, sCss, true) && ws != -1)
				{
					if (!WriteFontAttr(-1, "vertical-align", esi, sCss, true))
					{
						if (sStyle == "Verse Number")
							m_writer.WriteLine("    vertical-align: super;");
						else if (sStyle == "Chapter Number")
							m_writer.WriteLine("    vertical-align: top;");
					}
				}
				if (!WriteFontAttr(ws, "text-decoration", esi, sCss, true) && ws != -1)
					WriteFontAttr(-1, "text-decoration", esi, sCss, true);

				if (esi.DirectionIsRightToLeft != TriStateBool.triNotSet)
					m_writer.WriteLine("    direction: {0};",
						esi.DirectionIsRightToLeft == TriStateBool.triTrue ? "rtl" : "ltr");
			}
			return bsi as ExportStyleInfo;		// in case someone else needs it.
		}

		private bool WriteFontAttr(int ws, string sAttr, ExportStyleInfo esi, string sCss, bool fTopLevel)
		{
			FontInfo fi = esi.FontInfoForWs(ws);
			bool fExplicit = false;
			bool fInherited = false;
			string sInheritance = String.Empty;
			if (sCss == null && !fTopLevel)
				sInheritance = "\t/* inherited through cascaded environment */";
			if (sCss == null)
				sInheritance = "\t/* cascaded environment */";
			else if (!fTopLevel)
				sInheritance = "\t/* inherited */";
			switch (sAttr)
			{
				case "font-family":
					if (fi.m_fontName.ValueIsSet)
					{
						string sFontName = esi.RealFontNameForWs(ws);
						if (String.IsNullOrEmpty(sFontName))
							m_writer.WriteLine("    font-family: \"{0}\", serif;{1}", fi.m_fontName.Value, sInheritance);
						else
							m_writer.WriteLine("    font-family: \"{0}\", serif;{1}", sFontName, sInheritance);
						return true;
					}
					fExplicit = fi.m_fontName.IsExplicit;
					fInherited = fi.m_fontName.IsInherited;
					break;
				case "font-size":
					if (fi.m_fontSize.ValueIsSet)
					{
						m_writer.WriteLine("    font-size: {0}pt;{1}", ConvertMptToPt(fi.m_fontSize.Value), sInheritance);
						return true;
					}
					fExplicit = fi.m_fontSize.IsExplicit;
					fInherited = fi.m_fontSize.IsInherited;
					break;
				case "font-weight":
					if (fi.m_bold.ValueIsSet)
					{
						m_writer.WriteLine("    font-weight: {0};{1}", fi.m_bold.Value ? "bold" : "normal", sInheritance);
						return true;
					}
					fExplicit = fi.m_bold.IsExplicit;
					fInherited = fi.m_bold.IsInherited;
					break;
				case "font-style":
					if (fi.m_italic.ValueIsSet)
					{
						m_writer.WriteLine("    font-style: {0};{1}", fi.m_italic.Value ? "italic" : "normal", sInheritance);
						return true;
					}
					fExplicit = fi.m_italic.IsExplicit;
					fInherited = fi.m_italic.IsInherited;
					break;
				case "color":
					if (fi.m_fontColor.ValueIsSet)
					{
						m_writer.WriteLine("    color: rgb({0},{1},{2});{3}",
							fi.m_fontColor.Value.R, fi.m_fontColor.Value.G, fi.m_fontColor.Value.B, sInheritance);
						return true;
					}
					fExplicit = fi.m_fontColor.IsExplicit;
					fInherited = fi.m_fontColor.IsInherited;
					break;
				case "background-color":
					if (fi.m_backColor.ValueIsSet)
					{
						m_writer.WriteLine("    background-color: rgb({0},{1},{2});{3}",
							fi.m_backColor.Value.R, fi.m_backColor.Value.G, fi.m_backColor.Value.B, sInheritance);
						return true;
					}
					fExplicit = fi.m_backColor.IsExplicit;
					fInherited = fi.m_backColor.IsInherited;
					break;
				case "vertical-align":
					if (fi.m_superSub.ValueIsSet)
					{
						m_writer.WriteLine("    vertical-align: {0};{1}", GetVerticalAlign(fi.m_superSub.Value), sInheritance);
						return true;
					}
					if (fi.m_offset.ValueIsSet)
					{
						m_writer.WriteLine("    vertical-align: {0}pt;{1}", ConvertMptToPt(fi.m_offset.Value), sInheritance);
						return true;
					}
					fExplicit = fi.m_offset.IsExplicit || fi.m_superSub.IsExplicit;
					fInherited = fi.m_offset.IsInherited || fi.m_superSub.IsInherited;
					break;
				case "text-decoration":
					if (fi.m_underline.ValueIsSet)
					{
						m_writer.WriteLine("    text-decoration: {0};{1}",
							(fi.m_underline.Value == FwUnderlineType.kuntNone ? "none" : "underline"), sInheritance);
						return true;
					}
					fExplicit = fi.m_underline.IsExplicit;
					fInherited = fi.m_underline.IsInherited;
					break;
			}
			if (fInherited)
			{
				string sBaseStyle = esi.InheritsFrom;
				BaseStyleInfo bsiBase;
				if (!String.IsNullOrEmpty(sBaseStyle) && m_styleTable.TryGetValue(sBaseStyle, out bsiBase))
				{
					if (WriteFontAttr(ws, sAttr, bsiBase as ExportStyleInfo, sCss, false))
						return true;
				}
				if (!String.IsNullOrEmpty(sCss))
				{
					List<string> rgsStyles;
					if (m_mapCssToStyleEnv.TryGetValue(sCss, out rgsStyles))
					{
						foreach (string sParaStyle in rgsStyles)
						{
							BaseStyleInfo bsiPara;
							if (m_styleTable.TryGetValue(sParaStyle, out bsiPara) &&
								WriteFontAttr(ws, sAttr, bsiPara as ExportStyleInfo, null, false))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private void WriteParaStyleInfoToCss(ExportStyleInfo esi)
		{
			string sLeading = esi.DirectionIsRightToLeft == TriStateBool.triTrue ? "right" : "left";
			string sTrailing = esi.DirectionIsRightToLeft == TriStateBool.triTrue ? "left" : "right";
			if (esi.HasAlignment)
			{
				FwTextAlign fta = esi.Alignment;
				switch (esi.Alignment)
				{
					case FwTextAlign.ktalCenter:
						m_writer.WriteLine("    text-align: center;");
						break;
					case FwTextAlign.ktalJustify:
						m_writer.WriteLine("    text-align: justify;");
						break;
					case FwTextAlign.ktalLeft:
						m_writer.WriteLine("    text-align: left;");
						break;
					case FwTextAlign.ktalRight:
						m_writer.WriteLine("    text-align: right;");
						break;
					case FwTextAlign.ktalLeading:
						m_writer.WriteLine("    /*text-align: leading;*/");
						m_writer.WriteLine("    text-align: {0};", sLeading);
						break;
					case FwTextAlign.ktalTrailing:
						m_writer.WriteLine("    /*text-align: trailing;*/");
						m_writer.WriteLine("    text-align: {0};", sTrailing);
						break;
				}
			}
			if (esi.HasBorder)
			{
				m_writer.WriteLine("    border-style: solid;");
				m_writer.WriteLine("    border-top-width: {0}pt;", ConvertMptToPt(esi.BorderTop));
				m_writer.WriteLine("    border-bottom-width: {0}pt;", ConvertMptToPt(esi.BorderBottom));
				m_writer.WriteLine("    border-left-width: {0}pt;", ConvertMptToPt(esi.BorderLeading));
				m_writer.WriteLine("    border-right-width: {0}pt;", ConvertMptToPt(esi.BorderTrailing));
			}
			if (esi.HasBorderColor)
				m_writer.WriteLine("    border-color: rgb({0},{1},{2});",
					esi.BorderColor.R, esi.BorderColor.G, esi.BorderColor.B);
			if (esi.HasFirstLineIndent)
			{
				m_writer.WriteLine("    text-indent: {0}pt;", ConvertMptToPt(esi.FirstLineIndent));
				m_writer.Write("    margin-{0}: ", sLeading);
				if (esi.FirstLineIndent < 0)
					m_writer.WriteLine("{0}pt;", ConvertMptToPt(-esi.FirstLineIndent));
				else
					m_writer.WriteLine("0pt;");
			}
			else
			{
				m_writer.WriteLine("    text-indent: 0pt;");
				m_writer.WriteLine("    margin-{0}: 0pt;", sLeading);
			}
			if (esi.HasLeadingIndent)
				m_writer.WriteLine("    padding-{0}: {1}pt;", sLeading, ConvertMptToPt(esi.LeadingIndent));
			if (esi.HasTrailingIndent)
				m_writer.WriteLine("    padding-{0}: {1}pt;", sTrailing, ConvertMptToPt(esi.TrailingIndent));
			if (esi.HasLineSpacing)
			{
				LineHeightInfo lhi = esi.LineSpacing;
				if (lhi.m_relative)
					m_writer.WriteLine("    line-height: {0}%;", lhi.m_lineHeight / 100);
				else if (lhi.m_lineHeight < 0)
					m_writer.WriteLine("    line-height: {0}pt;", ConvertMptToPt(-lhi.m_lineHeight));
				else
					m_writer.WriteLine("    line-height: normal;");	// "at least" semantics??
			}
			if (esi.HasSpaceAfter)
				m_writer.WriteLine("    padding-bottom: {0}pt;", ConvertMptToPt(esi.SpaceAfter));
			if (esi.HasSpaceBefore)
				m_writer.WriteLine("    padding-top: {0}pt;", ConvertMptToPt(esi.SpaceBefore));
			if (esi.HasWidowOrphanControl)
			{
				m_writer.WriteLine("    orphans: {0};", esi.WidowOrphanControl ? "2" : "1");
				m_writer.WriteLine("    widows: {0};", esi.WidowOrphanControl ? "2" : "1");
			}
			// esi.KeepTogether;
			// esi.KeepWithNext;
		}
		private string GetVerticalAlign(FwSuperscriptVal super)
		{
			switch (super)
			{
				case FwSuperscriptVal.kssvSub:
					return "sub";
				case FwSuperscriptVal.kssvSuper:
					return "super";
				default:
					return "baseline";
			}
		}

		private string ConvertMptToPt(int mpt)
		{
			int pt = mpt / 1000;
			int frac = Math.Abs(mpt) % 1000;
			if (frac == 0)
				return pt.ToString();
			else
				return String.Format("{0}.{1:d3}", pt, frac);
		}

		private string EscapeCharsForCss(string sBefore)
		{
			string s = sBefore.Replace("\\", "\\\\");
			s = s.Replace("\"", "\\\"");
			s = s.Replace("\r", "\\00000d");
			s = s.Replace("\n", "\\00000a");
			return s;
		}

		/// <summary>
		/// XSLT processors do too much with xmlns attributes, so we need to add it here to the
		/// html element.  We also need to retrieve the class and lang attributes that result
		/// from the XSLT processing, so we do that as well.
		/// </summary>
		/// <param name="sOutputFile"></param>
		/// <param name="sTempFile"></param>
		public void FinalizeXhtml(string sOutputFile, string sTempFile)
		{
			using (TextReader rdr = new StreamReader(sTempFile))
			{
				using (TextWriter wtr = new StreamWriter(sOutputFile))
				{
					string sLine = rdr.ReadLine();
					while (sLine != null)
					{
						int idxClass = -1;
						while ((idxClass = sLine.IndexOf(" class=\"", idxClass + 1)) >= 0)
						{
							int idxMin = idxClass + 8;
							int idxLim = sLine.IndexOf('"', idxMin);
							if (idxLim > idxMin)
							{
								string sClass = sLine.Substring(idxMin, idxLim - idxMin);
								string sLang = null;
								if (sLine.Substring(idxLim).StartsWith("\" lang=\""))
								{
									int idxLang = idxLim + 8;
									int idxLangLim = sLine.IndexOf('"', idxLang);
									if (idxLangLim > idxLang)
										sLang = sLine.Substring(idxLang, idxLangLim - idxLang);
								}
								List<string> rgsLangs;
								if (!m_dictClassData.TryGetValue(sClass, out rgsLangs))
								{
									// Many TE styles have spaces in their names, which are
									// replaced by underscores.  Try finding the original name
									// before inserting a new value into the table.
									string sFwStyle;
									if (!m_mapCssClassToFwStyle.TryGetValue(sClass, out sFwStyle))
										sFwStyle = sClass.Replace('_', ' ');		// just in case...
									if (!m_dictClassData.TryGetValue(sFwStyle, out rgsLangs))
									{
										rgsLangs = new List<string>();
										m_dictClassData.Add(sClass, rgsLangs);
									}
								}
								if (!String.IsNullOrEmpty(sLang) && !rgsLangs.Contains(sLang))
									rgsLangs.Add(sLang);
							}
						}
						int idxHtml = sLine.IndexOf("<html ");
						if (idxHtml >= 0)
							sLine = sLine.Insert(idxHtml + 5, " xmlns=\"http://www.w3.org/1999/xhtml\"");
						wtr.WriteLine(sLine);
						sLine = rdr.ReadLine();
					}
					wtr.Close();
				}
				rdr.Close();
			}
		}
	}
}
