/*--------------------------------------------------------------------*//*:Ignore this sentence.
Copyright (C) 2003 SIL International. All rights reserved.

Distributable under the terms of either the Common Public License or the
GNU Lesser General Public License, as specified in the LICENSING.txt file.

File: testViews.cpp
Responsibility:
Last reviewed:

	Global initialization/cleanup for unit testing the Views DLL classes.
-------------------------------------------------------------------------------*//*:End Ignore*/
#include "testViews.h"

namespace unitpp
{
	void GlobalSetup(bool verbose)
	{
		ModuleEntry::DllMain(0, DLL_PROCESS_ATTACH);
		::OleInitialize(NULL);
		StrUtil::InitIcuDataDir();

	}
	void GlobalTeardown()
	{
		::OleUninitialize();
		ModuleEntry::DllMain(0, DLL_PROCESS_DETACH);
	}
}

namespace TestViews
{
	ILgWritingSystemFactoryPtr g_qwsf;
	int g_wsEng = 0;
	int g_wsFrn = 0;
	int g_wsGer = 0;

	// Create a dummy writing system factory with English and French.
	void CreateTestWritingSystemFactory()
	{
		StrUni stuWs;
		IWritingSystemPtr qws;
		g_qwsf.CreateInstance(CLSID_LgWritingSystemFactory);
		// Don't install these dummy languages.
		g_qwsf->put_BypassInstall(TRUE);
		// Add a writing system for English.
		stuWs.Assign(L"en");
		CheckHr(g_qwsf->get_Engine(stuWs.Bstr(), &qws));
		StrUni stuTimesNewRoman(L"Times New Roman");
		qws->put_DefaultSerif(stuTimesNewRoman.Bstr());
		qws->put_DefaultBodyFont(stuTimesNewRoman.Bstr());
		CheckHr(qws->get_WritingSystem(&g_wsEng));

		// Add a writing system for French.
		stuWs.Assign(L"fr");
		CheckHr(g_qwsf->get_Engine(stuWs.Bstr(), &qws));
		qws->put_DefaultSerif(stuTimesNewRoman.Bstr());
		qws->put_DefaultBodyFont(stuTimesNewRoman.Bstr());
		CheckHr(qws->get_WritingSystem(&g_wsFrn));

		// Add a writing system for German.
		stuWs.Assign(L"de");
		CheckHr(g_qwsf->get_Engine(stuWs.Bstr(), &qws));
		qws->put_DefaultSerif(stuTimesNewRoman.Bstr());
		qws->put_DefaultBodyFont(stuTimesNewRoman.Bstr());
		CheckHr(qws->get_WritingSystem(&g_wsGer));
	}

	// Free the dummy writing system factory.
	void CloseTestWritingSystemFactory()
	{
		if (g_qwsf)
		{
			g_qwsf->Shutdown();
			g_qwsf.Clear();
		}
	}
}

// Local Variables:
// mode:C++
// compile-command:"cmd.exe /e:4096 /c c:\\FW\\Bin\\mkvw-tst.bat DONTRUN"
// End: (These 4 lines are useful to Steve McConnel.)
