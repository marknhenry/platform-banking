@description('Deployment location')
param location string

@description('Branch token used in resource naming')
param branchToken string

@description('Tags to apply to resources')
param tags object = {}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-pb-${branchToken}'
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: 'appi-pb-${branchToken}'
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

output logAnalyticsWorkspaceName string = workspace.name
output appInsightsName string = appInsights.name
