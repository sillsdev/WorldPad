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
// File: DataZipTest.cs
// Responsibility: TE Team
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace SIL.FieldWorks.Common.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Test the <see cref="DataZip"/> class.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[TestFixture]
	public class DataZipTest
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="DataZipTest"/> class.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		public DataZipTest()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test <see cref="DataZip.PackData"/> method with null input.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TestPackNull()
		{
			Assert.IsNull(DataZip.PackData(null));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test <see cref="DataZip.UnpackData"/> method with null input.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TestUnpackNull()
		{
			Assert.IsNull(DataZip.UnpackData(null));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Test <see cref="DataZip.PackData"/> and <see cref="DataZip.UnpackData"/> methods.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[Test]
		public void TestPackAndUnpack()
		{
			byte[] orig = new byte[] {122,34,67,89,34,10,10,67,56,122,122,10,0,34,56,12};
			byte[] zipped = DataZip.PackData(orig);
			Assert.IsTrue(orig != zipped);
			byte[] unzipped = DataZip.UnpackData(zipped);
			Assert.AreEqual(orig.Length, unzipped.Length);
			for(int i = 0; i < orig.Length; i++)
			{
				Assert.AreEqual(orig[i], unzipped[i]);
			}
		}
	}
}