. "E:\lims-auth\smoke.ps1"
$t = Load-Token
$h = Auth-H $t

$paths = @(
    "/api/Experiments",
    "/api/Experiments/tasks",
    "/api/export",
    "/api/export/labs",
    "/api/export/excel",
    "/api/Export",
    "/api/v1/Experiments",
    "/api/v1/Export",
    "/api/v1/export"
)
foreach ($p in $paths) {
    $r = Get-Json $p $h
    Write-Host ("{0,-30} HTTP={1}" -f $p, $r.code)
}
