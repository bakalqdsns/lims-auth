. "E:\lims-auth\smoke.ps1"
$out = "E:\lims-auth\smoke\agent4.log"
"" | Out-File $out -Encoding utf8
$global:SmokeLog = $out
$token = Load-Token
$h = Auth-H $token
$uniq = [Guid]::NewGuid().ToString().Substring(0,8)
function L($m) { $m | Out-File $out -Append -Encoding utf8; Write-Host $m }

L "=== Agent #4: Experiment (Experiments/TeachingApplications/UsageRegistrations) ==="
L "Timestamp: $(Get-Date -Format o)"

# --- ExperimentsController (route: api/v1/[controller] = api/v1/experiments) ---
L "--- ExperimentsController (path: /api/v1/experiments) ---"
Expect (Get-Json "/api/v1/experiments/tasks" $h) 200 "experiments/tasks list"
Expect (Get-Json "/api/v1/experiments/items" $h) @(200, 404) "experiments/items list"
Expect (Get-Json "/api/v1/experiments/plans" $h) @(200, 404) "experiments/plans list"
Expect (Get-Json "/api/v1/experiments/assessments" $h) @(200, 404) "experiments/assessments list"

# --- TeachingApplicationsController ---
L "--- TeachingApplicationsController (api/v1/teaching-applications) ---"
Expect (Get-Json "/api/v1/teaching-applications" $h) 200 "teaching-applications list"
Expect (Get-Json "/api/v1/teaching-applications/pending" $h) 200 "teaching-applications pending"
Expect (Get-Json "/api/v1/teaching-applications/my" $h) 200 "teaching-applications my"

# --- UsageRegistrationsController ---
L "--- UsageRegistrationsController (api/v1/usage-registrations) ---"
Expect (Get-Json "/api/v1/usage-registrations" $h) 200 "usage-registrations list"
Expect (Get-Json "/api/v1/usage-registrations/pending" $h) 200 "usage-registrations pending"
Expect (Get-Json "/api/v1/usage-registrations/overdue" $h) 200 "usage-registrations overdue"
Expect (Get-Json "/api/v1/usage-registrations/statistics/completion" $h) 200 "usage-registrations completion"

L "=== Agent #4 DONE ==="
