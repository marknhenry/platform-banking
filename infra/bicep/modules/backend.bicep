@description('Deployment location')
param location string

@description('Branch token used in resource naming')
param branchToken string

@description('Tags to apply to resources')
param tags object = {}

resource managedEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'cae-pb-${branchToken}'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'none'
    }
    zoneRedundant: false
  }
}

resource backendApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'ca-pb-api-${branchToken}'
  location: location
  tags: tags
  properties: {
    managedEnvironmentId: managedEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
      }
      activeRevisionsMode: 'Single'
    }
    template: {
      containers: [
        {
          name: 'api'
          image: 'mcr.microsoft.com/dotnet/aspnet:10.0'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
}

output backendContainerAppName string = backendApp.name
output backendContainerAppUrl string = 'https://${backendApp.properties.configuration.ingress.fqdn}'
