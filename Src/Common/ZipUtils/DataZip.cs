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
// File: DataZip.cs
// Responsibility:
// Last reviewed:
//
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace SIL.FieldWorks.Common.Utils
{
	/// <summary>
	/// Summary description for DataZip.
	/// </summary>
	public class DataZip
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// The <see cref="DataZip"/> class doesn't need to be constructed, dude!
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private DataZip()
		{
		}

		#region Static methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Unzip a byte array and return unpacked data
		/// </summary>
		/// <param name="data">Byte array containing data to be unzipped</param>
		/// <returns>unpacked data</returns>
		/// ------------------------------------------------------------------------------------
		public static byte[] UnpackData(byte[] data)
		{
			if (data == null)
				return null;
			try
			{
				MemoryStream memStream = new MemoryStream(data);
				ZipInputStream zipStream = new ZipInputStream(memStream);
				zipStream.GetNextEntry();
				MemoryStream streamWriter = new MemoryStream();
				while (true)
				{
					{
					else
					{
				streamWriter.Close();
				memStream.Close();
				return streamWriter.ToArray();
			}
			catch(Exception e)
			{
				System.Console.Error.WriteLine("Got exception: {0} while unpacking data.",
					e.Message);
				throw;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Zip a byte array and return packed data
		/// </summary>
		/// <param name="data">Byte array containing data to be zipped</param>
		/// <returns>packed data</returns>
		/// ------------------------------------------------------------------------------------
		public static byte[] PackData(byte[] data)
		{
			if (data == null)
				return null;
			try
			{
				MemoryStream memStream = new MemoryStream();
				ZipOutputStream zipStream = new ZipOutputStream(memStream);
				zipStream.PutNextEntry(new ZipEntry("packeddata"));
				MemoryStream streamReader = new MemoryStream(data);
				while (true)
				{
					{
					else
					{
				streamReader.Close();
				memStream.Close();
				return memStream.ToArray();
			}
			catch(Exception e)
			{
				System.Console.Error.WriteLine("Got exception: {0} while packing data.",
					e.Message);
				throw;
			}
		}
		#endregion

	}
}