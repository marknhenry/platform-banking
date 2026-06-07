@description('Deployment location')
param location string

@description('Branch token used in resource naming')
param branchToken string

@description('Tags to apply to resources')
param tags object = {}

resource staticSite 'Microsoft.Web/staticSites@2023-12-01' = {
  name: 'swa-pb-${branchToken}'
  location: location
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    stagingEnvironmentPolicy: 'Enabled'
    publicNetworkAccess: 'Enabled'
  }
}

output staticWebAppName string = staticSite.name
output staticWebAppDefaultHostName string = staticSite.properties.defaultHostname
