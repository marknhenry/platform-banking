param(
    [Parameter(Mandatory = $true)][string]$Branch,
    [Parameter(Mandatory = $true)][string]$Environment,
    [string]$ResourceGroup,
    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$envFile = Join-Path $repoRoot "infra\env\$Environment.json"

if (-not (Test-Path $envFile)) {
    throw "Environment configuration file not found: $envFile"
}

$envConfig = Get-Content $envFile -Raw | ConvertFrom-Json

if (-not $ResourceGroup) {
    $ResourceGroup = [string]$envConfig.resourceGroup
}

$location = [string]$envConfig.location
$templateFile = Join-Path $repoRoot 'infra\bicep\main.bicep'

Write-Host "Preparing deployment for branch '$Branch' to '$Environment'"
Write-Host "Resource group: $ResourceGroup"
Write-Host "Location: $location"
Write-Host "Template: $templateFile"

$deploymentName = "phase0-$Environment-$(Get-Date -Format 'yyyyMMddHHmmss')"
$baseCommand = @(
    'deployment', 'group', 'create',
    '--name', $deploymentName,
    '--resource-group', $ResourceGroup,
    '--template-file', $templateFile,
    '--parameters', "branchName=$Branch", "environment=$Environment", "location=$location"
)

if ($WhatIf) {
    $whatIfCommand = @('deployment', 'group', 'what-if') + $baseCommand[4..($baseCommand.Length - 1)]
    & az @whatIfCommand
}
else {
    & az @baseCommand
}
