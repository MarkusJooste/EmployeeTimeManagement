<#
.SYNOPSIS
  Builds the program and assembles the USB stick folder.

.DESCRIPTION
  Output (default installer\Stick):
    Setup.exe
    .env                 placeholder; the team fills it in before copying to the stick
    README.txt           fresh install and update steps
    prerequisites\...    copied from installer\prerequisites (binaries are not in the repo)

  The version comes from AssemblyInfo.cs (the one place to bump per release).

.PARAMETER Output
  Stick folder to (re)create.
.PARAMETER PrerequisitesDir
  Folder holding the offline prerequisite binaries, laid out as the installer expects.
#>
param(
  [string]$Output = (Join-Path $PSScriptRoot 'Stick'),
  [string]$PrerequisitesDir = (Join-Path $PSScriptRoot 'prerequisites')
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$proj = Join-Path $repo 'EmployeeTimeManagement\EmployeeTimeManagement.csproj'
$buildDir = Join-Path $repo 'EmployeeTimeManagement\bin\Release'
$iss = Join-Path $PSScriptRoot 'EmployeeTimeManagement.iss'

$requiredPrerequisites = @(
  'ndp48-x86-x64-allos-enu.exe'
) + (@('kb4490628', 'kb4474419', 'kb3140245') | ForEach-Object {
  $kb = $_
  foreach ($arch in 'x86', 'x64') { "win7\windows6.1-$kb-$arch.msu" }
})

function Find-Tool([string]$name, [string[]]$candidates) {
  foreach ($c in $candidates) { if ($c -and (Test-Path $c)) { return (Resolve-Path $c).Path } }
  $cmd = Get-Command $name -ErrorAction SilentlyContinue
  if ($cmd) { return $cmd.Source }
  throw "$name not found. Install it or add it to PATH."
}

function Find-MSBuild {
  $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
  if (Test-Path $vswhere) {
    $found = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
    if ($found) { return $found }
  }
  Find-Tool 'msbuild.exe' @()
}

# Fail early on missing prerequisites, before spending time on the build.
$missing = $requiredPrerequisites | Where-Object { -not (Test-Path (Join-Path $PrerequisitesDir $_)) }
if ($missing) {
  throw "Missing prerequisite files in ${PrerequisitesDir}:`n  " + ($missing -join "`n  ")
}

$msbuild = Find-MSBuild
$iscc = Find-Tool 'ISCC.exe' @(
  (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
  (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'),
  (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe')
)

Write-Host "Building Release..."
& $msbuild $proj -restore -t:Rebuild -p:Configuration=Release -v:minimal
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

$exe = Join-Path $buildDir 'EmployeeTimeManagement.exe'
$programVersion = [Version](Get-Item $exe).VersionInfo.FileVersion

if (Test-Path $Output) { Remove-Item $Output -Recurse -Force }
New-Item -ItemType Directory -Path $Output | Out-Null

Write-Host "Compiling installer..."
& $iscc "/DBuildDir=$buildDir" "/DOutputDir=$Output" $iss
if ($LASTEXITCODE -ne 0) { throw "Inno Setup compile failed." }

# Setup.exe must carry the same version as the program.
$setupVersion = [Version](Get-Item (Join-Path $Output 'Setup.exe')).VersionInfo.FileVersion
if ($setupVersion -ne $programVersion) {
  throw "Version mismatch: program $programVersion, Setup.exe $setupVersion."
}

Write-Host "Copying prerequisites..."
foreach ($rel in $requiredPrerequisites) {
  $dest = Join-Path $Output "prerequisites\$rel"
  New-Item -ItemType Directory -Force -Path (Split-Path $dest) | Out-Null
  Copy-Item (Join-Path $PrerequisitesDir $rel) $dest
}

@'
# Database connection for Employee Time Management.
# Fill in the real values BEFORE copying this stick to the customer, then keep the file name ".env".
# Setup copies it to the PC; the program encrypts it into the machine's store on first run and
# deletes the plaintext copy. Delete this file from the stick if the PC already has a saved connection.
#
MYSQL_CONNECTION_STRING=Server=HOST;Port=3306;Database=DBNAME;Uid=USER;Pwd=PASSWORD;
'@ | Set-Content (Join-Path $Output '.env') -Encoding ASCII

@"
Employee Time Management - version $programVersion
==========================================

FRESH INSTALL
1. Plug in the stick. No internet is needed.
2. Run Setup.exe (click Yes at the admin prompt; SmartScreen may warn: More info > Run anyway).
3. Setup installs what Windows is missing (.NET Framework 4.8; on Windows 7 also SP1 check and
   TLS 1.2 updates). Restart if asked; Setup continues afterwards.
4. The program opens at the end. It tests the connection from the .env on the stick:
   - works: you go straight in
   - fails: choose Retry or Continue for now
   - no .env: choose Create (enter the details) or Continue for now
5. Check the version number on the login screen.

UPDATE FROM A NEW STICK
1. Plug in the new stick and run its Setup.exe. Do NOT uninstall first.
2. Setup closes the program, replaces the files, and keeps the saved connection and logs.
3. Setup refuses to install an older version over a newer one.
4. The .env on the new stick is only needed if the connection details changed.

PROBLEMS
Use "Copy diagnostics" on the error dialog and send the text to the team, quoting the version.
Logs: C:\ProgramData\EmployeeTimeManagement\logs
"@ | Set-Content (Join-Path $Output 'README.txt') -Encoding ASCII

Write-Host "Done: $Output (version $programVersion)"
