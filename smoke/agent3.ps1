. "E:\lims-auth\smoke.ps1"
$out = "E:\lims-auth\smoke\agent3.log"
"" | Out-File $out -Encoding utf8
$global:SmokeLog = $out
$token = Load-Token
$h = Auth-H $token
$uniq = [Guid]::NewGuid().ToString().Substring(0,8)
function L($m) { $m | Out-File $out -Append -Encoding utf8; Write-Host $m }

L "=== Agent #3: Venue (Campuses/Buildings/Labs/Equipments) ==="
L "Timestamp: $(Get-Date -Format o)"

# --- CampusesController ---
L "--- CampusesController (api/v1/campuses) ---"
Expect (Get-Json "/api/v1/campuses" $h) 200 "campuses list"
$newCamp = @{ code = "TCAMP-$uniq"; name = "Test Campus"; isActive = $true }
$r = Post-Json "/api/v1/campuses" $newCamp $h
Expect $r 200 "campuses create"
$camid = $null
try { $camid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($camid) {
    Expect (Get-Json "/api/v1/campuses/$camid" $h) 200 "campuses get by id"
    Expect (Put-Json "/api/v1/campuses/$camid" @{ name = "Test Campus 2" } $h) 200 "campuses update"
    Expect (Patch-Json "/api/v1/campuses/$camid/status" @{ isActive = $false } $h) 200 "campuses patch status"
}

# --- BuildingsController ---
L "--- BuildingsController (api/v1/buildings) ---"
Expect (Get-Json "/api/v1/buildings" $h) 200 "buildings list"
if ($camid) {
    Expect (Get-Json "/api/v1/buildings/by-campus/$camid" $h) 200 "buildings by-campus"
}
$newBld = @{ code = "TBLD-$uniq"; name = "Test Building"; isActive = $true; campusId = $camid }
$r = Post-Json "/api/v1/buildings" $newBld $h
Expect $r 200 "buildings create"
$bid = $null
try { $bid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($bid) {
    Expect (Get-Json "/api/v1/buildings/$bid" $h) 200 "buildings get by id"
    Expect (Put-Json "/api/v1/buildings/$bid" @{ name = "Test Building 2" } $h) 200 "buildings update"
    Expect (Patch-Json "/api/v1/buildings/$bid/status" @{ isActive = $false } $h) 200 "buildings patch status"
}

# --- LabsController ---
L "--- LabsController (api/v1/labs) ---"
Expect (Get-Json "/api/v1/labs" $h) 200 "labs list"
$newLab = @{
    code = "TLAB-$uniq"; name = "Test Lab"; labType = "physics"
    isActive = $true; buildingId = $bid; capacity = 30
}
$r = Post-Json "/api/v1/labs" $newLab $h
Expect $r 200 "labs create"
$lid = $null
try { $lid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($lid) {
    Expect (Get-Json "/api/v1/labs/$lid" $h) 200 "labs get by id"
    Expect (Put-Json "/api/v1/labs/$lid" @{ name = "Test Lab 2" } $h) 200 "labs update"
    Expect (Patch-Json "/api/v1/labs/$lid/status" @{ isActive = $false } $h) 200 "labs toggle status"
    Expect (Delete-Json "/api/v1/labs/$lid" $h) 200 "labs delete"
}

# --- EquipmentsController ---
L "--- EquipmentsController (api/v1/equipments) ---"
Expect (Get-Json "/api/v1/equipments" $h) 200 "equipments list"
# Equipment.LabId is optional; pass empty to avoid FK error since lab was already deleted
$newEq = @{
    code = "TEQ-$uniq"; name = "Test Equipment"
    isActive = $true
}
$r = Post-Json "/api/v1/equipments" $newEq $h
Expect $r 200 "equipments create"
$eid = $null
try { $eid = ($r.body | ConvertFrom-Json).data.id } catch {}
if ($eid) {
    Expect (Get-Json "/api/v1/equipments/$eid" $h) 200 "equipments get by id"
    Expect (Put-Json "/api/v1/equipments/$eid" @{ name = "Test Eq 2" } $h) 200 "equipments update"
    Expect (Delete-Json "/api/v1/equipments/$eid" $h) 200 "equipments delete"
}

# Cleanup
if ($bid) { Expect (Delete-Json "/api/v1/buildings/$bid" $h) 200 "buildings delete" }
if ($camid) { Expect (Delete-Json "/api/v1/campuses/$camid" $h) 200 "campuses delete" }

L "=== Agent #3 DONE ==="
