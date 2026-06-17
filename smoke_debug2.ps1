. "E:\lims-auth\smoke.ps1"
$body = @{ username = "admin"; password = "Admin@123" } | ConvertTo-Json -Compress
$r = Post-Json "/api/v1/auth/login" $body
Write-Host "HTTP $($r.code)"
Write-Host "Body (first 300): $($r.body.Substring(0, [Math]::Min(300, $r.body.Length)))"
$obj = $r.body | ConvertFrom-Json
Write-Host "obj.code = $($obj.code)"
Write-Host "obj.data.token = $($obj.data.token.Substring(0, 40))..."
$tok = Login "admin" "Admin@123"
Write-Host "Login returned: $tok"
