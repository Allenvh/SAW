param([switch]$Cleanup)
Write-Host 'Remove SAW services (placeholder service names).'
if($Cleanup){ Remove-Item 'C:\SAW' -Recurse -Force -ErrorAction SilentlyContinue }
