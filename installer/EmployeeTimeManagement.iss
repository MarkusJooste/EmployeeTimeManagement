; Inno Setup 6 script for Employee Time Management. Compile with ISCC.exe.
;
; Defines (override on the ISCC command line with /D...):
;   BuildDir   folder holding the Release build output (default: ..\EmployeeTimeManagement\bin\Release)
;   OutputDir  where Setup.exe is written (default: Output)
;
; The version comes from the built exe, so it is bumped in one place (AssemblyInfo.cs).

#ifndef BuildDir
  #define BuildDir "..\EmployeeTimeManagement\bin\Release"
#endif
#ifndef OutputDir
  #define OutputDir "Output"
#endif

#define AppName "Employee Time Management"
#define AppExe "EmployeeTimeManagement.exe"
#define AppVersion GetVersionNumbersString(BuildDir + "\" + AppExe)

[Setup]
; Fixed ID: a later Setup.exe finds this install and upgrades it in place. Never change it.
AppId={{6F1C2B7A-3D54-4E8B-9A0C-5B7E2D41F9A3}
AppName={#AppName}
AppVersion={#AppVersion}
VersionInfoVersion={#AppVersion}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
UninstallDisplayIcon={app}\{#AppExe}
OutputDir={#OutputDir}
OutputBaseFilename=Setup
Compression=lzma2
SolidCompression=yes
; Windows 7 SP1 through 11. Install in 64-bit mode where available; the program is AnyCPU.
MinVersion=6.1sp1
ArchitecturesInstallIn64BitMode=x64os
PrivilegesRequired=admin
; Close the running program (and restart nothing) before files are replaced.
CloseApplications=yes
CloseApplicationsFilter={#AppExe}
RestartApplications=no
DisableProgramGroupPage=yes

[Files]
Source: "{#BuildDir}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.vshost.*,.env"; Flags: recursesubdirs createallsubdirs ignoreversion
; The .env sits next to Setup.exe on the stick. The program imports it into the machine's
; encrypted store on first run and deletes this copy. Skipped when the stick has none.
Source: "{src}\.env"; DestDir: "{app}"; Flags: external skipifsourcedoesntexist ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"

[Run]
; Launches the program at the end of setup (it runs the startup connection flow itself).
Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"; Flags: nowait skipifsilent

; Connection data and logs live in ProgramData, outside {app}, and are deliberately not
; listed in [UninstallDelete], so upgrades and uninstalls keep them.

[Code]
const
  UninstallKey = 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{6F1C2B7A-3D54-4E8B-9A0C-5B7E2D41F9A3}_is1';

// The version of the install already on this PC, or '' when there is none.
function InstalledVersion(): String;
begin
  Result := '';
  if not RegQueryStringValue(HKLM, UninstallKey, 'DisplayVersion', Result) then
    if not RegQueryStringValue(HKCU, UninstallKey, 'DisplayVersion', Result) then
      Result := '';
end;

// Refuse to install an older version over a newer one.
function InitializeSetup(): Boolean;
var
  Installed: String;
  InstalledPacked, NewPacked: Int64;
begin
  Result := True;
  Installed := InstalledVersion();
  if Installed = '' then
    Exit;
  if StrToVersion(Installed, InstalledPacked) and
     StrToVersion('{#AppVersion}', NewPacked) then
    if ComparePackedVersion(InstalledPacked, NewPacked) > 0 then
    begin
      MsgBox('{#AppName} ' + Installed + ' is already installed, which is newer than this ' +
        'setup (' + '{#AppVersion}' + ').' + #13#10#13#10 +
        'Setup will not install an older version over a newer one. Use the newest USB stick.',
        mbError, MB_OK);
      Result := False;
    end;
end;
