using System;
using System.Collections;
using System.Windows.Forms;

using NUnit.Framework;

using SIL.FieldWorks.Common.Utils;

namespace XCore
{
	/// <summary>
	/// TestMessageSequencer.
	/// </summary>
	[TestFixture]
	public class TestMessageSequencer
	{
		public TestMessageSequencer()
		{
		}

		private class DummyForm : Form
		{
			/// --------------------------------------------------------------------------------
			/// <summary>
			/// Creates the handle, but doesn't show the form
			/// </summary>
			/// --------------------------------------------------------------------------------
			public void Create()
			{
				Visible = false;
				CreateHandle();
			}
		}
		/// <summary>
		/// Test non-reentrant operation
		/// </summary>
		[Test]
		public void NormalOperation()
		{
			// need to create a dummy form because PaintEventArgs wants a Graphics object
			using (DummyForm dummy = new DummyForm())
			{
				dummy.Create();
				TestControl1 tc1 = new TestControl1();
				Message m1 = Message.Create(IntPtr.Zero, 10, new IntPtr(101), new IntPtr(1001));
				tc1.CallWndProc(ref m1);
				Message m2 = Message.Create(IntPtr.Zero, 20, new IntPtr(201), new IntPtr(2001));
				tc1.CallWndProc(ref m2);
				using (System.Drawing.Graphics g = dummy.CreateGraphics())
				{
					PaintEventArgs pe1 = new PaintEventArgs(g, new System.Drawing.Rectangle(0, 1, 2, 3));
					tc1.CallOnPaint(pe1);
					Message m3 = Message.Create(IntPtr.Zero, 30, new IntPtr(301), new IntPtr(3001));
					tc1.CallWndProc(ref m3);
					object[] expected = { m1, m1, m2, m2, pe1, pe1, m3, m3 };
					VerifyArray(expected, tc1);
				}
			}
		}

		private void VerifyArray(object[] expected, TestControl1 tc1)
		{
			Assert.AreEqual(expected.Length, tc1.m_messages.Count);
			for (int i = 0; i < expected.Length; i++)
				Assert.AreEqual(expected[i], tc1.m_messages[i], "unexpected object at " + i);
		}

		/// <summary>
		/// Test reentrant operation
		/// </summary>
		[Test]
		public void KillFocusAndGetFocusInActivateInKeyDown()
		{
			// Similate a Keydown, inside which we get an MdiActivate, inside which we get a kill focus and a GetFocus.
			// Since activate is not being sequenced, but the other two are, we expect the sequence
			// start of keydown, start of activate, end of activate, end of keydown, start of killfocus, end of killfocus, start of get focus, end of get focus.
			Message msgKeyDown = Message.Create(IntPtr.Zero, (int)Win32.WinMsgs.WM_KEYDOWN, new IntPtr(101), new IntPtr(1001));
			Message msgActivate = Message.Create(IntPtr.Zero, (int)Win32.WinMsgs.WM_MDIACTIVATE, new IntPtr(201), new IntPtr(2001));
			Message msgKillFocus = Message.Create(IntPtr.Zero, (int)Win32.WinMsgs.WM_KILLFOCUS, new IntPtr(201), new IntPtr(2001));
			Message msgSetFocus = Message.Create(IntPtr.Zero, (int)Win32.WinMsgs.WM_SETFOCUS, new IntPtr(301), new IntPtr(3001));
			TestControl2 tc2 = new TestControl2(msgKeyDown, msgActivate, msgKillFocus, msgSetFocus);

			tc2.CallWndProc(ref msgKeyDown);

			object[] expected = {msgKeyDown, msgActivate, msgActivate, msgKeyDown, msgKillFocus, msgKillFocus, msgSetFocus, msgSetFocus};
			VerifyArray(expected, tc2);
		}

