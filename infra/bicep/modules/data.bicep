@description('Deployment location')
param location string

@description('Branch token used in resource naming')
param branchToken string

@description('Tags to apply to resources')
param tags object = {}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: 'sqlpb${branchToken}'
  location: location
  tags: tags
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: 'platform-banking-admin'
      sid: '11111111-1111-1111-1111-111111111111'
      tenantId: subscription().tenantId
    }
    publicNetworkAccess: 'Disabled'
    minimalTlsVersion: '1.2'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  name: 'platform-banking'
  parent: sqlServer
  location: location
  sku: {
    name: 'GP_S_Gen5_1'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
}

resource redisCache 'Microsoft.Cache/redis@2024-03-01' = {
  name: 'redis-pb-${branchToken}'
  location: location
  tags: tags
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Disabled'
    redisConfiguration: {
      'maxmemory-policy': 'volatile-lru'
    }
  }
  sku: {
    name: 'Basic'
    family: 'C'
    capacity: 0
  }
}

output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name
output redisName string = redisCache.name
