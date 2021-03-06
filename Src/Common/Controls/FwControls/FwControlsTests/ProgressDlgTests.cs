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
// File: ProgressDlgTests.cs
// Responsibility: _Aman
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Threading;
using NUnit.Framework;
using SIL.FieldWorks.Common.COMInterfaces;

namespace SIL.FieldWorks.Common.Controls
{
	#region DummyProgressDlg
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class DummyProgressDlg : ProgressDialogWithTask
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DummyProgressDlg"/> class.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public DummyProgressDlg() : base(null)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating wether or not the cancel button is visible.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool IsCancelVisible
		{
			get
			{
				CheckDisposed();
				return CancelButtonVisible;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Simulates pressing the cancel button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void SimulatePressCancel()
		{
			PressCancelButton();
		}
	}
	#endregion

	#region ProgressDlgTests
	/// ---------------------------------------------------------------------------------------
	/// <summary>
	/// Summary description for ProgressDlgTests.
	/// </summary>
	/// ---------------------------------------------------------------------------------------
	[TestFixture]
	public class ProgressDlgTests
	{
		private DummyProgressDlg m_dlg;
		private volatile bool m_fGotCancel;
		private System.Threading.Timer m_timer;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Set up the test
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[SetUp]
		public void Setup()
		{
			m_fGotCancel = false;

			m_dlg = new DummyProgressDlg();
			m_dlg.Cancel += new CancelHandler(HandleCancel);
			m_dlg.Maximum = 10;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Teardowns this instance.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[TearDown]
		public void Teardown()
		{
			if (m_timer != null)
			{
				m_timer.Dispose();
				m_timer = null;
			}

			if (m_dlg != null)
			{
				m_dlg.Dispose();
				m_dlg = null;
			}

		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handle our dummy's cancel event.
		/// </summary>
		/// <param name="sender"></param>
		/// ------------------------------------------------------------------------------------
		private void HandleCancel(object sender)
		{
			// m_fGotCancel is volatile, so no need to lock explicitly.
			m_fGotCancel = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This is a timer callback that gets fired to cancel the dialog.
		/// </summary>
		/// <param name="state">Minnesota, Illinois, etc.</param>
		/// ------------------------------------------------------------------------------------
		private void CancelDialog(object state)
		{
			if (m_dlg == null)
				return;

			m_dlg.SimulatePressCancel();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a timer and kicks it off.
		/// </summary>
		/// <param name="delay">How many milliseconds until the timer delegate gets fired.
		/// </param>
		/// ------------------------------------------------------------------------------------
		private void StartTimer(long delay)
		{
			// Wait for the simulated cancel button, but if we don't get it after 'delay'
			// milliseconds, give up and close it when the following timer runs out.
			m_timer = new System.Threading.Timer(
				new System.Threading.TimerCallback(CancelDialog),
				null, delay, System.Threading.Timeout.Infinite);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The task to do in the background. We just increment the progress and sleep in
		/// between.
		/// </summary>
		/// <param name="progressDlg">The progress dialog.</param>
		/// <param name="parameters">The parameters. Not used.</param>
		/// <returns>Current progress</returns>
		/// ------------------------------------------------------------------------------------
		private object BackgroundTask(IAdvInd4 progressDlg, object[] parameters)
		{
			int i = 0;
			for (; i < 10; i++)
			{
				if (m_fGotCancel)
					break;

				progressDlg.Step(1);
				Thread.Sleep(1000);
			}

			return i;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the progress dialog box with a cancel button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TestWithCancel()
		{
			StartTimer(2500);

			int nProgress = (int)m_dlg.RunTask(false, new BackgroundTaskInvoker(BackgroundTask));

			Assert.Less(nProgress, 10);
			Assert.IsTrue(m_fGotCancel);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test the progress dialog and verifies the cancel button doesn't show when there
		/// isn't a Cancel delegate specified. Also verifies the count is correct at the end,
		/// although that's not an incredibly exciting test.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TestWithoutCancel()
		{
			int nProgress = (int)m_dlg.RunTask(false, new BackgroundTaskInvoker(BackgroundTask));

			Assert.AreEqual(10, nProgress);
			Assert.IsFalse(m_fGotCancel);
		}
	}
	#endregion
}
