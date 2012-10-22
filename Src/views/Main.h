/*--------------------------------------------------------------------*//*:Ignore this sentence.
Copyright (C) 1999, 2001 SIL International. All rights reserved.

Distributable under the terms of either the Common Public License or the
GNU Lesser General Public License, as specified in the LICENSING.txt file.

File: Main.h
Responsibility: John Thomson
Last reviewed: Not yet.

Description:
	Main header file for the views component.
-------------------------------------------------------------------------------*//*:End Ignore*/
#pragma once
#ifndef VIEWS_H
#define VIEWS_H 1

#define NO_EXCEPTIONS 1
#include "Common.h"
#include <shlobj.h> // for one call to SHGetSpecialFolderPath

/* ---------------------
If you want to show colored boxes around the boxes uncomment the following define:
----------------------*/
//#define _DEBUG_SHOW_BOX
/*----------------------*/
#define kdzptInch 72
#define kdzmpInch 72000

#define kchwHardLineBreak (wchar)0x2028

#include "VwResources.h"
#include "..\..\..\Src\AppCore\Res\AfAppRes.h"

// This is needed now by FwGr.h which uses std::wstring
#include <string>

using namespace fwutil;

//:>**********************************************************************************
//:>	Forward declarations
//:>**********************************************************************************
class VwBox;
class VwGroupBox;
class VwRootBox;
class VwEnv;
class VwStringBox;
class VwStringBoxMain;
class VwBrokenStringBox;
class VwParagraphBox;
class VwRootBox;
class VwTableBox;
class VwAbstractNotifier;
class VwNotifier;
class VwSelection;
class VwTextSelection;
class VwTableRowBox;
class VwTableCellBox;
class SilDataAccess;
class VwAccessRoot;
class VwSynchronizer;
DEFINE_COM_PTR(VwSynchronizer); // deals with circularity of root box and synch.

//:>**********************************************************************************
//:>	Classes we have to include before we can do typedefs
//:>**********************************************************************************
#include "HashMap.h"
#include "Vector.h"

//:>**********************************************************************************
//:>	Other classes we have to include.
//:>**********************************************************************************
#include "OleDb.h"

//:>**********************************************************************************
//:>	Interfaces.
//:>**********************************************************************************
#include "FwKernelTlb.h"
//#include "DbAccessTlb.h"
//#include "LanguageTlb.h" // for justification

// these defines are in ViewTlb.idl, but for some reason not copied to ViewTlb.h
#define HVO long // Hungarian hvo (Handle to Viewable Object)
#define PropTag int
#include "ViewsTlb.h"

//:>**********************************************************************************
//:>	Types and constants used in View subsystem
//:>**********************************************************************************

// BuildRecs are used to construct a description of the part of a display that needs
// to be rebuilt when an underlying value changes.
// A record corresponds to two layers of the display hierarchy: one object, and one
// attribute of that object.
// By tracing up throught the display being rebuilt, we determine that we need to
// rebuild part of the display of a particular occurrence of a particular object
// in its parent attribute, and that we need to rebuild a particular attribute of
// that object (or part of that attribute).
struct BuildRec // Hungarian: bldrec
{
	HVO hvo;	// object we want to rebuild part of display for
	int ihvo;			// expected index of that object in its attribute to
						// distinguish multiple occurrences
	int tag;		// of attr we want from that object
	int cprop;		// last record: number of times to rebuild that prop
						// others: number of occurrences of that prop to ignore
						// before going ahead (for repeats in same object)
};
// A BuildVec is a list, ordered from the top level object in the display down to
// the one whose attr changed, completely describing what needs rebuilding.
typedef Vector<BuildRec> BuildVec; // Hungarian: bldvec


typedef HashMap<VwBox *, Rect> FixupMap; // Hungarian fixmap

typedef Vector<VwBox *> BoxVec; // Hungarian vbox

typedef Vector<VwGroupBox *> GroupBoxVec; // Hungarian vgbox

typedef Vector<int> IntVec; // Hungarian intvec, or replace int with what it signifies
typedef Vector<HVO> HvoVec; // Hungarian vhvo
typedef Vector<long> LongVec; // Hungarian vlo

typedef ComVector<VwAbstractNotifier> NotifierVec; // vpanote

typedef Set<VwBox *> BoxSet;  //Hungarian boxset

typedef ComMultiMap<VwBox *, VwAbstractNotifier> NotifierMap; // Hungarian mmboxqnote

typedef ComVector<ITsTextProps> TtpVec; // Hungarian vqttp
typedef ComVector<IVwPropertyStore> VwPropsVec; // Hungarian vqvps

typedef ComMultiMap<HVO, VwAbstractNotifier> ObjNoteMap; // Hungarian mmhvoqnote

#include "UtilView.h"
// Needed only so VwCacheDa can reference kflids
// that relate to structured text, and VwOleDbDa can use kwsAnal.
#include "FwCellarTlb.h"

// The Enchant spelling checker interfaces.
#include "enchant++.h"


//:>**********************************************************************************
//:>	Implementations.
//:>**********************************************************************************
// for interfacing with Graphite:
namespace gr {
typedef unsigned char utf8;
typedef wchar_t utf16;
typedef unsigned long int utf32;
#define UtfType LgUtfForm
class GrEngine;
} // gr

// obsolete #include "ActualTextProperties.h"
#include "VwBaseDataAccess.h"
#include "VwBaseVirtualHandler.h"
#include "VwCacheDa.h"
#include "VwOverlay.h"
#include "GrResult.h"
/////#include "IGrGraphics.h"
#include "ITextSource.h"
#include "IGrJustifier.h"
/////#include "GrGraphics.h"
#include "GrTxtSrc.h"
#include "GrJustifier.h"
#include "GraphiteProcess.h"
#include "FwGraphiteProcess.h"
#include "FwGr.h"
#include "VwGraphics.h"
#include "VwJustifier.h"
#include "VwPropertyStore.h"
#include "VwTxtSrc.h"
#include "VwPrintContext.h"
#include "VwSimpleBoxes.h"
#include "VwNotifier.h"
#include "VwTextBoxes.h"
#include "VwSelection.h"
#include "VwRootBox.h"
#include "VwEnv.h"
#include "VwTableBox.h"
#include "VwLazyBox.h"
#include "AfDef.h"
#include "AfColorTable.h"
#include "AfGfx.h"
#include "VwPattern.h"
#include "WriteXml.h"
#include "SqlUndoAction.h"
#include "DbColSpec.h"
#include "VwOleDbDa.h"
#include "FwStyledText.h"
#include "VwAccessRoot.h"
#include "VwSynchronizer.h"
#include "VwTextStore.h"
#include "VwLayoutStream.h"
#include "VwUndo.h"
#include "DelObjUndoAction.h"
#include "VwInvertedViews.h"


// #include "SilDataAccess.h"
//#include "VwWindow.h" // not part of the DLL

#endif //!VIEWS_H