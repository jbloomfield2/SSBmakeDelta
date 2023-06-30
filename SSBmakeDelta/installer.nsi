;NSIS Modern User Interface
;Basic Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"
!include nsDialogs.nsh
!include FileFunc.nsh
;--------------------------------
;General

  ;Name and file
  Name "SSB xdelta tool"
  OutFile "SSBpatchtool.exe"
  Unicode True

  ;Default installation folder
  InstallDir "C:\GEEdit4\patchtool\"
  
  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin

Function WriteToFile
Exch $0 ;file to write to
Exch
Exch $1 ;text to write
 
  FileOpen $0 $0 a #open file
  FileSeek $0 0 END #go to end
  FileWrite $0 $1 #write to file
  FileClose $0
 
Pop $1
Pop $0
FunctionEnd
 
!macro WriteToFile NewLine File String
  !if `${NewLine}` == true
  Push `${String}$\r$\n`
  !else
  Push `${String}`
  !endif
  Push `${File}`
  Call WriteToFile
!macroend
!define WriteToFile `!insertmacro WriteToFile false`
!define WriteLineToFile `!insertmacro WriteToFile true`

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "${NSISDIR}\Docs\Modern UI\License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  Page Custom PageCreate PageLeave
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

LangString PAGE_TITLE 1033 "Select SSB ROM"
LangString PAGE_SUBTITLE 1033 "Please locate your USA SSB rom"

Var RomPath
Function PageCreate
!insertmacro MUI_HEADER_TEXT $(PAGE_TITLE) $(PAGE_SUBTITLE)
nsDialogs::Create 1018
Pop $0

${NSD_CreateText} 0 5u -25u 13u "C:\GEEdit4\Super Smash Bros. (USA).z64"
Pop $RomPath

${NSD_CreateBrowseButton} -23u 4u 20u 15u "..."
Pop $0
${NSD_OnClick} $0 SelRom

nsDialogs::Show

FunctionEnd

Function SelRom
Pop $0
${NSD_GetText} $RomPath $0
nsDialogs::SelectFileDialog open $0 "*.z64"
Pop $0
${If} $0 != ""
    ${NSD_SetText} $RomPath $0
${EndIf}
FunctionEnd

Function PageLeave
${NSD_GetText} $RomPath $0
${GetFileName} $0 $1
FileOpen $3 "$INSTDIR\rompath.ini" w
FileWrite $3 $0
FunctionEnd
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "SSBpatchtool" SecTool

  SetOutPath "$INSTDIR"
  
  ;ADD YOUR OWN FILES HERE...
File SSBMakeDelta.exe
File xdelta3.dll
File System.Buffers.dll
File System.Memory.dll
File System.Runtime.CompilerServices.Unsafe.dll
File xdelta3.net.dll
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

SectionEnd

Section "Right click menu entries" SecRightclick
StrCpy $1 '$INSTDIR\SSBMakeDelta.exe "%1"'
StrCpy $2 '$INSTDIR\SSBMakeDelta.exe "%1" -rdb'

WriteRegStr HKCR "*\shell\SSB apply xdelta" "" "Apply SSB Xdelta"
WriteRegStr HKCR "*\shell\SSB apply xdelta" "AppliesTo" ".xdelta"
WriteRegStr HKCR "*\shell\SSB apply xdelta\command" "" $1

WriteRegStr HKCR "*\shell\SSB create xdelta" "" "Create SSB Xdelta"
WriteRegStr HKCR "*\shell\SSB create xdelta" "AppliesTo" ".z64"
WriteRegStr HKCR "*\shell\SSB create xdelta\command" "" $1

WriteRegStr HKCR "*\shell\SSB create RDB" "" "Create SSB RDB"
WriteRegStr HKCR "*\shell\SSB create RDB" "AppliesTo" ".z64"
WriteRegStr HKCR "*\shell\SSB create RDB\command" "" $2



SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecTool ${LANG_ENGLISH} "Main installation."
  LangString DESC_SecRightClick ${LANG_ENGLISH} "Adds right click menu entry for .z64 and .xdelta files."


  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecTool} $(DESC_SecTool)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecRightClick} $(DESC_SecRightClick)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  Delete "$INSTDIR\Uninstall.exe"

  RMDir "$INSTDIR"

  DeleteRegKey /ifempty HKCU "Software\Modern UI Test"

SectionEnd