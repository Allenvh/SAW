param([switch]$KinitTest)
Write-Host 'Checking DNS for target.local'; Resolve-DnsName target.local -ErrorAction SilentlyContinue
Write-Host 'Checking Docker'; docker version
Write-Host 'Checking image'; docker image inspect saw-browser:latest
if($KinitTest){ Write-Host 'Run kinit test in secured process' }
