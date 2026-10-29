param(
    [string]$OutputDirectory = (Join-Path $PSScriptRoot 'dist/seamless-puzzle-compact-win-x64'),
    [string]$ClipSlimExe,
    [switch]$FrameworkDependent
)
$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$app = Join-Path $OutputDirectory 'app'
$contained = if ($FrameworkDependent) { 'false' } else { 'true' }
& dotnet publish (Join-Path $PSScriptRoot 'src/SeamlessPuzzle/SeamlessPuzzle.csproj') -c Release -r win-x64 --self-contained $contained --output $app --nologo -p:DebugType=None -p:DebugSymbols=false
if ($LASTEXITCODE -ne 0) { throw 'SeamlessPuzzle publish failed.' }
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe' }
& $compiler /nologo /target:winexe /optimize+ /r:System.Windows.Forms.dll /r:Microsoft.CSharp.dll "/out:$(Join-Path $OutputDirectory '启动无缝拼图.exe')" "/win32icon:$(Join-Path $PSScriptRoot 'src/SeamlessPuzzle/Assets/Icons/app.ico')" (Join-Path $PSScriptRoot 'src/PortableLauncher/PortableLauncher.cs')
if ($LASTEXITCODE -ne 0) { throw 'Launcher compilation failed.' }
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LICENSE'),(Join-Path $PSScriptRoot 'README.md'),(Join-Path $PSScriptRoot 'CHANGELOG.md'),(Join-Path $PSScriptRoot 'THIRD_PARTY_NOTICES.md') -Destination $OutputDirectory -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'third-party') -Destination $OutputDirectory -Recurse -Force
if ($ClipSlimExe) {
    Copy-Item -LiteralPath $ClipSlimExe -Destination (Join-Path $OutputDirectory 'ClipSlim-v1.7.2.exe') -Force
}
Write-Output "Built: $OutputDirectory"
