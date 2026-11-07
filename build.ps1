param([Parameter(Mandatory=$true)][string]$GameDirectory)
$ErrorActionPreference = 'Stop'
$core = Join-Path $GameDirectory 'BepInEx\core'
$managed = Join-Path $GameDirectory 'CasualtiesUnknown_Data\Managed'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
Push-Location $PSScriptRoot
try {
    & $compiler /nologo /target:library /optimize+ /out:TutorialLayerUnlock.dll "/reference:$core\BepInEx.dll" "/reference:$core\0Harmony.dll" "/reference:$managed\UnityEngine.dll" "/reference:$managed\UnityEngine.CoreModule.dll" "/reference:$managed\netstandard.dll" Plugin.cs TutorialUnlockPatch.cs
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    Write-Host 'Built TutorialLayerUnlock.dll'
} finally { Pop-Location }
