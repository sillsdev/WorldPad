namespace SIL.FieldWorks.LexText.Controls
{
	partial class LexOptionsDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LexOptionsDlg));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.m_tabInterface = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.m_userInterfaceChooser = new SIL.FieldWorks.Common.Widgets.UserInterfaceChooser();
			this.m_tabPlugins = new System.Windows.Forms.TabPage();
			this.m_labelRights = new System.Windows.Forms.Label();
			this.m_labelPluginBlurb = new System.Windows.Forms.Label();
			this.m_lvPlugins = new System.Windows.Forms.ListView();
			this.m_chName = new System.Windows.Forms.ColumnHeader();
			this.m_chDescription = new System.Windows.Forms.ColumnHeader();
			this.m_btnOK = new System.Windows.Forms.Button();
			this.m_btnCancel = new System.Windows.Forms.Button();
			this.m_btnHelp = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.m_tabInterface.SuspendLayout();
			this.m_tabPlugins.SuspendLayout();
			this.SuspendLayout();
			//
			// label1
			//
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			//
			// tabControl1
			//
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Controls.Add(this.m_tabInterface);
			this.tabControl1.Controls.Add(this.m_tabPlugins);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			//
			// m_tabInterface
			//
			resources.ApplyResources(this.m_tabInterface, "m_tabInterface");
			this.m_tabInterface.Controls.Add(this.label2);
			this.m_tabInterface.Controls.Add(label1);
			this.m_tabInterface.Controls.Add(this.m_userInterfaceChooser);
			this.m_tabInterface.Name = "m_tabInterface";
			this.m_tabInterface.UseVisualStyleBackColor = true;
			//
			// label2
			//
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			//
			// m_userInterfaceChooser
			//
			resources.ApplyResources(this.m_userInterfaceChooser, "m_userInterfaceChooser");
			this.m_userInterfaceChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_userInterfaceChooser.FormattingEnabled = true;
			this.m_userInterfaceChooser.Name = "m_userInterfaceChooser";
			this.m_userInterfaceChooser.Sorted = true;
			//
			// m_tabPlugins
			//
			this.m_tabPlugins.Controls.Add(this.m_labelRights);
			this.m_tabPlugins.Controls.Add(this.m_labelPluginBlurb);
			this.m_tabPlugins.Controls.Add(this.m_lvPlugins);
			resources.ApplyResources(this.m_tabPlugins, "m_tabPlugins");
			this.m_tabPlugins.Name = "m_tabPlugins";
			this.m_tabPlugins.UseVisualStyleBackColor = true;
			//
			// m_labelRights
			//
			resources.ApplyResources(this.m_labelRights, "m_labelRights");
			this.m_labelRights.Name = "m_labelRights";
			//
			// m_labelPluginBlurb
			//
			resources.ApplyResources(this.m_labelPluginBlurb, "m_labelPluginBlurb");
			this.m_labelPluginBlurb.Name = "m_labelPluginBlurb";
			//
			// m_lvPlugins
			//
			this.m_lvPlugins.CheckBoxes = true;
			this.m_lvPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.m_chName,
			this.m_chDescription});
			this.m_lvPlugins.FullRowSelect = true;
			this.m_lvPlugins.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			resources.ApplyResources(this.m_lvPlugins, "m_lvPlugins");
			this.m_lvPlugins.MultiSelect = false;
			this.m_lvPlugins.Name = "m_lvPlugins";
			this.m_lvPlugins.UseCompatibleStateImageBehavior = false;
			this.m_lvPlugins.View = System.Windows.Forms.View.Details;
			//
			// m_chName
			//
			resources.ApplyResources(this.m_chName, "m_chName");
			//
			// m_chDescription
			//
			resources.ApplyResources(this.m_chDescription, "m_chDescription");
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
			this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btnCancel.Name = "m_btnCancel";
			this.m_btnCancel.UseVisualStyleBackColor = true;
			this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
			//
			// m_btnHelp
			//
			resources.ApplyResources(this.m_btnHelp, "m_btnHelp");
			this.m_btnHelp.Name = "m_btnHelp";
			this.m_btnHelp.UseVisualStyleBackColor = true;
			this.m_btnHelp.Click += new System.EventHandler(this.m_btnHelp_Click);
			//
			// LexOptionsDlg
			//
			this.AcceptButton = this.m_btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.m_btnCancel;
			this.Controls.Add(this.m_btnHelp);
			this.Controls.Add(this.m_btnCancel);
			this.Controls.Add(this.m_btnOK);
			this.Controls.Add(this.tabControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LexOptionsDlg";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.tabControl1.ResumeLayout(false);
			this.m_tabInterface.ResumeLayout(false);
			this.m_tabInterface.PerformLayout();
			this.m_tabPlugins.ResumeLayout(false);
			this.m_tabPlugins.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage m_tabInterface;
		private System.Windows.Forms.Button m_btnOK;
		private System.Windows.Forms.Button m_btnCancel;
		private System.Windows.Forms.Button m_btnHelp;
		private SIL.FieldWorks.Common.Widgets.UserInterfaceChooser m_userInterfaceChooser;
		private System.Windows.Forms.TabPage m_tabPlugins;
		private System.Windows.Forms.Label m_labelPluginBlurb;
		private System.Windows.Forms.ListView m_lvPlugins;
		private System.Windows.Forms.ColumnHeader m_chName;
		private System.Windows.Forms.ColumnHeader m_chDescription;
		private System.Windows.Forms.Label m_labelRights;
		private System.Windows.Forms.Label label2;
	}
}