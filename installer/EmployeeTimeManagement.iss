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
; Windows 7 SP1 through 11 (SP1 is checked in InitializeSetup for a clear message).
; Install in 64-bit mode where available; the program is AnyCPU.
MinVersion=6.1
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

; Offline prerequisites are read from the stick at install time (not compiled into Setup.exe):
;   {src}\prerequisites\ndp48-x86-x64-allos-enu.exe
;   {src}\prerequisites\win7\windows6.1-kb4490628-{x86|x64}.msu   servicing stack
;   {src}\prerequisites\win7\windows6.1-kb4474419-{x86|x64}.msu   SHA-2 code signing support
;   {src}\prerequisites\win7\windows6.1-kb3140245-{x86|x64}.msu   TLS 1.2 in WinHTTP
; Only what is missing is installed; the Win7 ones are skipped on Windows 10/11.

[Code]
const
  Net48Release = 528040;
  Net48Setup = 'ndp48-x86-x64-allos-enu.exe';
  ExitRebootRequired = 3010;
  ExitAlreadyInstalled = 2359302;
  Schannel = 'SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client';
  WinHttp = 'SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\WinHttp';
  UninstallKey = 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{6F1C2B7A-3D54-4E8B-9A0C-5B7E2D41F9A3}_is1';

// The version of the install already on this PC, or '' when there is none.
function InstalledVersion(): String;
begin
  Result := '';
  if not RegQueryStringValue(HKLM, UninstallKey, 'DisplayVersion', Result) then
    if not RegQueryStringValue(HKCU, UninstallKey, 'DisplayVersion', Result) then
      Result := '';
end;

function IsWindows7(): Boolean;
var
  V: TWindowsVersion;
begin
  GetWindowsVersionEx(V);
  Result := (V.Major = 6) and (V.Minor = 1);
end;

function IsWindows7WithoutSP1(): Boolean;
var
  V: TWindowsVersion;
begin
  GetWindowsVersionEx(V);
  Result := (V.Major = 6) and (V.Minor = 1) and (V.ServicePackMajor < 1);
end;

// Refuse to install an older version over a newer one, and Windows 7 without SP1.
function InitializeSetup(): Boolean;
var
  Installed: String;
  InstalledPacked, NewPacked: Int64;
begin
  Result := True;
  if IsWindows7WithoutSP1() then
  begin
    MsgBox('{#AppName} needs Windows 7 Service Pack 1.' + #13#10#13#10 +
      'Install Service Pack 1 for Windows 7 (Windows Update, or Microsoft''s standalone ' +
      'installer), restart, then run Setup again.', mbError, MB_OK);
    Result := False;
    Exit;
  end;
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

function IsNet48Installed(): Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
    'Release', Release) and (Release >= Net48Release);
end;

function IsHotfixInstalled(const KB: String): Boolean;
var
  Code: Integer;
begin
  Result := Exec(ExpandConstant('{cmd}'),
    '/C wmic qfe get hotfixid | findstr /i ' + KB + ' >nul', '', SW_HIDE, ewWaitUntilTerminated, Code)
    and (Code = 0);
end;

function ArchSuffix(): String;
begin
  if IsWin64 then
    Result := 'x64'
  else
    Result := 'x86';
end;

// Runs one prerequisite from the stick. Returns '' on success, else the reason it failed.
// Sets NeedsRestart when the installer asks for a reboot.
function RunPrerequisite(const Title, FileName, Params: String; IsMsu: Boolean;
  var NeedsRestart: Boolean): String;
var
  Path, Exe, Args: String;
  Code: Integer;
begin
  Path := ExpandConstant('{src}\') + FileName;
  if not FileExists(Path) then
  begin
    Result := Title + ' is missing from the USB stick:' + #13#10 + Path;
    Exit;
  end;
  WizardForm.StatusLabel.Caption := 'Installing ' + Title + '...';
  if IsMsu then
  begin
    Exe := ExpandConstant('{sys}\wusa.exe');
    Args := '"' + Path + '" /quiet /norestart';
  end
  else
  begin
    Exe := Path;
    Args := Params;
  end;
  if not Exec(Exe, Args, '', SW_HIDE, ewWaitUntilTerminated, Code) then
    Result := Title + ' could not be started: ' + SysErrorMessage(Code)
  else if (Code = ExitRebootRequired) or (Code = 1641) then
    NeedsRestart := True
  else if (Code <> 0) and (Code <> ExitAlreadyInstalled) then
    Result := Title + ' failed to install (exit code ' + IntToStr(Code) + ').';
end;

procedure EnableTls12Client();
begin
  RegWriteDWordValue(HKLM, Schannel, 'DisabledByDefault', 0);
  RegWriteDWordValue(HKLM, Schannel, 'Enabled', 1);
  // TLS 1.1 + 1.2 as WinHTTP defaults (KB3140245).
  RegWriteDWordValue(HKLM, WinHttp, 'DefaultSecureProtocols', $00000A00);
end;

function InstallWin7Prerequisite(const KB, Title: String; var NeedsRestart: Boolean): String;
begin
  Result := '';
  if not IsHotfixInstalled(KB) then
    Result := RunPrerequisite(Title, 'prerequisites\win7\windows6.1-' + LowerCase(KB) + '-' +
      ArchSuffix() + '.msu', '', True, NeedsRestart);
end;

// Installs only what is missing, from the stick, with no internet. A non-empty result aborts setup.
function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := '';
  if IsWindows7() then
  begin
    Result := InstallWin7Prerequisite('KB4490628', 'Windows 7 servicing stack update', NeedsRestart);
    if Result = '' then
      Result := InstallWin7Prerequisite('KB4474419', 'Windows 7 SHA-2 update', NeedsRestart);
    if Result = '' then
      Result := InstallWin7Prerequisite('KB3140245', 'Windows 7 TLS 1.2 update', NeedsRestart);
    if Result = '' then
      EnableTls12Client();
    // Later updates need the earlier ones active, so reboot before continuing.
    if (Result = '') and NeedsRestart then
      Exit;
  end;
  if (Result = '') and not IsNet48Installed() then
    Result := RunPrerequisite('.NET Framework 4.8', 'prerequisites\' + Net48Setup,
      '/q /norestart', False, NeedsRestart);
end;
