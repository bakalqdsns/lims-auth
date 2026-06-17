. "E:\lims-auth\smoke.ps1"
$r = Get-Json "/api/v1/auth/health"
Show $r
Write-Host "--- LOGIN ADMIN ---"
$token = Login "admin" "Admin@123"
if ($token) {
    Write-Host ("OK token len: " + $token.Length)
    $token | Out-File -FilePath "E:\lims-auth\.admin_token.txt" -Encoding ascii
} else {
    Write-Host "LOGIN FAIL"
}
Write-Host "--- LOGIN TEACHER ---"
$ttoken = Login "teacher" "Admin@123"
if ($ttoken) {
    Write-Host ("OK teacher token len: " + $ttoken.Length)
    $ttoken | Out-File -FilePath "E:\lims-auth\.teacher_token.txt" -Encoding ascii
} else {
    Write-Host "TEACHER LOGIN FAIL"
}
