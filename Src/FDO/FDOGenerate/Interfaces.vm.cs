## --------------------------------------------------------------------------------------------
## Copyright (C) 2006 SIL International. All rights reserved.
##
## Distributable under the terms of either the Common Public License or the
## GNU Lesser General Public License, as specified in the LICENSING.txt file.
##
## NVelocity template file
## This file is used by the FdoGenerate task to generate the source code from the XMI
## database model.
## --------------------------------------------------------------------------------------------
//This is automatically generated by FDOGenerate task.  ****Do not edit****
using System;
using SIL.FieldWorks;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.Common.COMInterfaces;

#foreach($module in $fdogenerate.Modules)
[assembly : CellarModule("$module.Name", "$module.Assembly")]
#end

namespace SIL.FieldWorks.FDO	// All generated class interfaces are in here.
{
	#region Interface for CmObject
	/// <summary>
	/// Interface for a CmObject.
	/// </summary>
	public partial interface ICmObject
	{
		/// <summary>
		/// Project-specific ID of the object.
		/// </summary>
		int Hvo
		{
			get;
		}

		/// <summary>
		/// Project-specific ID of the object's owner.
		/// </summary>
		/// <remarks>
		/// It may be zero, if the obejct is not owned (generally, not done).
		/// </remarks>
		int OwnerHVO
		{
			get;
		}

		/// <summary>
		/// Field ID of the owning object where the object is stored.
		/// </summary>
		int OwningFlid
		{
			get;
		}

		/// <summary>
		/// Owning ord of the owning object where the object is stored.
		/// </summary>
		int OwnOrd
		{
			get;
		}

		/// <summary>
		/// Class ID of the object.
		/// </summary>
		int ClassID
		{
			get;
		}

		/// <summary>
		/// Unique ID of the object.
		/// </summary>
		Guid Guid
		{
			get;
			set;
		}

		/// <summary>
		/// Get the UpdDttm value of the CmObject
		/// </summary>
		DateTime UpdTime
		{
			get;
		}
	}
	#endregion Interface for CmObject

#foreach($module in $fdogenerate.Modules)
	#foreach($class in $module.Classes)
		#if($class.Name != "CmObject")
			#parse("classInterface.vm.cs")
		#end
	#end
#end

}