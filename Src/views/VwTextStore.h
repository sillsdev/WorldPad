/*--------------------------------------------------------------------*//*:Ignore this sentence.
Copyright (C) 2003 SIL International. All rights reserved.

Distributable under the terms of either the Common Public License or the
GNU Lesser General Public License, as specified in the LICENSING.txt file.

File: VwTextStore.h
Responsibility:
Last reviewed: Not yet.

Description:
	Defines the class VwTextStore which implements the MS Text Services Framework interface
	ITextSourceACP.
-------------------------------------------------------------------------------*//*:End Ignore*/
#pragma once
#ifndef VwTextStore_INCLUDED
#define VwTextStore_INCLUDED

DEFINE_COM_PTR(ITextStoreACP);
DEFINE_COM_PTR(ITextStoreACPSink);
DEFINE_COM_PTR(ITextStoreACPServices);
DEFINE_COM_PTR(ITfThreadMgr);
DEFINE_COM_PTR(ITfDocumentMgr);
DEFINE_COM_PTR(ITfContext);
DEFINE_COM_PTR(ITfCategoryMgr);
DEFINE_COM_PTR(ITfDisplayAttributeMgr);
DEFINE_COM_PTR(ITfProperty);
DEFINE_COM_PTR(ITfReadOnlyProperty);
DEFINE_COM_PTR(IEnumTfRanges);
DEFINE_COM_PTR(ITfRange);
DEFINE_COM_PTR(ITfRangeACP);
DEFINE_COM_PTR(ITfDisplayAttributeInfo);
DEFINE_COM_PTR(ITfMouseSink);

