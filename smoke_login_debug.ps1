. "E:\lims-auth\smoke.ps1"
$body = @{ username = "admin"; password = "Admin@123" } | ConvertTo-Json -Compress
$r = Post-Json "/api/v1/auth/login" $body
Write-Host "HTTP $($r.code)"
Write-Host "Body: $($r.body)"
