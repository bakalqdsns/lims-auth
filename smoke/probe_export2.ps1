. "E:\lims-auth\smoke.ps1"
$t = Load-Token
$h = Auth-H $t
$paths = @(
    "/api/export/templates",
    "/api/export/experiment/task-list",
    "/api/export/experiment/schedule-plan"
)
foreach ($p in $paths) {
    $r = Get-Json $p $h
    Write-Host ("{0,-50} HTTP={1}  len={2}" -f $p, $r.code, $r.body.Length)
}
