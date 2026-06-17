$paths = @(
    "E:\lims-auth\smoke.ps1",
    "E:\lims-auth\smoke_login.ps1",
    "E:\lims-auth\smoke\relogin.ps1",
    "E:\lims-auth\smoke\agent1.ps1",
    "E:\lims-auth\smoke\agent2.ps1",
    "E:\lims-auth\smoke\agent3.ps1",
    "E:\lims-auth\smoke\agent4.ps1",
    "E:\lims-auth\smoke\agent5.ps1",
    "E:\lims-auth\smoke\agent6.ps1"
)
foreach ($p in $paths) {
    if (-not (Test-Path $p)) { continue }
    $b = [System.IO.File]::ReadAllBytes($p)
    if ($b.Length -gt 2 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) {
        $b2 = $b[3..($b.Length - 1)]
        [System.IO.File]::WriteAllBytes($p, $b2)
        Write-Host ("BOM stripped: " + $p)
    } else {
        Write-Host ("No BOM:      " + $p)
    }
}
