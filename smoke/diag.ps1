. "E:\lims-auth\smoke.ps1"
Write-Host ("Base: " + $global:Base)
Write-Host ("Get-Json function: " + (Get-Command Get-Json -ErrorAction SilentlyContinue))
Write-Host ("Login function: " + (Get-Command Login -ErrorAction SilentlyContinue))
