!include "MUI2.nsh"
!define INSTALLATIONNAME "Mecy"
!define PRODUCT_NAME "Mecy"
!define PRODUCT_VERSION "1.2"
!define PRODUCT_PUBLISHER "buuhuu"
!define PRODUCT_WEB_SITE "https://github.com/buu-huu/mecy"
!define MUI_ICON "..\..\MecyApplication\Resources\256x256.ico"

Name "Mecy 1.2"

OutFile "MecyInstaller.exe"
InstallDir $PROGRAMFILES\${INSTALLATIONNAME}

# GUI changes
!insertmacro MUI_PAGE_LICENSE "..\..\LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "German"

# Install files
Section ""
    SetOutPath $INSTDIR
    File /r "..\..\MecyApplication\bin\x64\Release\*"
    WriteUninstaller $INSTDIR\uninstall.exe

    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "DisplayName" "Mecy"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "Publisher" "buuhuu"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "URLInfoAbout" "https://github.com/buu-huu/mecy"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "DisplayIcon" '"$INSTDIR\Resources\256x256.ico"'
    
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "DisplayVersion" "1.1"

    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}" "NoRepair" 1
SectionEnd

# Create start menu shortcut
Section "Start Menu Shortcuts"
    CreateDirectory "$SMPROGRAMS\${INSTALLATIONNAME}"
    CreateShortCut "$SMPROGRAMS\${INSTALLATIONNAME}\UninstallMecy.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
    CreateShortCut "$SMPROGRAMS\${INSTALLATIONNAME}\Mecy.lnk" "$INSTDIR\MecyApplication.exe" "" "$INSTDIR\MecyApplication.exe" 0
SectionEnd

# Create desktop shortcut
Section "Desktop Shortcut"
    CreateShortCut "$DESKTOP\Mecy.lnk" "$INSTDIR\MecyApplication.exe"
SectionEnd

# Create uninstaller
Section "Uninstall"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}"

    Delete $INSTDIR\uninstall.exe
    Delete $INSTDIR\MecyApplication.exe
    RMDir $INSTDIR

    Delete "$SMPROGRAMS\${INSTALLATIONNAME}\*.*"
    RMDir "$SMPROGRAMS\${INSTALLATIONNAME}"
    Delete "$DESKTOP\Mecy.lnk"
SectionEnd