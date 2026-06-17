# Shared smoke-test helpers
# Source:  . E:\lims-auth\smoke.ps1

$global:Base = "http://127.0.0.1:5180"

function Get-Json {
    param([string]$Path, [hashtable]$Headers = @{})
    $h = $Headers.Clone()
    if (-not $h.ContainsKey('Accept')) { $h['Accept'] = 'application/json' }
    try {
        $r = Invoke-WebRequest -Method Get -Uri "$global:Base$Path" -Headers $h -UseBasicParsing
        return @{ code = [int]$r.StatusCode; body = $r.Content }
    } catch {
        $ex = $_.Exception.Response
        if ($ex) {
            $stream = $ex.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $body = $reader.ReadToEnd()
            return @{ code = [int]$ex.StatusCode; body = $body }
        }
        return @{ code = -1; body = $_.Exception.Message }
    }
}

function Post-Json {
    param([string]$Path, $Body, [hashtable]$Headers = @{})
    $json = if ($Body -is [string]) { $Body } else { $Body | ConvertTo-Json -Depth 10 -Compress }
    $h = $Headers.Clone()
    $h['Content-Type'] = 'application/json'
    $h['Accept'] = 'application/json'
    try {
        $r = Invoke-WebRequest -Method Post -Uri "$global:Base$Path" -Headers $h -Body $json -UseBasicParsing
        return @{ code = [int]$r.StatusCode; body = $r.Content }
    } catch {
        $ex = $_.Exception.Response
        if ($ex) {
            $stream = $ex.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $body = $reader.ReadToEnd()
            return @{ code = [int]$ex.StatusCode; body = $body }
        }
        return @{ code = -1; body = $_.Exception.Message }
    }
}

function Put-Json {
    param([string]$Path, $Body, [hashtable]$Headers = @{})
    $json = if ($Body -is [string]) { $Body } else { $Body | ConvertTo-Json -Depth 10 -Compress }
    $h = $Headers.Clone()
    $h['Content-Type'] = 'application/json'
    $h['Accept'] = 'application/json'
    try {
        $r = Invoke-WebRequest -Method Put -Uri "$global:Base$Path" -Headers $h -Body $json -UseBasicParsing
        return @{ code = [int]$r.StatusCode; body = $r.Content }
    } catch {
        $ex = $_.Exception.Response
        if ($ex) {
            $stream = $ex.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $body = $reader.ReadToEnd()
            return @{ code = [int]$ex.StatusCode; body = $body }
        }
        return @{ code = -1; body = $_.Exception.Message }
    }
}

function Patch-Json {
    param([string]$Path, $Body, [hashtable]$Headers = @{})
    $json = if ($Body -is [string]) { $Body } else { $Body | ConvertTo-Json -Depth 10 -Compress }
    $h = $Headers.Clone()
    $h['Content-Type'] = 'application/json'
    $h['Accept'] = 'application/json'
    try {
        $r = Invoke-WebRequest -Method Patch -Uri "$global:Base$Path" -Headers $h -Body $json -UseBasicParsing
        return @{ code = [int]$r.StatusCode; body = $r.Content }
    } catch {
        $ex = $_.Exception.Response
        if ($ex) {
            $stream = $ex.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $body = $reader.ReadToEnd()
            return @{ code = [int]$ex.StatusCode; body = $body }
        }
        return @{ code = -1; body = $_.Exception.Message }
    }
}

function Delete-Json {
    param([string]$Path, [hashtable]$Headers = @{})
    $h = $Headers.Clone()
    $h['Accept'] = 'application/json'
    try {
        $r = Invoke-WebRequest -Method Delete -Uri "$global:Base$Path" -Headers $h -UseBasicParsing
        return @{ code = [int]$r.StatusCode; body = $r.Content }
    } catch {
        $ex = $_.Exception.Response
        if ($ex) {
            $stream = $ex.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $body = $reader.ReadToEnd()
            return @{ code = [int]$ex.StatusCode; body = $body }
        }
        return @{ code = -1; body = $_.Exception.Message }
    }
}

function Short-Body {
    param($Body, [int]$Max = 220)
    if ($null -eq $Body) { return "<null>" }
    $s = $Body.ToString()
    if ($s.Length -gt $Max) { return $s.Substring(0, $Max) + "...[trunc]" }
    return $s
}

function Pass($msg) {
    Write-Host "[PASS] $msg"
    if ($global:SmokeLog) { "[PASS] $msg" | Out-File $global:SmokeLog -Append -Encoding utf8 }
}
function Fail($msg) {
    Write-Host "[FAIL] $msg"
    if ($global:SmokeLog) { "[FAIL] $msg" | Out-File $global:SmokeLog -Append -Encoding utf8 }
}
function Info($msg) {
    Write-Host "[INFO] $msg"
    if ($global:SmokeLog) { "[INFO] $msg" | Out-File $global:SmokeLog -Append -Encoding utf8 }
}

function Load-Token {
    param([string]$File = "E:\lims-auth\.admin_token.txt")
    if (-not (Test-Path $File)) { throw "Token file not found: $File" }
    return (Get-Content $File -Raw).Trim()
}

function Login {
    param([string]$User, [string]$Pwd)
    $r = Post-Json "/api/v1/auth/login" @{ username = $User; password = $Pwd }
    if ($r.code -ne 200) { return $null }
    $obj = $r.body | ConvertFrom-Json
    if ($obj.code -ne 200) { return $null }
    return $obj.data.token
}

function Auth-H {
    param([string]$Token)
    return @{ Authorization = "Bearer $Token" }
}

function Expect {
    param($R, [int[]]$Allowed, [string]$Label)
    $ok = $Allowed | Where-Object { $_ -eq $R.code }
    if ($ok) {
        Pass "$Label  HTTP=$($R.code)"
    } else {
        Fail "$Label  HTTP=$($R.code)  Body=$(Short-Body $R.body)"
    }
    return $R
}
