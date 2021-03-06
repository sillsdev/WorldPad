//#define TESTMS
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
// File: ReferenceViewBase.cs
// Responsibility: Eric Pyle
// Last reviewed:
//
// <remarks>
// For handling things common to ReferenceView classes.
// </remarks>
// --------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Diagnostics;
using System.Xml;

using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.Common.RootSites;
using SIL.FieldWorks.Common.Utils;
using SIL.FieldWorks.Common.COMInterfaces;
using SIL.FieldWorks.FdoUi;
using SIL.FieldWorks.FDO.Validation;

namespace SIL.FieldWorks.Common.Framework.DetailControls
{
	/// <summary>
	/// Base class for handling things common to ReferenceView classes.
	/// </summary>
	public class ReferenceViewBase : RootSiteControl
	{

		protected ICmObject m_rootObj;
		protected int m_rootFlid;
		protected string m_displayNameProperty;

		#region Construction

		public ReferenceViewBase()
		{
		}

		public void Initialize(ICmObject rootObj, int rootFlid, FdoCache cache, string displayNameProperty,
				XCore.Mediator mediator)
		{
			CheckDisposed();
			Debug.Assert(cache != null && m_fdoCache == null);
			m_displayNameProperty = displayNameProperty;
			m_fdoCache = cache;
			m_rootObj = rootObj;
			m_rootFlid = rootFlid;
			Mediator = mediator;
			if (m_rootb == null)
				MakeRoot();
		}

		#endregion // Construction

		protected override bool OnRightMouseUp(Point pt, Rectangle rcSrcRoot,
			Rectangle rcDstRoot)
		{
			IVwSelection sel = RootBox.MakeSelAt(pt.X, pt.Y, rcSrcRoot, rcDstRoot, false);
			TextSelInfo tsi = new TextSelInfo(sel);
			return HandleRightClickOnObject(tsi.Hvo(false));
		}

		protected virtual bool HandleRightClickOnObject(int hvo)
		{
#if TESTMS
Debug.WriteLine("Starting: ReferenceViewBase.HandleRightClickOnObject");
#endif
			if (hvo == 0)
			{
#if TESTMS
Debug.WriteLine("ReferenceViewBase.HandleRightClickOnObject: hvo is 0, so returning.");
#endif
				return false;
			}
			ReferenceBaseUi ui = ReferenceBaseUi.MakeUi(Cache, m_rootObj, m_rootFlid, hvo);
#if TESTMS
Debug.WriteLine("Created ReferenceBaseUi");
Debug.WriteLine("hvo=" + hvo.ToString()+" "+ui.Object.ShortName+"  "+ ui.Object.ToString());
#endif
			if (ui != null)
			{
#if TESTMS
Debug.WriteLine("ui.HandleRightClick: and returning true.");
#endif
				return ui.HandleRightClick(Mediator, this, true);
			}
#if TESTMS
Debug.WriteLine("No ui: returning false");
#endif
			return false;
		}
	}
}
