param([switch]$Simulate)
if($Simulate){ Write-Host "Simulation mode"; return }
New-Item -ItemType Directory -Force -Path 'C:\SAW' | Out-Null
if(-not [System.Diagnostics.EventLog]::SourceExists('SAW')){ [System.Diagnostics.EventLog]::CreateEventSource('SAW','Application') }
Write-Host 'Validate Docker and firewall configuration manually per policy.'
