﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;

using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.Common.Framework;
using SIL.FieldWorks.Common.FwUtils;

namespace SIL.FieldWorks.XWorks.MorphologyEditor
{
	public class ContextOccurrenceDlg : Form, IFWDisposable
	{
		const string s_helpTopic = "khtpCtxtOccur";

		private Label m_lblMin;
		private Label m_lblMax;
		private Button m_btnOK;
		private Button m_btnCancel;
		private ComboBox m_cboMax;
		private Button m_btnHelp;
		private HelpProvider m_helpProvider;
		private Label m_bracketLabel;
		private ComboBox m_cboMin;

		public ContextOccurrenceDlg(int min, int max)
		{
			InitializeComponent();

			m_cboMin.SelectedIndex = min;
			m_cboMax.SelectedIndex = max == -1 ? 0 : max;

			m_helpProvider.HelpNamespace = FwApp.App.HelpFile;
			m_helpProvider.SetHelpKeyword(this, FwApp.App.GetHelpString(s_helpTopic, 0));
			m_helpProvider.SetHelpNavigator(this, HelpNavigator.Topic);
		}

		public int Minimum
		{
			get
			{
				CheckDisposed();
				return m_cboMin.SelectedIndex;
			}
		}

		public int Maximum
		{
			get
			{
				CheckDisposed();
				return m_cboMax.SelectedIndex == 0 ? -1 : m_cboMax.SelectedIndex;
			}
		}

		/// <summary>
		/// Check to see if the object has been disposed.
		/// All public Properties and Methods should call this
		/// before doing anything else.
		/// </summary>
		public void CheckDisposed()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(String.Format("'{0}' in use after being disposed.", GetType().Name));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if (disposing)
			{
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContextOccurrenceDlg));
			this.m_lblMin = new System.Windows.Forms.Label();
			this.m_lblMax = new System.Windows.Forms.Label();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_cboMax = new System.Windows.Forms.ComboBox();
			this.m_cboMin = new System.Windows.Forms.ComboBox();
			this.m_btnHelp = new System.Windows.Forms.Button();
			this.m_helpProvider = new System.Windows.Forms.HelpProvider();
			this.m_bracketLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			// m_lblMin
			//
			resources.ApplyResources(this.m_lblMin, "m_lblMin");
			this.m_lblMin.Name = "m_lblMin";
			//
			// m_lblMax
			//
			resources.ApplyResources(this.m_lblMax, "m_lblMax");
			this.m_lblMax.Name = "m_lblMax";
			//
			// m_btnOK
			//
			resources.ApplyResources(this.m_btnOK, "m_btnOK");
			this.m_btnOK.Name = "m_btnOK";
			this.m_btnOK.UseVisualStyleBackColor = true;
			this.m_btnOK.Click += new System.EventHandler(this.m_btnOK_Click);
			//
			// m_btnCancel
			//
			resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
			//
			// m_cboMax
			//
			this.m_cboMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboMax.FormattingEnabled = true;
			this.m_cboMax.Items.AddRange(new object[] {
			resources.GetString("m_cboMax.Items"),
			resources.GetString("m_cboMax.Items1"),
			resources.GetString("m_cboMax.Items2"),
			resources.GetString("m_cboMax.Items3"),
			resources.GetString("m_cboMax.Items4"),
			resources.GetString("m_cboMax.Items5"),
			resources.GetString("m_cboMax.Items6"),
			resources.GetString("m_cboMax.Items7"),
			resources.GetString("m_cboMax.Items8"),
			resources.GetString("m_cboMax.Items9")});
			resources.ApplyResources(this.m_cboMax, "m_cboMax");
			this.m_cboMax.Name = "m_cboMax";
			//
			// m_cboMin
			//
			this.m_cboMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cboMin.FormattingEnabled = true;
			this.m_cboMin.Items.AddRange(new object[] {
			resources.GetString("m_cboMin.Items"),
			resources.GetString("m_cboMin.Items1"),
			resources.GetString("m_cboMin.Items2"),
			resources.GetString("m_cboMin.Items3"),
			resources.GetString("m_cboMin.Items4"),
			resources.GetString("m_cboMin.Items5"),
			resources.GetString("m_cboMin.Items6"),
			resources.GetString("m_cboMin.Items7"),
			resources.GetString("m_cboMin.Items8"),
			resources.GetString("m_cboMin.Items9")});
			resources.ApplyResources(this.m_cboMin, "m_cboMin");
			this.m_cboMin.Name = "m_cboMin";
			//
			// m_btnHelp
			//
			resources.ApplyResources(this.m_btnHelp, "m_btnHelp");
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.m_btnHelp_Click);
			//
			// m_bracketLabel
			//
			resources.ApplyResources(this.m_bracketLabel, "m_bracketLabel");
			this.m_bracketLabel.Name = "m_bracketLabel";
			//
			// ContextOccurrenceDlg
			//
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_cboMax);
			this.Controls.Add(this.m_cboMin);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.m_lblMax);
			this.Controls.Add(this.m_lblMin);
			this.Controls.Add(this.m_bracketLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ContextOccurrenceDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void m_btnOK_Click(object sender, EventArgs e)
		{
			if (Maximum == -1 || Minimum <= Maximum)
				DialogResult = DialogResult.OK;
			else
				MessageBox.Show(MEStrings.ksContextOccurrenceDlgError);
		}

		private void m_btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void m_btnHelp_Click(object sender, EventArgs e)
		{
			ShowHelp.ShowHelpTopic(FwApp.App, s_helpTopic);
		}
	}
}
