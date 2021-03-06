﻿using System;
using System.Windows.Forms;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.Common.Framework.DetailControls;
using SIL.FieldWorks.Common.RootSites;
using SIL.FieldWorks.FdoUi;

namespace SIL.FieldWorks.XWorks.MorphologyEditor
{
	/// <summary>
	/// This is a view slice that contains a <c>RuleFormulaControl</c>. It is extended by
	/// phonological/morphological rule slices.
	/// </summary>
	public class RuleFormulaSlice : ViewSlice, XCore.IxCoreColleague
	{
		public RuleFormulaSlice()
		{
		}

		public override RootSite RootSite
		{
			get
			{
				CheckDisposed();
				return RuleFormulaControl.RootSite;
			}
		}

		public RuleFormulaControl RuleFormulaControl
		{
			get
			{
				CheckDisposed();
				return Control as RuleFormulaControl;
			}
		}

		protected override void Dispose(bool disposing)
		{
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if (disposing)
			{
				RuleFormulaControl.InsertionControl.SizeChanged -= new EventHandler(InsertionControl_SizeChanged);
			}

			base.Dispose(disposing);
		}

		void InsertionControl_SizeChanged(object sender, EventArgs e)
		{
			// resize entire slice when the insertion control changes size
			Height = DesiredHeight(RuleFormulaControl.RootSite);
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);

			// show the insertion control
			RuleFormulaControl.InsertionControl.Show();
			Height = DesiredHeight(RuleFormulaControl.RootSite);
			RuleFormulaControl.PerformLayout();
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);

			// hide the insertion control
			RuleFormulaControl.InsertionControl.Hide();
			if (RuleFormulaControl.RootSite.RootBox != null && ContainingDataTree != null)
				Height = DesiredHeight(RuleFormulaControl.RootSite);
		}

		protected override int DesiredHeight(RootSite rs)
		{
			int height = base.DesiredHeight(rs);
			// only include the height of the insertion contorl when it is visible
			if (RuleFormulaControl.InsertionControl.Visible)
				height += RuleFormulaControl.InsertionControl.Height;
			return height;
		}

		public override void Install(DataTree parent)
		{
			CheckDisposed();
			base.Install(parent);

			RuleFormulaControl.Initialize((FdoCache)Mediator.PropertyTable.GetValue("cache"), Object, -1, MEStrings.ksRuleEnvChooserName,
				ContainingDataTree.PersistenceProvder, Mediator, null, null);

			RuleFormulaControl.InsertionControl.Hide();
			RuleFormulaControl.InsertionControl.SizeChanged += new EventHandler(InsertionControl_SizeChanged);
		}

		public bool OnDisplayContextSetFeatures(object commandObject, ref XCore.UIItemDisplayProperties display)
		{
			CheckDisposed();
			bool enable = RuleFormulaControl.IsFeatsNCContextCurrent;
			display.Enabled = enable;
			display.Visible = enable;
			return true;
		}

		public bool OnContextSetFeatures(object args)
		{
			CheckDisposed();
			RuleFormulaControl.SetContextFeatures();
			return true;
		}

		public bool OnDisplayContextJumpToNaturalClass(object commandObject, ref XCore.UIItemDisplayProperties display)
		{
			CheckDisposed();
			bool enable = RuleFormulaControl.IsNCContextCurrent;
			display.Enabled = enable;
			display.Visible = enable;
			return true;
		}

		public bool OnContextJumpToNaturalClass(object args)
		{
			CheckDisposed();
			int hvo = RuleFormulaControl.CurrentContextHvo;
			IPhSimpleContextNC ctxt = new PhSimpleContextNC(m_cache, hvo);
			Mediator.PostMessage("FollowLink", FwLink.Create("naturalClassedit",
				m_cache.GetGuidFromId(ctxt.FeatureStructureRAHvo), m_cache.ServerName, m_cache.DatabaseName));
			return true;
		}

		public virtual bool OnDisplayContextJumpToPhoneme(object commandObject, ref XCore.UIItemDisplayProperties display)
		{
			CheckDisposed();
			bool enable = RuleFormulaControl.IsPhonemeContextCurrent;
			display.Enabled = enable;
			display.Visible = enable;
			return true;
		}

		public virtual bool OnContextJumpToPhoneme(object args)
		{
			CheckDisposed();
			int hvo = RuleFormulaControl.CurrentContextHvo;
			IPhSimpleContextSeg ctxt = new PhSimpleContextSeg(m_cache, hvo);
			Mediator.PostMessage("FollowLink", FwLink.Create("phonemeEdit",
				m_cache.GetGuidFromId(ctxt.FeatureStructureRAHvo), m_cache.ServerName, m_cache.DatabaseName));
			return true;
		}
	}
}