/*----------------------------------------------------------------------------------------------
	Class: VwTextStore
	Description: implement the ITextStoreACP interface as a wrapper around a VwRootBox.
	Hungarian: txs
----------------------------------------------------------------------------------------------*/
class VwTextStore : public ITextStoreACP, public ITfContextOwnerCompositionSink,
	public ITfMouseTrackerACP
{
public:
	VwTextStore(VwRootBox * prootb);
	~VwTextStore();

	// IUnknown methods.
	STDMETHOD(QueryInterface)(REFIID, LPVOID*);
	STDMETHOD_(ULONG, AddRef)(void)
	{
		return InterlockedIncrement(&m_cref);
	}
	STDMETHOD_(ULONG, Release)(void)
	{
		long cref = InterlockedDecrement(&m_cref);
		if (cref == 0)
		{
			m_cref = 1;
			delete this;
		}
		return cref;
	}

	// ITextStoreACP methods.
	STDMETHOD(AdviseSink)(REFIID riid, IUnknown * punk, DWORD dwMask);
	STDMETHOD(UnadviseSink)(IUnknown * punk);
	STDMETHOD(RequestLock)(DWORD dwLockFlags, HRESULT * phrSession);
	STDMETHOD(GetStatus)(TS_STATUS * pdcs);
	STDMETHOD(QueryInsert)(LONG acpTestStart, LONG acpTestEnd, ULONG cch,
		LONG * pacpResultStart, LONG * pacpResultEnd);
	STDMETHOD(GetSelection)(ULONG ulIndex, ULONG ulCount, TS_SELECTION_ACP * pSelection,
		ULONG * pcFetched);
	STDMETHOD(SetSelection)(ULONG ulCount, const TS_SELECTION_ACP * pSelection);
	STDMETHOD(GetText)(LONG acpStart, LONG acpEnd, WCHAR * pchPlain, ULONG cchPlainReq,
		ULONG * pcchPlainOut, TS_RUNINFO * prgRunInfo, ULONG ulRunInfoReq,
		ULONG * pulRunInfoOut, LONG * pacpNext);
	STDMETHOD(SetText)(DWORD dwFlags, LONG acpStart, LONG acpEnd, const WCHAR * pchText,
		ULONG cch, TS_TEXTCHANGE * pChange);
	STDMETHOD(GetFormattedText)(LONG acpStart, LONG acpEnd, IDataObject ** ppDataObject);
	STDMETHOD(GetEmbedded)(LONG acpPos, REFGUID rguidService, REFIID riid, IUnknown ** ppunk);
	STDMETHOD(QueryInsertEmbedded)(const GUID * pguidService, const FORMATETC * pFormatEtc,
		BOOL * pfInsertable);
	STDMETHOD(InsertEmbedded)(DWORD dwFlags, LONG acpStart, LONG acpEnd,
		IDataObject * pDataObject, TS_TEXTCHANGE * pChange);
	STDMETHOD(RequestSupportedAttrs)(DWORD dwFlags, ULONG cFilterAttrs,
		const TS_ATTRID * paFilterAttrs);
	STDMETHOD(RequestAttrsAtPosition)(LONG acpPos, ULONG cFilterAttrs,
		const TS_ATTRID * paFilterAttrs, DWORD dwFlags);
	STDMETHOD(RequestAttrsTransitioningAtPosition)(LONG acpPos, ULONG cFilterAttrs,
		const TS_ATTRID * paFilterAttrs, DWORD dwFlags);
	STDMETHOD(FindNextAttrTransition)(LONG acpStart, LONG acpHalt, ULONG cFilterAttrs,
		const TS_ATTRID * paFilterAttrs, DWORD dwFlags, LONG * pacpNext, BOOL * pfFound,
		LONG * plFoundOffset);
	STDMETHOD(RetrieveRequestedAttrs)(ULONG ulCount, TS_ATTRVAL * paAttrVals,
		ULONG * pcFetched);
	STDMETHOD(GetEndACP)(LONG * pacp);
	STDMETHOD(GetActiveView)(TsViewCookie * pvcView);
	STDMETHOD(GetACPFromPoint)(TsViewCookie vcView, const POINT * pt, DWORD dwFlags,
		LONG * pacp);
	STDMETHOD(GetTextExt)(TsViewCookie vcView, LONG acpStart, LONG acpEnd, RECT * prc,
		BOOL * pfClipped);
	STDMETHOD(GetScreenExt)(TsViewCookie vcView, RECT * prc);
	STDMETHOD(GetWnd)(TsViewCookie vcView, HWND * phwnd);
	STDMETHOD(InsertTextAtSelection)(DWORD dwFlags, const WCHAR * pchText, ULONG cch,
		LONG * pacpStart, LONG * pacpEnd, TS_TEXTCHANGE * pChange);
	STDMETHOD(InsertEmbeddedAtSelection)(DWORD dwFlags, IDataObject * pDataObject,
		LONG * pacpStart, LONG * pacpEnd, TS_TEXTCHANGE * pChange);

	//ITfContextOwnerCompositionSink methods
	STDMETHOD(OnStartComposition)(ITfCompositionView *pComposition, BOOL *pfOk);
	STDMETHOD(OnUpdateComposition)(ITfCompositionView *pComposition, ITfRange *pRangeNew);
	STDMETHOD(OnEndComposition)(ITfCompositionView *pComposition);

	// ITfMouseTrackerACP
	STDMETHOD(AdviseMouseSink)(ITfRangeACP * range, ITfMouseSink* pSink,
		DWORD* pdwCookie);
	STDMETHOD(UnadviseMouseSink)(DWORD dwCookie);

	// Other Public Methods.

	void OnDocChange();
	void OnSelChange(int nHow);
	void OnLayoutChange();
	void SetFocus();
	void Init();
	void Close();
	void AddToKeepList(LazinessIncreaser *pli);
	bool MouseEvent(int xd, int yd, RECT rcSrc1, RECT rcDst1, VwMouseEvent me);

protected:
	// Member variables
	long m_cref;
	// We keep references to compositions as a desperate attempt to make them work.
	ComVector<ITfCompositionView> m_compositions;

	struct AdviseSinkInfo
	{
		IUnknownPtr  m_qunkID;
		ITextStoreACPSinkPtr m_qTextStoreACPSink;
		DWORD m_dwMask;
	} m_AdviseSinkInfo;
	ITextStoreACPServicesPtr m_qServices;

	// Set this to false to suppress all notifications, typically while we do something
	// through the interface that would produce a notification if done any other way.
	bool m_fNotify;
	bool m_fLocked;
	DWORD m_dwLockType;
	bool m_fPendingLockUpgrade;
	bool m_fLayoutChanged;	// Set when TSF changes layout but can't notify until lock released.
	bool m_fInterimChar;	// Used in intermediate states of far-east IMEs.

	VwRootBoxPtr m_qrootb;

	// if non-null, receives mouse notifications, if mouse over range of characters specified
	// by following variables.
	ITfMouseSinkPtr m_qMouseSink;
	VwParagraphBox * m_pvpboxMouseSink;
	int m_ichMinMouseSink; // in NFD form
	int m_ichLimMouseSink; // in NFD form
	// This keeps track of the length of the paragraph we last worked on.
	// It is used to give some sort of reasonable DocChanged notification
	// when the selection is destroyed.
	int m_cchLastPara; // in NFD form
	// This is set true if a property is updated without normalization (and therefore not
	// in the database) in order to avoid messing up compositions.
	bool m_fCommitDuringComposition;
	bool m_fDoingRecommit;

	static const int kNFDBufferSize = 64;
	IWritingSystemPtr m_qws;
	int AcpToLog(int acpReq);
	int LogToAcp(int ichReq);
	bool IsNfdIMEActive();
	void GetCurrentWritingSystem();

public:
	VwParagraphBox * m_pvpboxCurrent;

	static ITfThreadMgrPtr s_qttmThreadMgr;
	static TfClientId s_tfClientID;
	static ITfCategoryMgrPtr s_qtfCategoryMgr;
	static ITfDisplayAttributeMgrPtr s_qtfDisplayAttributeMgr;
	ITfDocumentMgrPtr m_qtdmDocMgr;
	ITfContextPtr m_qtcContext;
	TfEditCookie m_tfEditCookie;

	// Internal methods.

	bool _LockDocument(DWORD dwLockFlags)
	{
		if (m_fLocked)
			return false;
		m_fLocked = true;
		m_dwLockType = dwLockFlags;
		return true;
	}

	void _UnlockDocument()
	{
		m_fLocked = false;
		m_dwLockType = 0;

		// if there is a pending lock upgrade, grant it
		if (m_fPendingLockUpgrade)
		{
			m_fPendingLockUpgrade = false;
			HRESULT hr;
			RequestLock(TS_LF_READWRITE, &hr);
		}

		// if any layout changes occurred during the lock, notify the manager
		if (m_fLayoutChanged)
		{
			m_fLayoutChanged = false;
			TsViewCookie tvc;
			GetActiveView(&tvc);
			m_AdviseSinkInfo.m_qTextStoreACPSink->OnLayoutChange(TS_LC_CHANGE, tvc);
		}
	}

	VwTextSelection * GetStartAndEndBoxes(VwParagraphBox ** ppvpboxStart,
		VwParagraphBox ** ppvpboxEnd, bool * pfEndBeforeAnchor = NULL);
	int TextLength();
	int ComputeBoxAndOffset(int acpNfd, VwParagraphBox * pvpboxFirst, VwParagraphBox * pvpboxLast,
		VwParagraphBox ** ppvpboxOut);
	void CreateNewSelection(int ichFirst, int ichLast, bool fEndBeforeAnchor,
		VwTextSelection ** pptsel);
	void ClearPointersTo(VwParagraphBox * pvpbox);
	void DoDisplayAttrs();
	void TerminateAllCompositions(void);
	// Conditionally terminate all compositions and return false.  Used mostly in MouseEvent().
	bool EndAllCompositions(bool fStop)
	{
		if (fStop)
			TerminateAllCompositions();
		return false;
	}
	void OnLoseFocus();
	DWORD SuspendAdvise(IUnknown ** ppunk);
	bool IsCompositionActive()
	{
		return m_compositions.Size() > 0;
	}

	void NoteCommitDuringComposition()
	{
		m_fCommitDuringComposition = true;
	}

	bool IsDoingRecommit()
	{
		return m_fDoingRecommit;
	}

private:
	int RetrieveText(int ichFirst, int ichLast, int cchPlainReqNfd, wchar* pchPlainNfd);
	void NormalizeText(StrUni & stuText, WCHAR* pchPlain, ULONG cchPlainReq,
		ULONG * pcchPlainOut, TS_RUNINFO * prgRunInfo, ULONG ulRunInfoReq,
		ULONG * pulRunInfoOut);
	int SetOrAppendRunInfo(TS_RUNINFO * prgRunInfo, ULONG ulRunInfoReq, int iRunInfo,
		TsRunType runType, int length);
};

DEFINE_COM_PTR(VwTextStore);

#endif  //VwTextStore_INCLUDED