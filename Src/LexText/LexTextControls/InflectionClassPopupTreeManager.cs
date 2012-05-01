using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.FDO.LangProj;
using SIL.FieldWorks.Common.Widgets;
using SIL.FieldWorks.Common.COMInterfaces;
using XCore;

namespace SIL.FieldWorks.LexText.Controls
{
	/// <summary>
	/// Handles a TreeCombo control (Widgets assembly) for use in selecting inflection classes.
	/// </summary>
	public class InflectionClassPopupTreeManager : PopupTreeManager
	{
		private const int kEmpty = 0;
		private const int kLine = -1;
		private const int kMore = -2;

		#region Data members

		#endregion Data members

		#region Events

		#endregion Events

		/// <summary>
		/// Constructor.
		/// </summary>
		public InflectionClassPopupTreeManager(TreeCombo treeCombo, FdoCache cache,  bool useAbbr, Form parent, int wsDisplay)
			: base(treeCombo, cache, cache.LangProject.PartsOfSpeechOA, wsDisplay, useAbbr, parent)
		{
		}

		/// <summary>
		/// Traverse a tree of objects.
		///	Put the appropriate descendant identifiers into collector.
		/// </summary>
		/// <param name="cache">data access to retrieve info</param>
		/// <param name="rootHvo">starting object</param>
		/// <param name="rootFlid">the children of rootHvo are in this property.</param>
		/// <param name="subFlid">grandchildren and great...grandchildren are in this one</param>
		/// <param name="itemFlid">want children where this is non-empty in the collector</param>
		/// <param name="flidName">multistring prop to get name of item from</param>
		/// <param name="wsName">multistring writing system to get name of item from</param>
		/// <param name="collector">Add for each item an HvoTreeNode with the name and id of the item.</param>
		internal static void GatherPartsOfSpeech(FdoCache cache,
			int rootHvo, int rootFlid, int subFlid, int itemFlid, int flidName, int wsName, List<HvoTreeNode> collector)
		{
			ISilDataAccess sda = cache.MainCacheAccessor;
			int chvo = sda.get_VecSize(rootHvo, rootFlid);
			for (int ihvo = 0; ihvo < chvo; ihvo++)
			{
				int hvoItem = sda.get_VecItem(rootHvo, rootFlid, ihvo);
				if (sda.get_VecSize(hvoItem, itemFlid) > 0)
				{
					ITsString tssLabel = GetTssLabel(cache, hvoItem, flidName, wsName);
					collector.Add(new HvoTreeNode(tssLabel, hvoItem));
				}
				GatherPartsOfSpeech(cache, hvoItem, subFlid, subFlid, itemFlid, flidName, wsName, collector);
			}
		}

		protected override TreeNode MakeMenuItems(PopupTree popupTree, int hvoTarget)
		{
			int tagNamePOS = UseAbbr ?
				(int)CmPossibility.CmPossibilityTags.kflidAbbreviation :
				(int)CmPossibility.CmPossibilityTags.kflidName;


			List<HvoTreeNode> relevantPartsOfSpeech = new List<HvoTreeNode>();
			GatherPartsOfSpeech(Cache, List.Hvo, (int)CmPossibilityList.CmPossibilityListTags.kflidPossibilities,
				(int)CmPossibility.CmPossibilityTags.kflidSubPossibilities,
				(int)PartOfSpeech.PartOfSpeechTags.kflidInflectionClasses,
				tagNamePOS, WritingSystem,
				relevantPartsOfSpeech);
			relevantPartsOfSpeech.Sort();
			int tagNameClass = UseAbbr ?
				(int) MoInflClass.MoInflClassTags.kflidAbbreviation :
				(int) MoInflClass.MoInflClassTags.kflidName;
			TreeNode match = null;
			foreach(HvoTreeNode item in relevantPartsOfSpeech)
			{
				popupTree.Nodes.Add(item);
				TreeNode match1 = AddNodes(item.Nodes, item.Hvo,
					(int)PartOfSpeech.PartOfSpeechTags.kflidInflectionClasses,
					(int)MoInflClass.MoInflClassTags.kflidSubclasses,
					hvoTarget, tagNameClass);
				if (match1 != null)
					match = match1;
			}
			return match;
		}
	}
}