targetScope = 'resourceGroup'

@description('Deployment location')
param location string = resourceGroup().location

@description('Feature branch name for scoped deployment naming')
param branchName string

output deploymentMessage string = 'Phase 0 trust foundation scaffold deployed.'
output normalizedBranch string = toLower(replace(branchName, '/', '-'))
