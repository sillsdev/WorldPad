REGEDIT4

[HKEY_CLASSES_ROOT\.wixproj]
@="VisualStudio.wixprojfile.%VS_VERSION%"
"Content Type"="text/plain"
[HKEY_CLASSES_ROOT\.wixproj\OpenWithList\devenv.exe]
[HKEY_CLASSES_ROOT\.wixproj\OpenWithProgids]
"VisualStudio.wixprojfile.%VS_VERSION%"=""
[HKEY_CLASSES_ROOT\VisualStudio.wixprojfile.%VS_VERSION%]
@="WiX Project File"
[HKEY_CLASSES_ROOT\VisualStudio.wixprojfile.%VS_VERSION%\DefaultIcon]
@="%DLLPATH%,0"
[HKEY_CLASSES_ROOT\VisualStudio.wixprojfile.%VS_VERSION%\shell\Open]
@="&Open in Visual Studio %VS_VERSION_YEAR%"
[HKEY_CLASSES_ROOT\VisualStudio.wixprojfile.%VS_VERSION%\shell\Open\command]
@="\"%DEVENVPATH%\" \"%1\""

[HKEY_CLASSES_ROOT\.wxs]
@="VisualStudio.wxsfile.%VS_VERSION%"
"Content Type"="text/xml"
"PerceivedType"="text"
[HKEY_CLASSES_ROOT\.wxs\OpenWithList\devenv.exe]
[HKEY_CLASSES_ROOT\.wxs\OpenWithProgids]
"VisualStudio.wxsfile.%VS_VERSION%"=""
[HKEY_CLASSES_ROOT\VisualStudio.wxsfile.%VS_VERSION%]
@="WiX Source File"
[HKEY_CLASSES_ROOT\VisualStudio.wxsfile.%VS_VERSION%\DefaultIcon]
@="%DLLPATH%,1"
[HKEY_CLASSES_ROOT\VisualStudio.wxsfile.%VS_VERSION%\shell\Open]
@="&Open in Visual Studio %VS_VERSION_YEAR%"
[HKEY_CLASSES_ROOT\VisualStudio.wxsfile.%VS_VERSION%\shell\Open\command]
@="\"%DEVENVPATH%\" \"%1\""

[HKEY_CLASSES_ROOT\.wxi]
@="VisualStudio.wxifile.%VS_VERSION%"
"Content Type"="text/xml"
"PerceivedType"="text"
[HKEY_CLASSES_ROOT\.wxi\OpenWithList\devenv.exe]
[HKEY_CLASSES_ROOT\.wxi\OpenWithProgids]
"VisualStudio.wxifile.%VS_VERSION%"=""
[HKEY_CLASSES_ROOT\VisualStudio.wxifile.%VS_VERSION%]
@="WiX Include File"
[HKEY_CLASSES_ROOT\VisualStudio.wxifile.%VS_VERSION%\DefaultIcon]
@="%DLLPATH%,3"
[HKEY_CLASSES_ROOT\VisualStudio.wxifile.%VS_VERSION%\shell\Open]
@="&Open in Visual Studio %VS_VERSION_YEAR%"
[HKEY_CLASSES_ROOT\VisualStudio.wxifile.%VS_VERSION%\shell\Open\command]
@="\"%DEVENVPATH%\" \"%1\""

[HKEY_CLASSES_ROOT\.wxl]
@="VisualStudio.wxlfile.%VS_VERSION%"
"Content Type"="text/xml"
"PerceivedType"="text"
[HKEY_CLASSES_ROOT\.wxl\OpenWithList\devenv.exe]
[HKEY_CLASSES_ROOT\.wxl\OpenWithProgids]
"VisualStudio.wxlfile.%VS_VERSION%"=""
[HKEY_CLASSES_ROOT\VisualStudio.wxlfile.%VS_VERSION%]
@="WiX Resource File"
[HKEY_CLASSES_ROOT\VisualStudio.wxlfile.%VS_VERSION%\DefaultIcon]
@="%DLLPATH%,5"
[HKEY_CLASSES_ROOT\VisualStudio.wxlfile.%VS_VERSION%\shell\Open]
@="&Open in Visual Studio %VS_VERSION_YEAR%"
[HKEY_CLASSES_ROOT\VisualStudio.wxlfile.%VS_VERSION%\shell\Open\command]
@="\"%DEVENVPATH%\" \"%1\""

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\CLSID\{85550E99-05E7-4778-BD7D-576FC334D522}]
@="Microsoft.Tools.WindowsInstallerXml.VisualStudioInfrastructure.GeneralPropertyPage"
"Class"="Microsoft.Tools.WindowsInstallerXml.VisualStudioInfrastructure.GeneralPropertyPage"
"CodeBase"="%SCONCEPATH%"
"InprocServer32"="%SYSDIR%\\mscoree.dll"
"ThreadingModel"="Both"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Editors\%XML_EDITOR_GUID%\Extensions]
"wxs"=dword:00000028
"wxi"=dword:00000028
"wxl"=dword:00000028

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\InstalledProducts\WiX]
"Package"="%PACKAGE_GUID%"
"UseInterface"=dword:00000001
"ToolsDirectory"="%WIXTOOLSDIR%"
"TraceLevel"="Information"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\NewProjectTemplates\TemplateDirs\%PACKAGE_GUID%\/1]
@="#150"
"TemplatesDir"="%TEMPLATESDIR%Projects"
"SortPriority"=dword:0000001e

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Packages\%PACKAGE_GUID%]
@="WiX Project Package"
"Assembly"=""
"Class"="Microsoft.Tools.WindowsInstallerXml.VisualStudio.WixPackage"
"CodeBase"="%DLLPATH%"
"CompanyName"="Microsoft"
"ID"=dword:00000092
"InprocServer32"="%SYSDIR%\\mscoree.dll"
"MinEdition"="Standard"
"ProductName"="WiX Project Package"
"ProductVersion"="2.0"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Packages\%PACKAGE_GUID%\SatelliteDll]
"Path"="%DLLDIR%"
"DllName"="VotiveUI.dll"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Projects\%PROJECT_GUID%]
@="#150"
"DisplayName"="#150"
"Package"="%PACKAGE_GUID%"
"ProjectTemplatesDir"="%TEMPLATESDIR%Projects"
"ItemTemplatesDir"="%TEMPLATESDIR%ProjectItems"
"DisplayProjectFileExtensions"="#111"
"PossibleProjectExtensions"="wixproj"
"DefaultProjectExtension"=".wixproj"

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Projects\%PROJECT_GUID%\AddItemTemplates\TemplateDirs\%PACKAGE_GUID%\/1]
@="#122"
"TemplatesDir"="%TEMPLATESDIR%ProjectItems"
"SortPriority"=dword:00000014

#[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Projects\%PROJECT_GUID%\Filters\/1]
#@="#117"
#"CommonOpenFilesFilter"=dword:00000000
#"CommonFindFilesFilter"=dword:00000000
#"NotAddExistingItemFilter"=dword:00000000
#"FindInFilesFilter"=dword:00000000
#"NotOpenFileFilter"=dword:00000000
#"SortPriority"=dword:000003e8

#[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%VS_VERSION%Exp\Projects\%PROJECT_GUID%\Filters\/2]
#@="#119"
#"CommonOpenFilesFilter"=dword:00000000
#"CommonFindFilesFilter"=dword:00000000
#"NotAddExistingItemFilter"=dword:00000001
#"FindInFilesFilter"=dword:00000001
#"NotOpenFileFilter"=dword:00000000
#"SortPriority"=dword:000003e8
