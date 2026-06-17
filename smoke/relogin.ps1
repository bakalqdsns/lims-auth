. "E:\lims-auth\smoke.ps1"
$global:SmokeLog = $null
$r = Get-Json "/api/v1/auth/health"
Write-Host ("health HTTP=" + $r.code)
$token = Login "admin" "Admin@123"
if ($token) {
    $token | Out-File "E:\lims-auth\.admin_token.txt" -Encoding ascii
    Write-Host ("OK token len=" + $token.Length)
} else {
    Write-Host "LOGIN FAIL"
}
