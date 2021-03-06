// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2004, SIL International. All Rights Reserved.
// <copyright from='2004' to='2004' company='SIL International'>
//		Copyright (c) 2004, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: ParserAnalysisRemover.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.Common.Controls;
using XCore;
using SIL.FieldWorks.FdoUi;

namespace SIL.FieldWorks.XWorks.MorphologyEditor
{
	/// <summary>
	/// This class serves to remove all analyses that are only approved by the parser.
	/// Analyses that have a human evaluation (approved or disapproved) remain afterwards.
	/// </summary>
	public class ParserAnalysisRemover : IUtility
	{
		#region Data members

		private UtilityDlg m_dlg;
		const string m_ksPath = "/group[@id='Linguistics']/group[@id='Morphology']/group[@id='RemoveParserAnalyses']/";

		#endregion Data members

		#region Construction

		/// <summary>
		/// Constructor.
		/// </summary>
		public ParserAnalysisRemover()
		{
		}

		#endregion Construction

		/// <summary>
		/// Override method to return the Label property.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Label;
		}

		#region IUtility implementation

		/// <summary>
		/// Get the main label describing the utility.
		/// </summary>
		public string Label
		{
			get
			{
				Debug.Assert(m_dlg != null);
				return m_dlg.Mediator.StringTbl.GetStringWithXPath("Label", m_ksPath);
			}
		}

		/// <summary>
		/// Set the UtilityDlg.
		/// </summary>
		/// <remarks>
		/// This must be set, before calling any other property or method.
		/// </remarks>
		public UtilityDlg Dialog
		{
			set
			{
				Debug.Assert(value != null);
				Debug.Assert(m_dlg == null);

				m_dlg = value;
			}
		}

		/// <summary>
		/// Load 0 or more items in the list box.
		/// </summary>
		public void LoadUtilities()
		{
			Debug.Assert(m_dlg != null);
			m_dlg.Utilities.Items.Add(this);

		}

		/// <summary>
		/// Notify the utility that has been selected in the dlg.
		/// </summary>
		public void OnSelection()
		{
			Debug.Assert(m_dlg != null);
			m_dlg.WhenDescription = m_dlg.Mediator.StringTbl.GetStringWithXPath("WhenDescription", m_ksPath);
			m_dlg.WhatDescription = m_dlg.Mediator.StringTbl.GetStringWithXPath("WhatDescription", m_ksPath);
			m_dlg.RedoDescription = m_dlg.Mediator.StringTbl.GetStringWithXPath("RedoDescription", m_ksPath);
		}

		/// <summary>
		/// Have the utility do what it does.
		/// </summary>
		public void Process()
		{
			Debug.Assert(m_dlg != null);
			FdoCache cache = (FdoCache)m_dlg.Mediator.PropertyTable.GetValue("cache");
			List<int> wordformIds = DbOps.ReadIntsFromCommand(
				cache,
				"SELECT Id FROM WfiWordform",
				null);
			int curObjId = 0;
			if (m_dlg.Mediator != null)
			{
				Mediator mediator = m_dlg.Mediator;
				RecordClerk activeClerk = (RecordClerk)mediator.PropertyTable.GetValue("ActiveClerk");
				if (activeClerk != null && activeClerk.Id == "concordanceWords" && activeClerk.CurrentObject != null)
					curObjId = activeClerk.CurrentObject.Hvo;
			}

			// Set up progress bar.
			m_dlg.ProgressBar.Minimum = 0;
			m_dlg.ProgressBar.Maximum = wordformIds.Count;
			m_dlg.ProgressBar.Step = 1;

			// stop parser if it's running.
			m_dlg.Mediator.SendMessage("StopParser", null);

			if (wordformIds.Count > 0)
			{
				if (cache.DatabaseAccessor.IsTransactionOpen())
					cache.DatabaseAccessor.CommitTrans();
				foreach (int wfId in wordformIds)
				{
					cache.DatabaseAccessor.BeginTrans();
					DbOps.ExecuteStoredProc(
						cache,
						string.Format("EXEC RemoveParserApprovedAnalyses$ {0}",wfId),
						null);
					cache.DatabaseAccessor.CommitTrans();
					using (WfiWordformUi wfui = new WfiWordformUi(WfiWordform.CreateFromDBObject(cache, wfId)))
					{
						wfui.UpdateWordsToolDisplay(curObjId, false, false, true, true);
					}
					m_dlg.ProgressBar.PerformStep();
				}
			}
			else
				m_dlg.ProgressBar.PerformStep();
		}

		#endregion IUtility implementation
	}
}