		/// <summary>
		/// Test reentrant operation
		/// </summary>
		[Test]
		public void KillFocusAndGetFocusInPaint()
		{
			// need to create a dummy form because PaintEventArgs wants a Graphics object
			using (DummyForm dummy = new DummyForm())
			{
				dummy.Create();
				// Similate OnPaint, inside which we get a kill focus and a SetFocus.
				using (System.Drawing.Graphics g = dummy.CreateGraphics())
				{
					PaintEventArgs pe1 = new PaintEventArgs(g, new System.Drawing.Rectangle(0, 1, 2, 3));
					Message msgKillFocus = Message.Create(IntPtr.Zero, (int)Win32.WinMsgs.WM_KILLFOCUS, new IntPtr(201), new IntPtr(2001));
					Message msgSetFocus = Message.Create(IntPtr.Zero, (int)Win32.WinMsgs.WM_SETFOCUS, new IntPtr(301), new IntPtr(3001));
					TestControl3 tc3 = new TestControl3(pe1, msgKillFocus, msgSetFocus);

					tc3.CallOnPaint(pe1);

					object[] expected = { pe1, pe1, msgKillFocus, msgKillFocus, msgSetFocus, msgSetFocus };
					VerifyArray(expected, tc3);
				}
			}
		}

		void Fill(int start, int end, ref int count, SafeQueue queue)
		{
			for (int i = start; i < end; i++)
			{
				queue.Add(i);
				count++;
				Assert.AreEqual(count, queue.Count);
			}
		}

		void Check(int start, int end, ref int count, SafeQueue queue)
		{
			for (int i = start; i < end; i++)
			{
				int result = (int) queue.Remove();
				count--;
				Assert.AreEqual(i, result);
				Assert.AreEqual(count, queue.Count);
			}
		}

		/// <summary>
		/// Queue without any tricks
		/// </summary>
		[Test]
		public void QueueNormal()
		{
			SafeQueue queue = new SafeQueue();
			int count = 0;
			// fill the first 49 of 100 inital slots. Just short of causing grow.
			Fill(0, 49, ref count, queue);

			// Remove the first 40.
			Check(0, 40, ref count, queue);
			Assert.AreEqual(9, count);

			// Add and remove another 40.
			Fill(49, 89, ref count, queue);
			Check(40, 80, ref count, queue);
			Assert.AreEqual(9, count);

			// And another group. This checks the situation where the queue is wrapped around during grow.
			Fill(89, 149, ref count, queue);
			Assert.AreEqual(69, count);

			Check(80, 149, ref count, queue);
			Assert.AreEqual(0, count);

			// Re-establishes a situation for an earlier group of tests.
			Fill(0,50, ref count, queue);
			Check(0, 20, ref count, queue);

			// Add another 55; this will force a grow, we now have 200 slots
			Fill(500, 555, ref count, queue);
			// Remove the next 30. Down to 55.
			Check(20, 50, ref count, queue);

			// Remove 10 more. Down to 45.
			Check(500, 510, ref count, queue);

			// Add another 50
			Fill(600, 650, ref count, queue);

			// Remove the rest of the 500 series, just to check
			Check(510, 555, ref count, queue);
			// Remove the rest of the 600 series, just to check, and make sure we can be back to 0.
			Check(600, 650, ref count, queue);
			Assert.AreEqual(0, queue.Count);
		}

		/// <summary>
		/// Test queueing when distorted by reentrant adds.
		/// </summary>
		[Test]
		public void QueueReentrant()
		{
			SafeQueue queue = new TrickQueue();
			int count = 0;
			// fill the first 50 of 100 inital slots.
			for (int i = 0; i < 49; i++)
			{
				queue.Add(i);
				count++;
				Assert.AreEqual(count, queue.Count);
			}
			queue.Add(300); // causes grow, with extra 10 from reentrant simulation.
			Assert.AreEqual(60, queue.Count);
			count += 11;
			for (int i = 0; i < 49; i++)
			{
				int result = (int) queue.Remove();
				count--;
				Assert.AreEqual(i, result);
				Assert.AreEqual(count, queue.Count);
			}
			Assert.AreEqual(300, (int)queue.Remove());
			count--;
			for (int i = 900; i < 910; i++)
			{
				int result = (int) queue.Remove();
				count--;
				Assert.AreEqual(i, result);
				Assert.AreEqual(count, queue.Count);
			}
		}
	}

	/// <summary>
	/// This adds an extra set of numbers, 900..909, whenever we grow.
	/// </summary>
	public class TrickQueue : SafeQueue
	{
		/// <summary>
		/// Generate some extra Adds during memory allocation.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		protected override object[] GetNewArray(int length)
		{
			for (int i = 900; i < 910; i++)
				this.Add(i);
			return new object[length];
		}
	}

	/// <summary>
	/// Test class that implements WndProc by noting start and finish, and possibly
	/// making recursive calls.
	/// </summary>
	public class TestControlBase : Control
	{
		/// <summary>
		/// public for test purposes
		/// </summary>
		public ArrayList m_messages = new ArrayList();

