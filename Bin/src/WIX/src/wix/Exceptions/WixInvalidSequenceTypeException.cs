//-------------------------------------------------------------------------------------------------
// <copyright file="WixInvalidSequenceTypeException.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
//
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//
//    You must not remove this notice, or any other, from this software.
// </copyright>
//
// <summary>
// WiX invalid sequence type exception.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
	using System;

	/// <summary>
	/// WiX invalid sequence type exception.
	/// </summary>
	public class WixInvalidSequenceTypeException : WixException
	{
		private string sequenceTypeName;

		/// <summary>
		/// Instantiate a new WixInvalidSequenceTypeException.
		/// </summary>
		/// <param name="sourceLineNumbers">Source line information of the exception.</param>
		/// <param name="sequenceTypeName">Name of the invalid sequence type.</param>
		public WixInvalidSequenceTypeException(SourceLineNumberCollection sourceLineNumbers, string sequenceTypeName) :
			base(sourceLineNumbers, WixExceptionType.InvalidSequenceType)
		{
			this.sequenceTypeName = sequenceTypeName;
		}

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <value>The error message that explains the reason for the exception, or an empty string("").</value>
		public override string Message
		{
			get { return String.Concat("Unknown sequence type: ", this.sequenceTypeName); }
		}
	}
}