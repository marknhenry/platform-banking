$ErrorActionPreference = 'Stop'

$contract = Join-Path $PSScriptRoot '..\..\..\specs\001-phase0-trust-foundation\contracts\phase0-api.yaml'
$contract = [System.IO.Path]::GetFullPath($contract)

if (-not (Test-Path $contract)) {
    throw "Contract file not found: $contract"
}

npx -y @apidevtools/swagger-cli validate $contract
Write-Host "OpenAPI contract validation succeeded: $contract"