		/// <summary>
		///
		/// </summary>
		public TestControlBase()
		{
		}
		/// <summary>
		/// Fake WndProc notes order things happen.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			m_messages.Add(m);
			RunPendingMessages(m);
			m_messages.Add(m);
		}

		/// <summary>
		/// Override to simulate recursive calls to WndProc.
		/// </summary>
		/// <param name="original"></param>
		protected virtual void RunPendingMessages(object original)
		{
		}

		/// <summary>
		/// Fake OnPaint notes order things happen.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			m_messages.Add(e);
			RunPendingMessages(e);
			m_messages.Add(e);
		}

		/// <summary>
		/// allow protected method to be called by test code
		/// </summary>
		/// <param name="m"></param>
		public void CallWndProc(ref Message m)
		{
			this.WndProc(ref m);
		}

		/// <summary>
		/// allow protected method to be called by test code
		/// </summary>
		public void CallOnPaint(PaintEventArgs e)
		{
			this.OnPaint(e);
		}

	}

	/// <summary>
	/// Test class that implements sequencing of messages, but simulates no recursion.
	/// This is a good model for what a class needs to do to get messages sequentially.
	/// </summary>
	public class TestControl1 : TestControlBase, IReceiveSequentialMessages
	{
		MessageSequencer m_messageSequencer;

		/// <summary>
		///
		/// </summary>
		public TestControl1()
		{
			m_messageSequencer = new MessageSequencer(this);
		}
		/// <summary>
		/// Required override to support sequencing messages.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			m_messageSequencer.SequenceWndProc(ref m);
		}

		/// <summary>
		/// Required override to support sequencing messages.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			m_messageSequencer.SequenceOnPaint(e);
		}

		#region implementation of IReceiveSequentialMessages

		/// <summary>
		/// saves the message it is passed in m_messages; if there are pending messages,
		/// sends them.
		/// </summary>
		/// <param name="m"></param>
		public void OriginalWndProc(ref Message m)
		{
			base.WndProc(ref m);
		}

		/// <summary>
		/// Minimal implementation is nothing, if you don't override OnPaint.
		/// </summary>
		/// <param name="e"></param>
		public void OriginalOnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		/// <summary>
		/// Retrieve the Message Sequencer.
		/// </summary>
		public MessageSequencer Sequencer
		{
			get { return m_messageSequencer; }
		}

		#endregion
	}

	/// <summary>
	/// This is initialized with windows messages for an activate, a killFocus, and a set focus.
	/// RunPendingMessages fires the killFocus and set focus from inside the activate; the
	/// activate from inside the keyDown
	/// </summary>
	public class TestControl2 : TestControl1
	{
		Message m_keyDown;
		Message m_activate;
		Message m_killFocus;
		Message m_setFocus;

		internal TestControl2(Message keyDown, Message activate, Message killFocus, Message setFocus)
		{
			m_keyDown = keyDown;
			m_activate = activate;
			m_killFocus =killFocus;
			m_setFocus = setFocus;
		}

		/// <summary>
		/// Override to produce the required behavior.
		/// </summary>
		/// <param name="original"></param>
		protected override void RunPendingMessages(object original)
		{
			if (m_keyDown.Equals(original))
			{
				WndProc(ref m_activate);
			}
			else if (m_activate.Equals(original))
			{
				WndProc(ref m_killFocus);
				WndProc(ref m_setFocus);
			}
		}
	}
	/// <summary>
	/// This is initialized with windows messages for a paint event, kill and set focus.
	/// RunPendingMessages fires the killFocus and setfocus from inside the paint.
	/// </summary>
	public class TestControl3 : TestControl1
	{
		PaintEventArgs m_pe;
		Message m_killFocus;
		Message m_setFocus;

		internal TestControl3(PaintEventArgs pe, Message killFocus, Message setFocus)
		{
			m_pe = pe;
			m_killFocus = killFocus;
			m_setFocus = setFocus;
		}

		/// <summary>
		/// Override to produce the required behavior.
		/// </summary>
		/// <param name="original"></param>
		protected override void RunPendingMessages(object original)
		{
			if (original.Equals(m_pe))
			{
				WndProc(ref m_killFocus);
				WndProc(ref m_setFocus);
			}
		}
	}
}
