param(
    [Parameter(Mandatory = $true)][string]$Branch,
    [Parameter(Mandatory = $true)][string]$Environment,
    [string]$ResourceGroup = "rg-platform-banking-$Environment"
)

$ErrorActionPreference = 'Stop'

Write-Host "Preparing deployment for branch '$Branch' to '$Environment'"
Write-Host "Resource group: $ResourceGroup"
Write-Host 'Scaffold mode: add az deployment group create command when environment wiring is ready.'
