targetScope = 'resourceGroup'

@description('Deployment location')
param location string = resourceGroup().location

@description('Feature branch name for scoped deployment naming')
param branchName string

@description('Environment name used for tags and resource labels')
param environment string = 'dev'

var branchNormalized = toLower(replace(replace(branchName, '/', '-'), '_', '-'))
var branchToken = substring(replace(branchNormalized, '-', ''), 0, min(12, length(replace(branchNormalized, '-', ''))))
var commonTags = {
	environment: environment
	branch: branchName
	workload: 'platform-banking'
	foundationPhase: 'phase0'
}

module backend './modules/backend.bicep' = {
	name: 'backend-${branchToken}'
	params: {
		location: location
		branchToken: branchToken
		tags: commonTags
	}
}

module frontend './modules/frontend.bicep' = {
	name: 'frontend-${branchToken}'
	params: {
		location: location
		branchToken: branchToken
		tags: commonTags
	}
}

module data './modules/data.bicep' = {
	name: 'data-${branchToken}'
	params: {
		location: location
		branchToken: branchToken
		tags: commonTags
	}
}

module observability './modules/observability.bicep' = {
	name: 'observability-${branchToken}'
	params: {
		location: location
		branchToken: branchToken
		tags: commonTags
	}
}

output deploymentMessage string = 'Phase 0 trust foundation baseline resources deployed.'
output normalizedBranch string = branchNormalized
output backendAppName string = backend.outputs.backendContainerAppName
output backendAppUrl string = backend.outputs.backendContainerAppUrl
output frontendAppName string = frontend.outputs.staticWebAppName
output frontendHostName string = frontend.outputs.staticWebAppDefaultHostName
output sqlServerName string = data.outputs.sqlServerName
output sqlDatabaseName string = data.outputs.sqlDatabaseName
output redisName string = data.outputs.redisName
output appInsightsName string = observability.outputs.appInsightsName
output logAnalyticsWorkspaceName string = observability.outputs.logAnalyticsWorkspaceName
