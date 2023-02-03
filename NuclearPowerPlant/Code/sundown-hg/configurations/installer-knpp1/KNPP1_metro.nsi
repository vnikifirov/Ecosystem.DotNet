;NSIS Modern User Interface
SetCompressor /SOLID lzma
  
;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

  !include "WordFunc.nsh"


;  !define PLACE_UNINSTALL

  !include ".\functions.nsh"
;--------------------------------
;General

  ;Name and file
  Name "���� ��� ��� ����� 1����"
  OutFile "KNPP1_metro.exe"

  ;Default installation folder
  InstallDir "$LOCALAPPDATA\KNPP1_Metro"
  
  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\KNPP1_Metro" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel user

;--------------------------------
;Variables

  Var StartMenuFolder

  Var FinishText
  Var FinishTitle

  Var FinishLink
  Var FinishLinkLocation
  
  Var CheckError
;--------------------------------
;Interface Settings

 !define MUI_ABORTWARNING

;--------------------------------
;Pages

 !define MINIMUM_DOTNET "2.0.50727"
;  !define MINIMUM_DOTNET "1.0.0"
;  !define MINIMUM_DOTNET "3.0.0"

  ;Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU"
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\KNPP1_Metro"
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "���� ������ ��� ��� ����� ���� 1"

  !define MUI_FINISHPAGE_TITLE_3LINES

  !define MUI_FINISHPAGE_TEXT $FinishText
  !define MUI_FINISHPAGE_TITLE $FinishTitle
  !define MUI_FINISHPAGE_LINK $FinishLink
  !define MUI_FINISHPAGE_LINK_LOCATION $FinishLinkLocation

  ;!insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  ;!insertmacro MUI_PAGE_COMPONENTS

  !define MUI_PAGE_CUSTOMFUNCTION_PRE pre_install
  !insertmacro MUI_PAGE_DIRECTORY

  !define MUI_PAGE_CUSTOMFUNCTION_PRE pre_check
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !define MUI_PAGE_CUSTOMFUNCTION_PRE pre_check
  !insertmacro MUI_PAGE_INSTFILES
 
  !insertmacro MUI_PAGE_FINISH
  

  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES


;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "Russian"

;--------------------------------
;Installer Sections


Function pre_install
    Call IsDotNETInstalled
    Pop $0
;    StrCpy $0 0   ; ����� ������������� ������� ��� .NET �� ����������
    StrCmp $0 1 check fail
    
fail:
    StrCpy $FinishText "� ������� �� ������� ����� ��������� .NET Framework ${MINIMUM_DOTNET}. ���������� ������ ��������� � ��������� ��������� ��������� ������. ����� ������� ��� ������ ������� �� ������ ����."
    goto raiseError

check:
    StrCpy $1 $2 "" 1
    ${VersionCompare} $1 ${MINIMUM_DOTNET} $3
    StrCmp $3 2 lowVer ok

lowVer:
    StrCpy $FinishText "� ������� ���������� ��������� .NET Framework ������ $2, ���� ��� ���������������� ��������� ��������� �� ���� ${MINIMUM_DOTNET} . ���������� ������ ��������� � ��������� ��������� ��������� ������. ����� ������� ��� ������ ������� �� ������ ����."
        
raiseError:
    StrCpy $FinishTitle "��������� �� �������"
        
    StrCpy $FinishLink "��������� Microsoft .NET Framework 2.0"
    StrCpy $FinishLinkLocation "http://www.microsoft.com/downloads/details.aspx?familyid=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5"
        
    StrCpy $CheckError 1
    abort
ok:

    StrCpy $FinishTitle "���� ��� ��� ����� ��� 1 ����� ����������� �������"
    StrCpy $FinishText "����� ���������� � ������ ������� � ���� ����->��������� � �������� '���� ��� ��� ����� 1����'."
functionend


function pre_check
	${If} $CheckError <> 0
		Abort
	${EndIf}
functionEnd


Section ;"Dummy Section" SecDummy
   
  SetOutPath "$INSTDIR"
  
  ;ADD YOUR OWN FILES HERE...
  ;File bin\*.*
;  File bin\AkgoSqlDb.dll
  File bin\Algorithms.dll
;;;  File bin\Boo.Lang.Compiler.dll
;;;  File bin\Boo.Lang.dll
;;;  File bin\Boo.Lang.Parser.dll
  File bin\config.xml
  File bin\corelib.dll
  File bin\ICSharpCode.TextEditor.dll
  File bin\KNPP1.csv
;  File bin\PgSqlStorage.dll
;  File bin\Npgsql.dll
  File bin\pvkTuple.tup
  File bin\RecoveryFactory.exe
;;;  File bin\Rhino.DSL.dll
  File bin\RockMicoPlugin.dll
  File bin\SqlLiteStorage.dll
  File bin\System.Data.SQLite.dll
  File bin\WindowsApplication1.dll
  
  ;Store installation folder
  WriteRegStr HKCU "Software\KNPP1_Metro" "" $INSTDIR
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\������ ��� ����� ���� 1.lnk" "$INSTDIR\RecoveryFactory.exe"
    !ifdef    PLACE_UNINSTALL
      CreateShortCut "$SMPROGRAMS\$StartMenuFolder\�������.lnk" "$INSTDIR\Uninstall.exe"
    !endif
  
  !insertmacro MUI_STARTMENU_WRITE_END

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "A test section."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END
 
;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  Delete "$INSTDIR\Uninstall.exe"

;  Delete "$INSTDIR\AkgoSqlDb.dll"
  Delete "$INSTDIR\Algorithms.dll"
  Delete "$INSTDIR\Boo.Lang.Compiler.dll"
  Delete "$INSTDIR\Boo.Lang.dll"
  Delete "$INSTDIR\Boo.Lang.Parser.dll"
  Delete "$INSTDIR\config.xml"
  Delete "$INSTDIR\corelib.dll"
  Delete "$INSTDIR\ICSharpCode.TextEditor.dll"
  Delete "$INSTDIR\KNPP1.csv"
;  Delete "$INSTDIR\PgSqlStorage.dll"
;  Delete "$INSTDIR\Npgsql.dll"
  Delete "$INSTDIR\pvkTuple.tup"
  Delete "$INSTDIR\RecoveryFactory.exe"
  Delete "$INSTDIR\Rhino.DSL.dll"
  Delete "$INSTDIR\RockMicoPlugin.dll"
  Delete "$INSTDIR\SqlLiteStorage.dll"
  Delete "$INSTDIR\System.Data.SQLite.dll"
  Delete "$INSTDIR\WindowsApplication1.dll"

  RMDir "$INSTDIR"
  
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder

!ifdef    PLACE_UNINSTALL
  Delete "$SMPROGRAMS\$StartMenuFolder\�������.lnk"
!endif

  Delete "$SMPROGRAMS\$StartMenuFolder\������ ��� ����� ���� 1.lnk"
  RMDir "$SMPROGRAMS\$StartMenuFolder"
  
  DeleteRegKey /ifempty HKCU "Software\KNPP1_Metro"

SectionEnd