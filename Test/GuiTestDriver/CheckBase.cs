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
// File: CheckBase.cs
// Responsibility: LastufkaM
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;

namespace GuiTestDriver
{
	/// <summary>
	/// Summary description for CheckBase.
	/// </summary>
	public abstract class CheckBase : Instruction
	{
		protected Message m_message;
		protected bool    m_Result;
		protected string  m_onPass    = null;
		protected string  m_onFail    = null;

		public CheckBase(): base()
		{
			m_message = null;
			m_tag     = "check";
			m_Result  = false;
		}

		public Message Message
		{
			get {return m_message;}
			set {m_message = value;}
		}

		public bool Result
		{
			get {return m_Result;}
			set {m_Result = value;}
		}

		public string OnFail
		{
			get {return m_onFail;}
			set {m_onFail = value;}
		}

		public string OnPass
		{
			get {return m_onPass;}
			set {m_onPass = value;}
		}

		/// <summary>
		/// Gets the image of this instruction's data.
		/// </summary>
		/// <param name="name">Name of the data to retrieve.</param>
		/// <returns>Returns the value of the specified data item.</returns>
		public override string GetDataImage (string name)
		{
			if (name == null) name = "result";
			switch (name)
			{
				case "result":	return m_Result.ToString();
				case "on-fail":	return m_onFail;
				case "on-pass":	return m_onPass;
				default:		return base.GetDataImage(name);
			}
		}

		/// <summary>
		/// Echos an image of the instruction with its attributes
		/// and possibly more for diagnostic purposes.
		/// Over-riding methods should pre-pend this base result to their own.
		/// </summary>
		/// <returns>An image of this instruction.</returns>
		public override string image()
		{
			string image = base.image();
			if (m_onFail != null) image += @" on-fail="""+m_onFail+@"""";
			if (m_onPass != null) image += @" on-pass="""+m_onPass+@"""";
			return image;
		}

		/// <summary>
		/// Returns attributes showing results of the instruction for the Logger.
		/// </summary>
		/// <returns>Result attributes.</returns>
		public override string resultImage()
		{
			string image = base.resultImage();
			image += @" result="""+m_Result+@"""";
			if (m_message != null) image += @" message="""+m_message.Read()+@"""";
			return image;
		}
	}
}