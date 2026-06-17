. "E:\lims-auth\smoke.ps1"
$out = "E:\lims-auth\smoke\agent5.log"
"" | Out-File $out -Encoding utf8
$global:SmokeLog = $out
$token = Load-Token
$h = Auth-H $token
$uniq = [Guid]::NewGuid().ToString().Substring(0,8)
function L($m) { $m | Out-File $out -Append -Encoding utf8; Write-Host $m }

L "=== Agent #5: Schedule (Schedules/Reservations/Statistics) ==="
L "Timestamp: $(Get-Date -Format o)"

# --- SchedulesController ---
L "--- SchedulesController (api/v1/schedules) ---"
Expect (Get-Json "/api/v1/schedules" $h) 200 "schedules list"
Expect (Get-Json "/api/v1/schedules/table-view" $h) @(200, 404) "schedules table-view"
Expect (Get-Json "/api/v1/schedules/available-labs" $h) @(200, 404) "schedules available-labs"

# --- ReservationsController ---
L "--- ReservationsController (api/v1/reservations) ---"
Expect (Get-Json "/api/v1/reservations" $h) 200 "reservations list"
Expect (Get-Json "/api/v1/reservations/pending" $h) 200 "reservations pending"

# --- StatisticsController ---
L "--- StatisticsController (api/v1/statistics) ---"
Expect (Get-Json "/api/v1/statistics/weekly-summary" $h) 200 "statistics weekly-summary"
Expect (Get-Json "/api/v1/statistics/lab-usage" $h) 200 "statistics lab-usage"
Expect (Get-Json "/api/v1/statistics/by-major" $h) 200 "statistics by-major"
Expect (Get-Json "/api/v1/statistics/by-class" $h) 200 "statistics by-class"
Expect (Get-Json "/api/v1/statistics/by-grade" $h) 200 "statistics by-grade"
Expect (Get-Json "/api/v1/statistics/by-course" $h) 200 "statistics by-course"
Expect (Get-Json "/api/v1/statistics/reservation" $h) 200 "statistics reservation"
Expect (Get-Json "/api/v1/statistics/completion-rate" $h) 200 "statistics completion-rate"
Expect (Get-Json "/api/v1/statistics/dashboard" $h) 200 "statistics dashboard"
Expect (Get-Json "/api/v1/statistics/export" $h) 200 "statistics export"

L "=== Agent #5 DONE ==="
