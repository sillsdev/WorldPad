﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIL.FieldWorks.Common.FwUtils {
	using System;


	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	// This class was auto-generated by the StronglyTypedResourceBuilder
	// class via a tool like ResGen or Visual Studio.
	// To add or remove a member, edit your .ResX file then rerun ResGen
	// with the /str option, or rebuild your VS project.
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class FwUtilsStrings {

		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal FwUtilsStrings() {
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SIL.FieldWorks.Common.FwUtils.FwUtilsStrings", typeof(FwUtilsStrings).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Selected scripture sections are included in your text corpus and are available for interlinearization. Each section becomes a separate text.
		///
		///If you clear a check box for a section it is no longer included as a text, however its words remain in the list of wordforms and any examples that reference it remain in the Lexicon area. If you include that section again, the interlinearization is remembered. You will need to refresh the window (F5) to see recent edits in the section. Check boxes with a gray check  [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string khtpScrSectionFilter {
			get {
				return ResourceManager.GetString("khtpScrSectionFilter", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The help file could not be located..
		/// </summary>
		internal static string ksCannotFindHelp {
			get {
				return ResourceManager.GetString("ksCannotFindHelp", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to An error occurred on line {0} offset {1} between the text &quot;{2}&quot; and &quot;{3}&quot;..
		/// </summary>
		internal static string ksErrorOnLineX {
			get {
				return ResourceManager.GetString("ksErrorOnLineX", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to The help topic is not available for topic: {0}.
		/// </summary>
		internal static string ksNoHelpTopicX {
			get {
				return ResourceManager.GetString("ksNoHelpTopicX", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Include Scripture Help.
		/// </summary>
		internal static string ksScrCaption {
			get {
				return ResourceManager.GetString("ksScrCaption", resourceCulture);
			}
		}
	}
}
