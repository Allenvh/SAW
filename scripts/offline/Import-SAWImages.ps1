param([string]$InDir = "./artifacts")
$manifest = Get-Content "$InDir/manifest.json" | ConvertFrom-Json
$hash = (Get-FileHash "$InDir/saw-browser.tar" -Algorithm SHA256).Hash
if($hash -ne $manifest.Hash){ throw "Hash mismatch" }
docker load -i "$InDir/saw-browser.tar"
docker run --rm saw-browser:latest /bin/bash -lc "echo smoke"
