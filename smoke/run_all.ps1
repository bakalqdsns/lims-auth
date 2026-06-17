# Launch all 6 smoke agents in parallel
$procs = @()
for ($i = 1; $i -le 6; $i++) {
    $script = "E:\lims-auth\smoke\agent$i.ps1"
    Write-Host "Launching agent$i..."
    $p = Start-Process -FilePath "powershell.exe" `
        -ArgumentList "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", "`"$script`"" `
        -PassThru -WindowStyle Hidden
    $procs += $p
}

# Wait for all to finish (max 120s each)
Write-Host "Waiting for all agents to complete..."
foreach ($p in $procs) {
    $p.WaitForExit(120000) | Out-Null
    Write-Host ("Agent PID {0} exit code: {1}" -f $p.Id, $p.ExitCode)
}

Write-Host "All agents finished."
