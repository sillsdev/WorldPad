/*-----------------------------------------------------------------------*//*:Ignore in Surveyor
Copyright (C) 1999, 2003 SIL International. All rights reserved.

Distributable under the terms of either the Common Public License or the
GNU Lesser General Public License, as specified in the LICENSING.txt file.

File: DebugReport.cpp
Responsibility: TE Team

	Implementation of the DebugReport class.
-------------------------------------------------------------------------------*//*:End Ignore*/
#include "Main.h"
#include "DebugReport.h"
#pragma hdrstop

#undef THIS_FILE
DEFINE_THIS_FILE

#ifdef DEBUG

//:>********************************************************************************************
//:>	DebugReport methods.
//:>********************************************************************************************

IDebugReportSinkPtr DebugReport::s_qReportSink = NULL;

/*----------------------------------------------------------------------------------------------
	C'tor
----------------------------------------------------------------------------------------------*/
DebugReport::DebugReport()
{
	m_cref = 1;
	::DebugProcsInit();
	m_oldHook = ::DbgSetReportHook(DebugReport::ReportHandler);
	ModuleEntry::ModuleAddRef();
}

/*----------------------------------------------------------------------------------------------
	D'tor
----------------------------------------------------------------------------------------------*/
DebugReport::~DebugReport()
{
	::DbgSetReportHook(m_oldHook);
	::DebugProcsExit();
	ModuleEntry::ModuleRelease();
}


//:>********************************************************************************************
//:>	DebugReport - Generic factory stuff to allow creating an instance w/ CoCreateInstance.
//:>********************************************************************************************

static GenericFactory g_factDbr(
	_T("SIL.Kernel.DebugReport"),
	&CLSID_DebugReport,
	_T("SIL Debug Report Handler"),
	_T("Apartment"),
	&DebugReport::CreateCom);


void DebugReport::CreateCom(IUnknown *punkCtl, REFIID riid, void ** ppv)
{
	AssertPtr(ppv);
	Assert(!*ppv);
	if (punkCtl)
	{
		ThrowHr(WarnHr(CLASS_E_NOAGGREGATION));
	}
	ComSmartPtr<DebugReport> qdbr;
	// Ref count initially 1
	qdbr.Attach(NewObj DebugReport());
	CheckHr(qdbr->QueryInterface(riid, ppv));
}


//:>********************************************************************************************
//:>	DebugReport - IUnknown Methods
//:>********************************************************************************************

STDMETHODIMP DebugReport::QueryInterface(REFIID iid, void ** ppv)
{
	AssertPtr(ppv);
	if (!ppv)
		return WarnHr(E_POINTER);
	*ppv = NULL;

	if (iid == IID_IUnknown)
		*ppv = static_cast<IUnknown *>(static_cast<IDebugReport *>(this));
	else if (iid == IID_IDebugReport)
		*ppv = static_cast<IDebugReport *>(this);
	else
		return E_NOINTERFACE;

	reinterpret_cast<IUnknown *>(*ppv)->AddRef();
	return S_OK;
}

STDMETHODIMP_(ULONG) DebugReport::AddRef(void)
{
	Assert(m_cref > 0);
	return ++m_cref;
}

STDMETHODIMP_(ULONG) DebugReport::Release(void)
{
	Assert(m_cref > 0);
	if (--m_cref > 0)
		return m_cref;

	m_cref = 1;
	delete this;
	return 0;
}

//:>********************************************************************************************
//:>	DebugReport - IDebugReport Methods
//:>********************************************************************************************

/*----------------------------------------------------------------------------------------------
	${IDebugReport#ShowAssertMessageBox}
----------------------------------------------------------------------------------------------*/
STDMETHODIMP DebugReport::ShowAssertMessageBox(ComBool fShowMessageBox)
{
	BEGIN_COM_METHOD;

	::ShowAssertMessageBox(fShowMessageBox);

	END_COM_METHOD(g_factDbr, IID_IDebugReport);
}

/*----------------------------------------------------------------------------------------------
	${IDebugReport#SetSink}
----------------------------------------------------------------------------------------------*/
STDMETHODIMP DebugReport::SetSink(IDebugReportSink * pSink)
{
	BEGIN_COM_METHOD;
	ChkComArgPtr(pSink);

	s_qReportSink = pSink;

	END_COM_METHOD(g_factDbr, IID_IDebugReport);
}

/*----------------------------------------------------------------------------------------------
	${IDebugReport#ClearSink}
----------------------------------------------------------------------------------------------*/
STDMETHODIMP DebugReport::ClearSink()
{
	BEGIN_COM_METHOD;

	s_qReportSink = NULL;

	END_COM_METHOD(g_factDbr, IID_IDebugReport);
}

/*----------------------------------------------------------------------------------------------
	Callback for DebugProcs
----------------------------------------------------------------------------------------------*/
void __stdcall DebugReport::ReportHandler(int reportType, char * szMsg)
{
	if (DebugReport::s_qReportSink)
	{
		StrAnsi sta = szMsg;
		SmartBstr bstr;
		sta.GetBstr(&bstr);
		DebugReport::s_qReportSink->Report((CrtReportType)reportType, bstr);
	}
}


#endif