#!/usr/bin/pwsh
Clear-Host
$trim = $true

if ($PSVersionTable.PSVersion.Major -lt 6)
{
    Write-Host 'Restarting using pwsh...'
    pwsh $PSCommandPath
    return
}

dotnet publish -v:q -r win-x64 -f net9.0 --self-contained -c Release -o distrib/win/ WgsConverter/WgsConverter.csproj /p:PublishTrimmed=$trim /p:PublishSingleFile=True

Write-Host 'Clearing extra files in distrib...' -ForegroundColor Cyan
Get-ChildItem -LiteralPath distrib -Include *.pdb,*.config -Recurse | Remove-Item

Compress-Archive -LiteralPath distrib/win/ -DestinationPath distrib/wgs_converter.zip -CompressionLevel Optimal -Force

Write-Host 'Done' -ForegroundColor Cyan


