// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2003, SIL International. All Rights Reserved.
// <copyright from='2003' to='2003' company='SIL International'>
//		Copyright (c) 2003, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: GotoReferenceDialog.cs
// Responsibility: TE Team
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Windows.Forms;
using SIL.FieldWorks.Common.Controls;
using SIL.FieldWorks.Common.Drawing;
using SIL.FieldWorks.Common.Framework;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.Common.ScriptureUtils;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Scripture;
using SIL.Utils;

namespace SIL.FieldWorks.TE
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// GotoReferenceDialog class.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class GotoReferenceDialog : Form, IFWDisposable
	{
		private ScrPassageControl scrPassageControl;
		//		private System.Windows.Forms.Button btn_ok;
		private IScripture m_scripture;
		private Button btn_help;
		private Button btn_cancel;
		private Button btn_OK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Default constructor - needed for Designer
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public GotoReferenceDialog() : this(ScrReference.Empty, null)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="GotoReferenceDialog"/> class.
		/// </summary>
		/// <param name="reference">The initial reference to populate the control.</param>
		/// <param name="scr">The Scripture object.</param>
		/// ------------------------------------------------------------------------------------
		public GotoReferenceDialog(ScrReference reference, IScripture scr)
		{
			Logger.WriteEvent("Opening 'Goto Reference' dialog");

			m_scripture = scr;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			scrPassageControl = new ScrPassageControl(reference, (Scripture)m_scripture, true);
			scrPassageControl.Location = new System.Drawing.Point(16, 16);
			scrPassageControl.Name = "scrPassageControl";
			scrPassageControl.Size = new System.Drawing.Size(Width - 36, 24);
			Controls.Add(scrPassageControl);

			scrPassageControl.TabIndex = 0;
			btn_OK.TabIndex = 1;
			btn_cancel.TabIndex = 2;
			btn_help.TabIndex = 3;
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

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged
		/// resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		/// -----------------------------------------------------------------------------------
		protected override void Dispose( bool disposing )
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GotoReferenceDialog));
			this.btn_cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			this.btn_help = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// btn_cancel
			//
			this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btn_cancel, "btn_cancel");
			this.btn_cancel.Name = "btn_cancel";
			//
			// btn_OK
			//
			resources.ApplyResources(this.btn_OK, "btn_OK");
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Click += new System.EventHandler(this.btn_ok_Click);
			//
			// btn_help
			//
			resources.ApplyResources(this.btn_help, "btn_help");
			this.btn_help.Name = "btn_help";
			this.btn_help.Click += new System.EventHandler(this.btn_help_Click);
			//
			// GotoReferenceDialog
			//
			this.AcceptButton = this.btn_OK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btn_cancel;
			this.Controls.Add(this.btn_OK);
			this.Controls.Add(this.btn_help);
			this.Controls.Add(this.btn_cancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GotoReferenceDialog";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);

		}
		#endregion

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The ScReference for where we want to go today.
		/// </summary>
		/// <value>The sc reference.</value>
		/// ------------------------------------------------------------------------------------
		public ScrReference ScReference
		{
			get
			{
				CheckDisposed();

				return scrPassageControl.ScReference;
			}
		}
		#endregion

		#region Event handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// Draw the etched, horizontal line separating the wizard buttons
			// from the rest of the form.
			LineDrawing.DrawDialogControlSeparator(e.Graphics, ClientRectangle, btn_help.Bounds);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		protected override void OnClosing(CancelEventArgs e)
		{
			Logger.WriteEvent("Closing 'Goto Reference' dialog with result " +
				DialogResult.ToString());
			base.OnClosing (e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handle a click on the OK button
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;

			// If the text has been edited in the scripture passage control, then the reference
			// may not have been parsed yet, so make sure it gets parsed.
			scrPassageControl.ResolveReference();

			Close();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Open the help window when the help button is pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// ------------------------------------------------------------------------------------
		private void btn_help_Click(object sender, System.EventArgs e)
		{
			ShowHelp.ShowHelpTopic(FwApp.App, "khtpGoToReference");
		}
		#endregion
	}
}
