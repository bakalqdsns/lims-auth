. "E:\lims-auth\smoke.ps1"
$out = "E:\lims-auth\smoke\agent2.log"
"" | Out-File $out -Encoding utf8
$global:SmokeLog = $out
$token = Load-Token
$h = Auth-H $token
$uniq = [Guid]::NewGuid().ToString().Substring(0,8)
function L($m) { $m | Out-File $out -Append -Encoding utf8; Write-Host $m }

L "=== Agent #2: Teaching (Semesters/Calendar/Courses/Majors/Classes/TeachingTasks/PeriodTimes) ==="
L "Timestamp: $(Get-Date -Format o)"

# --- SemestersController ---
L "--- SemestersController (api/v1/semesters) ---"
Expect (Get-Json "/api/v1/semesters" $h) 200 "semesters list"
Expect (Get-Json "/api/v1/semesters/current" $h) @(200, 404) "semesters/current"
Expect (Get-Json "/api/v1/semesters/current/today" $h) @(200, 404) "semesters/current/today"

# --- CalendarController ---
L "--- CalendarController (api/v1/calendar) ---"
Expect (Get-Json "/api/v1/calendar" $h) @(200, 404) "calendar list"
Expect (Get-Json "/api/v1/calendar/today" $h) @(200, 404) "calendar/today"
Expect (Get-Json "/api/v1/calendar/week-info" $h) @(200, 404) "calendar/week-info"
Expect (Get-Json "/api/v1/calendar/event-types" $h) @(200, 404) "calendar/event-types"
Expect (Get-Json "/api/v1/calendar/holidays" $h) @(200, 404) "calendar/holidays"
Expect (Get-Json "/api/v1/calendar/check-permission" $h) @(200, 400) "calendar/check-permission (requires businessType query)"

# --- CoursesController ---
L "--- CoursesController (api/v1/courses) ---"
Expect (Get-Json "/api/v1/courses" $h) 200 "courses list"
$newCourse = @{ code = "TCOURSE-$uniq"; name = "Test Course"; credits = 2.0; hours = 32; isActive = $true }
$r = Post-Json "/api/v1/courses" $newCourse $h
Expect $r 200 "courses create"
$cid = $null
try { $cid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($cid) {
    Expect (Get-Json "/api/v1/courses/$cid" $h) 200 "courses get by id"
    Expect (Put-Json "/api/v1/courses/$cid" @{ name = "Test Course 2" } $h) 200 "courses update"
    Expect (Patch-Json "/api/v1/courses/$cid/status" @{ isActive = $false } $h) 200 "courses patch status"
    Expect (Delete-Json "/api/v1/courses/$cid" $h) 200 "courses delete"
}

# --- MajorsController ---
L "--- MajorsController (api/v1/majors) ---"
Expect (Get-Json "/api/v1/majors" $h) 200 "majors list"
Expect (Get-Json "/api/v1/majors/all" $h) 200 "majors/all"
# pick an existing department from seed (departments/all returns the list)
$deptList = Get-Json "/api/v1/departments/all" $h
$deptId = $null
try {
    $arr = ($deptList.body | ConvertFrom-Json).data
    if ($arr -and $arr.Count -gt 0) { $deptId = $arr[0].id }
} catch {}
L "  using departmentId: $deptId"
$newMajor = @{
    code = "TMAJ-$uniq"; name = "Test Major"; isActive = $true
    duration = 4; departmentId = $deptId
}
$r = Post-Json "/api/v1/majors" $newMajor $h
Expect $r 200 "majors create"
$mid = $null
try { $mid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($mid) {
    Expect (Get-Json "/api/v1/majors/$mid" $h) 200 "majors get by id"
    Expect (Put-Json "/api/v1/majors/$mid" @{ name = "Test Major 2" } $h) 200 "majors update"
    Expect (Delete-Json "/api/v1/majors/$mid" $h) 200 "majors delete"
}

# --- ClassesController ---
L "--- ClassesController (api/v1/classes) ---"
Expect (Get-Json "/api/v1/classes" $h) 200 "classes list"
# create a parent major first
$parentMajor = Post-Json "/api/v1/majors" @{
    code = "TMAJP-$uniq"; name = "Parent Major"; departmentId = $deptId
    isActive = $true; duration = 4
} $h
$parentMajorId = $null
try { $parentMajorId = ($parentMajor.body | ConvertFrom-Json).data.id } catch {}
$newClass = @{
    code = "TCLASS-$uniq"; name = "Test Class"
    isActive = $true; grade = "2024"
    majorId = $parentMajorId; departmentId = $deptId
}
$r = Post-Json "/api/v1/classes" $newClass $h
Expect $r 200 "classes create"
$clid = $null
try { $clid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($clid) {
    Expect (Get-Json "/api/v1/classes/$clid" $h) 200 "classes get by id"
    Expect (Put-Json "/api/v1/classes/$clid" @{ name = "Test Class 2" } $h) 200 "classes update"
    Expect (Delete-Json "/api/v1/classes/$clid" $h) 200 "classes delete"
}
if ($parentMajorId) { Delete-Json "/api/v1/majors/$parentMajorId" $h | Out-Null }

# --- TeachingTasksController ---
L "--- TeachingTasksController (api/v1/teaching-tasks) ---"
Expect (Get-Json "/api/v1/teaching-tasks" $h) 200 "teaching-tasks list"

# --- PeriodTimesController ---
L "--- PeriodTimesController (api/v1/period-times) ---"
Expect (Get-Json "/api/v1/period-times" $h) 200 "period-times list"

L "=== Agent #2 DONE ==="
