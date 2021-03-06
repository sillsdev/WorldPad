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
// File: TeStylesXmlAccessor.cs
// Responsibility: TE Team
//
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.LangProj;
using SIL.FieldWorks.FDO.Scripture;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.Common.Controls;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.Common.Framework;
using SIL.FieldWorks.FwCoreDlgs;

namespace SIL.FieldWorks.TE
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class containing static methods for accessing the information about factory styles in
	/// the TeStyles.xml file.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TeStylesXmlAccessor : StylesXmlAccessor
	{
		#region Data members
		/// <summary>The FDO Scripture object which will own the new styles</summary>
		protected IScripture m_scr;
		#endregion

		/// ------------------------------------------------------------------------------------
		/// Used only for getting help info...instance cannot support StyleCollection call
		/// and therefore not CreateStyles, EnsureCurrentStylesheet, etc.
		/// ------------------------------------------------------------------------------------
		private TeStylesXmlAccessor()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TeStylesXmlAccessor"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected TeStylesXmlAccessor(IScripture scr)
		{
			m_scr = scr;
		}

		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// Required implementation of abstract property to get the relative path to stylesheet
		/// configuration file from the FieldWorks install folder.
		/// </summary>
		/// -------------------------------------------------------------------------------------
		protected override string ResourceFilePathFromFwInstall
		{
			get { return @"\Translation Editor\" + ResourceFileName; }
		}

		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// Required implementation of abstract property to get the name of the stylesheet
		/// configuration file.
		/// </summary>
		/// -------------------------------------------------------------------------------------
		protected override string ResourceName
		{
			get { return "TeStyles"; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the DB object which owns the CmResource corresponding to the settings file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override ICmObject ResourceOwner
		{
			get { return m_scr; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the flid of the property in which the CmResources are owned.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override int ResourcesFlid
		{
			get { return (int)Scripture.ScriptureTags.kflidResources; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The collection that owns the styles; for example, Scripture.StylesOC.
		/// </summary>
		/// <value></value>
		/// ------------------------------------------------------------------------------------
		protected override FdoOwningCollection<IStStyle> StyleCollection
		{
			get { return m_scr.StylesOC; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Complain if the context is not valid for the tool that is loading the styles.
		/// TE currently allows all but Flex's private context
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		protected override void ValidateContext(ContextValues context, string styleName)
		{
			if (context == ContextValues.InternalConfigureView)
			{
				ReportInvalidInstallation(String.Format(TeResourceHelper.GetResourceString("ksInvalidStyleContext"),
					styleName, context.ToString(), ResourceFileName));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Throw an exception if the specified context is not valid for the specified paragraph
		/// style. TE overrides to forbid 'general' paragraph styles.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="styleName"></param>
		/// ------------------------------------------------------------------------------------
		protected override void ValidateParagraphContext(ContextValues context, string styleName)
		{
			if (context == ContextValues.General)
				ReportInvalidInstallation(String.Format(TeResourceHelper.GetResourceString("ksInvalidParagraphStyleContext"),
					styleName, context.ToString()));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Special overridable method to allow application-specific overrides to allow a
		/// particular style to be renamed.
		/// </summary>
		/// <param name="styleName">Name of the original style.</param>
		/// <param name="replStyleName">Name of the replacement style.</param>
		/// <returns>
		/// <c>true</c> for replacing specific annotation styles with the Remark style;
		/// <c>false</c> (the base implementation) otherwise.
		/// </returns>
		/// ------------------------------------------------------------------------------------
		protected override bool StyleReplacementAllowed(string styleName, string replStyleName)
		{
			if (replStyleName == ScrStyleNames.Remark)
			{
				switch (styleName)
				{
					case "Notation Discussion":
					case "Notation Quote":
					case "Notation Resolution":
					case "Notation Suggestion":
						return true;
				}
			}
			return base.StyleReplacementAllowed(styleName, replStyleName);
		}

		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// If the current stylesheet version in the Db doesn't match that of the current XML
		/// file, update the DB.
		/// </summary>
		/// <param name="lp">The language project</param>
		/// <param name="progressDlg">The progress dialog from the splash screen</param>
		/// -------------------------------------------------------------------------------------
		public static void EnsureCurrentStylesheet(ILangProject lp, IAdvInd4 progressDlg)
		{
			TeStylesXmlAccessor acc = new TeStylesXmlAccessor(lp.TranslatedScriptureOA);
			acc.EnsureCurrentResource(progressDlg);

			if (acc.UserModifiedStyles.Count > 0)
			{
				using (FwStylesModifiedDlg dlg = new FwStylesModifiedDlg(acc.UserModifiedStyles,
					lp.Cache.ProjectName(), FwApp.App))
				{
					dlg.ShowDialog();
				}
			}
		}

		#region CreateFactoryScrStyles
		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// Create factory styles from the TE Styles XML file.
		/// </summary>
		/// <param name="progressDlg">Progress dialog so the user can cancel</param>
		/// <param name="scr">The Scripture</param>
		/// -------------------------------------------------------------------------------------
		public static void CreateFactoryScrStyles(IAdvInd4 progressDlg, IScripture scr)
		{
			TeStylesXmlAccessor acc = new TeStylesXmlAccessor(scr);
			acc.CreateStyles(progressDlg, scr.StylesOC, acc.LoadDoc());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Define reserved styles
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void InitReservedStyles()
		{
			base.InitReservedStyles();
			if (s_htReservedStyles.Count > 0)
			{
				foreach (string styleName in s_htReservedStyles.Keys)
					s_htReservedStyles[styleName].created = false;
				return;
			}
			// Reserved paragraph styles
			s_htReservedStyles[ScrStyleNames.Normal] = new ReservedStyleInfo(ContextValues.Internal,
				StructureValues.Undefined, FunctionValues.Prose, StyleType.kstParagraph,
				ScrStyleNames.NormalParagraph, null);

			s_htReservedStyles[ScrStyleNames.NormalParagraph] = new ReservedStyleInfo(
				ContextValues.Text,	StructureValues.Body, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.NormalParagraph, ScrStyleNames.Normal);

			s_htReservedStyles[ScrStyleNames.SectionHead] = new ReservedStyleInfo(
				ContextValues.Text, StructureValues.Heading, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.NormalParagraph, ScrStyleNames.Normal);

			s_htReservedStyles[ScrStyleNames.IntroParagraph] = new ReservedStyleInfo(
				ContextValues.Intro, StructureValues.Body, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.IntroParagraph, ScrStyleNames.NormalParagraph);

			s_htReservedStyles[ScrStyleNames.IntroSectionHead] = new ReservedStyleInfo(
				ContextValues.Intro, StructureValues.Heading, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.IntroParagraph, ScrStyleNames.SectionHead);

			s_htReservedStyles[ScrStyleNames.Remark] = new ReservedStyleInfo(
				ContextValues.Annotation, StructureValues.Undefined, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.Remark, ScrStyleNames.Normal);

			s_htReservedStyles[ScrStyleNames.MainBookTitle] = new ReservedStyleInfo(
				ContextValues.Title, StructureValues.Undefined, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.MainBookTitle, ScrStyleNames.SectionHead);

			s_htReservedStyles[ScrStyleNames.NormalFootnoteParagraph] = new ReservedStyleInfo(
				ContextValues.Note, StructureValues.Undefined, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.NormalFootnoteParagraph, ScrStyleNames.NormalParagraph);
			s_htReservedStyles[ScrStyleNames.CrossRefFootnoteParagraph] = new ReservedStyleInfo(
				ContextValues.Note, StructureValues.Undefined, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.CrossRefFootnoteParagraph, ScrStyleNames.NormalFootnoteParagraph);

			// REVIEW JimA or MarkB (TomB): seems odd that the "next" style for Caption should be
			// Paragraph. Doesn't seem to matter though since TE won't let you create a new para
			// when IP is in the caption.
			s_htReservedStyles[ScrStyleNames.Figure] = new ReservedStyleInfo(
				ContextValues.Internal, StructureValues.Undefined, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.NormalParagraph, ScrStyleNames.Normal);

			s_htReservedStyles[ScrStyleNames.Header] = new ReservedStyleInfo(
				ContextValues.Internal, StructureValues.Undefined, FunctionValues.Prose,
				StyleType.kstParagraph, ScrStyleNames.NormalParagraph, ScrStyleNames.Normal);

			// Reserved character styles
			s_htReservedStyles[ScrStyleNames.ChapterNumber] = new ReservedStyleInfo(
				ContextValues.Text, StructureValues.Body, FunctionValues.Chapter);

			s_htReservedStyles[ScrStyleNames.VerseNumber] = new ReservedStyleInfo(
				ContextValues.Text, StructureValues.Body, FunctionValues.Verse);

			s_htReservedStyles[ScrStyleNames.CanonicalRef] = new ReservedStyleInfo(
				ContextValues.InternalMappable, StructureValues.Undefined, FunctionValues.Prose);

			s_htReservedStyles[ScrStyleNames.FootnoteMarker] = new ReservedStyleInfo(
				ContextValues.Internal, StructureValues.Undefined, FunctionValues.Prose);

			s_htReservedStyles[ScrStyleNames.FootnoteTargetRef] = new ReservedStyleInfo(
				ContextValues.InternalMappable, StructureValues.Undefined, FunctionValues.Footnote);

			s_htReservedStyles[ScrStyleNames.UntranslatedWord] = new ReservedStyleInfo(
				ContextValues.BackTranslation, StructureValues.Undefined, FunctionValues.Prose);

			s_htReservedStyles[ScrStyleNames.NotationTag] = new ReservedStyleInfo(
				ContextValues.Internal, StructureValues.Undefined, FunctionValues.Prose);
		}

		#endregion

		#region Style upgrade stuff
		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// Detemine whether the newly proposed function for a style is compatible with its
		/// current function.
		/// </summary>
		/// <param name="currFunction">The existing function of the style</param>
		/// <param name="proposedFunction">The function we want</param>
		/// <returns><c>true </c>if the passed in function can be upgraded as requested;
		/// <c>false</c> otherwise.</returns>
		/// -------------------------------------------------------------------------------------
		public override bool CompatibleFunction(FunctionValues currFunction, FunctionValues proposedFunction)
		{
			return base.CompatibleFunction(currFunction, proposedFunction) ||
				(currFunction == FunctionValues.Line &&
				proposedFunction == FunctionValues.StanzaBreak);
		}

		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// For any footnote in the specified book whose first paragraph has a run marked with
		/// the Note Target Reference style, remove that run and set the property on the footnote
		/// to display the target reference in the footnote view.
		/// </summary>
		/// <param name="book">The ScrBook whose footnotes are to be searched</param>
		/// -------------------------------------------------------------------------------------
		private void RemoveDirectUsesOfFootnoteTargetRef(IScrBook book)
		{
			foreach (StFootnote footnote in book.FootnotesOS)
			{
				// Probably only need to worry about first para
				if (footnote.ParagraphsOS.Count == 0)
					continue;
				StTxtPara para = (StTxtPara)footnote.ParagraphsOS[0];
				ITsString tss = para.Contents.UnderlyingTsString;
				ITsStrBldr bldr = null;
				for (int iRun = 0; iRun < tss.RunCount; iRun++)
				{
					ITsTextProps props = tss.get_Properties(iRun);
					string style = props.GetStrPropValue(
						(int)FwTextPropType.ktptNamedStyle);
					if (style == ScrStyleNames.FootnoteTargetRef)
					{
						if (bldr == null)
							bldr = tss.GetBldr();
						bldr.Replace(tss.get_MinOfRun(iRun), tss.get_LimOfRun(iRun), string.Empty, null);
					}
				}
				if (bldr != null)
				{
					para.Contents.UnderlyingTsString = bldr.GetString();
					footnote.DisplayFootnoteReference = true;
				}
			}
		}

		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// Update the style context and do any special processing needed to deal with existing
		/// data that may be marked with the given style. (Since it was previously not an
		/// internal style, it is possible the user has used it in ways that would be
		/// incompatible with its intended use.) Any time a factory style is changed to an
		/// internal context, specific code must be written here to deal with it. Some possible
		/// options for dealing with this scenario are:
		/// * Delete any data previously marked with the style (and possibly set some other
		///   object properties)
		/// * Add to the m_deletedStyles or m_replacedStyles arrays so existing data will be
		///   marked with a different style (note that this will only work if no existing data
		///   should be preserved with the style).
		/// </summary>
		/// <param name="style">The style being updated</param>
		/// <param name="context">The context (either internal or internal mappable) that the
		/// style is to be given</param>
		/// -------------------------------------------------------------------------------------
		protected override void ChangeFactoryStyleToInternal(IStStyle style, ContextValues context)
		{
			if (!CompatibleContext((ContextValues)style.Context, context))
			{
				if (style.Name == ScrStyleNames.FootnoteTargetRef)
				{
					foreach (IScrBook book in m_scr.ScriptureBooksOS)
						RemoveDirectUsesOfFootnoteTargetRef(book);
					foreach (IScrDraft draft in m_scr.ArchivedDraftsOC)
					{
						foreach (IScrBook book in draft.BooksOS)
							RemoveDirectUsesOfFootnoteTargetRef(book);
					}
				}
				else if (style.InUse)
				{
					// This is where we should handle any future upgrade issues
				}
			}
			style.Context = context;
		}
		#endregion

		#region Helper methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether or not the context of the specified style is excluded (i.e., should
		/// not result in the creation of a TE Style).
		/// </summary>
		/// <param name="context">The context to test</param>
		/// <returns>True if the context is excluded, false otherwise</returns>
		/// ------------------------------------------------------------------------------------
		protected override bool IsExcludedContext(ContextValues context)
		{
			return (base.IsExcludedContext(context) ||
				context == ContextValues.BackMatter ||
				context == ContextValues.Book ||
				context == ContextValues.Publication ||
				context == ContextValues.IntroTitle ||
				context == ContextValues.PsuedoStyle ||
				context == ContextValues.InternalConfigureView /* Only used in FLEx */);
		}

		/// -------------------------------------------------------------------------------------
		/// <summary>
		/// If the proposed context for a style is internal or internalMappable, make sure the
		/// program actually expects and supports this context for this style.
		/// </summary>
		/// <param name="style">The style being updated</param>
		/// <param name="proposedContext">The proposed context for the style</param>
		/// <returns><c>true</c>if the proposed context is internal or internal mappable and
		/// the program recognizes it as a valid</returns>
		/// -------------------------------------------------------------------------------------
		public override bool IsValidInternalStyleContext(IStStyle style,
			ContextValues proposedContext)
		{
			return ((proposedContext == ContextValues.Internal &&
				ScrStyleNames.InternalStyles.Contains(style.Name)) ||
				(proposedContext == ContextValues.InternalMappable &&
				ScrStyleNames.InternalMappableStyles.Contains(style.Name)));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the application specific default font. This is usually the default font, but
		/// in the case of TE it is the body font.
		/// </summary>
		/// <value>The app default font.</value>
		/// ------------------------------------------------------------------------------------
		protected override string AppDefaultFont
		{
			get { return StStyle.DefaultPubFont; }
		}

		/// <summary>
		/// Return true if this is the 'normal' style that all others inherit from. TE overrides.
		/// </summary>
		/// <param name="styleName"></param>
		/// <returns></returns>
		protected override bool IsNormalStyle(string styleName)
		{
			return styleName == ScrStyleNames.Normal;
		}

		#endregion

		#region GetHelpTopicForStyle
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a help topic with an example of how to use the given style. If the given
		/// style is not a factory style, then a general topic about using user-defined styles
		/// is returned.
		/// </summary>
		/// <param name="styleName">The  style name</param>
		/// <returns>A help topic with an example of how to use the given style, or if the given
		/// style is not a factory style, then a general topic about using user-defined styles
		/// </returns>
		/// ------------------------------------------------------------------------------------
		public static string GetHelpTopicForStyle(string styleName)
		{
			if (styleName == FdoResources.DefaultParaCharsStyleName)
			{
				return @"Redirect.htm#its:Using_Styles.chm::/Using_Styles/Styles_Grouped_by_Type/" +
					"Special_Text_and_Character_Styles/Default_Paragraph_Characters_description.htm";
			}

			string editedName = styleName.Replace(" ", "_");
			XmlNode teStyles = new TeStylesXmlAccessor().LoadDoc();
			try
			{
				XmlNode help =
					teStyles.SelectSingleNode("markup/tag[@id='" + editedName + "']/help");

				XmlNode category = null;
				if (help != null) // without this test it sometimes throws a nullref exception, which is annoying while debugging though caught below.
					category = help.Attributes.GetNamedItem("category");
				if (category != null)
				{
					return @"Redirect.htm#its:Using_Styles.chm::/Using_Styles/Styles_Grouped_by_Type/" +
						category.Value + "/" + editedName +
						((editedName == ScrStyleNames.Normal) ? "_description.htm" : "_example.htm");
				}
				XmlNode topic = null;
				if (help != null) // without this test it sometimes throws a nullref exception, which is annoying while debugging though caught below.
					topic = help.Attributes.GetNamedItem("topic");
				if (topic != null)
				{
					return topic.Value;
				}
			}
			catch
			{
			}
			return @"Advanced_Tasks/Customizing_Styles/User-defined_style.htm";
		}
		#endregion
	}
}
