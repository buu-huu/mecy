!include "MUI.nsh"
!define INSTALLATIONNAME "Mecy"
!define PRODUCT_NAME "Mecy"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "buuhuu"
!define PRODUCT_WEB_SITE "https://github.com/buu-huu/mecy"

Name "Mecy Installer"

OutFile "MecyInstaller.exe"
InstallDir $PROGRAMFILES\${INSTALLATIONNAME}

!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "German"

Section ""
    SetOutPath $INSTDIR
    File /r "..\MecyApplication\bin\Debug\*"
    WriteUninstaller $INSTDIR\uninstall.exe
SectionEnd

Section "Start Menu Shortcuts"
    CreateDirectory "$SMPROGRAMS\${INSTALLATIONNAME}"
    CreateShortCut "$SMPROGRAMS\${INSTALLATIONNAME}\UninstallMecy.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
    CreateShortCut "$SMPROGRAMS\${INSTALLATIONNAME}\Mecy.lnk" "$INSTDIR\MecyApplication.exe" "" "$INSTDIR\MecyApplication.exe" 0
SectionEnd

Section "Uninstall"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INSTALLATIONNAME}"

    Delete $INSTDIR\uninstall.exe
    Delete $INSTDIR\MecyApplication.exe
    RMDir $INSTDIR

    Delete "$SMPROGRAMS\${INSTALLATIONNAME}\*.*"
    RMDir "$SMPROGRAMS\${INSTALLATIONNAME}"
SectionEnd