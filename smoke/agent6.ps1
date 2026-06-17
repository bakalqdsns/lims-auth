. "E:\lims-auth\smoke.ps1"
$out = "E:\lims-auth\smoke\agent6.log"
"" | Out-File $out -Encoding utf8
$global:SmokeLog = $out
$token = Load-Token
$h = Auth-H $token
function L($m) { $m | Out-File $out -Append -Encoding utf8; Write-Host $m }

L "=== Agent #6: Export (ExportController) ==="
L "ExportController route: api/v1/export"

$paths = @(
    @{ path = "/api/v1/export/templates"; expected = 200 }
    @{ path = "/api/v1/export/experiment/task-list"; expected = 200 }
    @{ path = "/api/v1/export/experiment/schedule-plan"; expected = 200 }
)
foreach ($entry in $paths) {
    $p = $entry.path
    $exp = $entry.expected
    $r = Get-Json $p $h
    if ($r.code -eq $exp) {
        L "[PASS] $p  HTTP=$($r.code)  (expected $exp, length=$($r.body.Length))"
    } else {
        L "[FAIL] $p  HTTP=$($r.code)  expected $exp"
        L "       body: $(Short-Body $r.body 400)"
    }
}

L "=== Agent #6 DONE ==="
