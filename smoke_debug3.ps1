. "E:\lims-auth\smoke.ps1"
$tok = Login "admin" "Admin@123"
Write-Host "tok type: $($tok.GetType().FullName)"
Write-Host "tok is null: $($null -eq $tok)"
Write-Host "tok is empty: $($tok -eq '')"
Write-Host "tok length: $($tok.Length)"
if ($tok) { Write-Host "BRANCH: truthy" } else { Write-Host "BRANCH: falsy" }
