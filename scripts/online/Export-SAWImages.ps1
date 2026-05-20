param([string]$OutDir = "./artifacts")
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
docker build -t saw-browser:latest ./docker/browser-image
docker save saw-browser:latest -o "$OutDir/saw-browser.tar"
Get-FileHash "$OutDir/saw-browser.tar" -Algorithm SHA256 | ConvertTo-Json | Set-Content "$OutDir/manifest.json"
