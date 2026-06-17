. "E:\lims-auth\smoke.ps1"
$out = "E:\lims-auth\smoke\agent1.log"
"" | Out-File $out -Encoding utf8
$global:SmokeLog = $out
$token = Load-Token
$h = Auth-H $token
$uniq = [Guid]::NewGuid().ToString().Substring(0,8)
function L($m) { $m | Out-File $out -Append -Encoding utf8; Write-Host $m }

L "=== Agent #1: Auth / Users / Roles / Permissions / Departments ==="
L "Timestamp: $(Get-Date -Format o)"

# --- AuthController ---
L "--- AuthController (api/v1/auth) ---"
Expect (Get-Json "/api/v1/auth/health") 200 "auth/health"
Expect (Post-Json "/api/v1/auth/login" @{ username = "admin"; password = "Admin@123" }) 200 "auth/login admin"
Expect (Get-Json "/api/v1/auth/me" $h) 200 "auth/me"
Expect (Post-Json "/api/v1/auth/refresh" "{}" $h) 200 "auth/refresh"

# --- UsersController ---
L "--- UsersController (api/v1/users) ---"
Expect (Get-Json "/api/v1/users" $h) 200 "users list"
Expect (Get-Json "/api/v1/users/00000000-0000-0000-0000-000000000000" $h) @(404,400) "users get unknown"
Expect (Get-Json "/api/v1/users/permissions/my" $h) @(200,404) "users permissions/my"

# --- RolesController ---
L "--- RolesController (api/v1/roles) ---"
Expect (Get-Json "/api/v1/roles" $h) 200 "roles list"
Expect (Get-Json "/api/v1/roles/all" $h) 200 "roles/all"

# --- PermissionsController ---
L "--- PermissionsController (api/v1/permissions) ---"
Expect (Get-Json "/api/v1/permissions" $h) 200 "permissions list"
Expect (Get-Json "/api/v1/permissions/by-module" $h) @(200,404) "permissions/by-module"
Expect (Get-Json "/api/v1/permissions/modules" $h) @(200,404) "permissions/modules"

# --- DepartmentsController ---
L "--- DepartmentsController (api/v1/departments) ---"
Expect (Get-Json "/api/v1/departments" $h) 200 "departments list"
Expect (Get-Json "/api/v1/departments/all" $h) 200 "departments/all"

L "=== Agent #1 DONE ==="
