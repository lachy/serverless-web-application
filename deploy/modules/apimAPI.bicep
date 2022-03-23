param apimName string
param currentResourceGroup string
param backendApiName string
param apiName string
param originUrl string
param apiSecret string

var functionAppKeyName = '${backendApiName}-key'

resource backendApiApp 'Microsoft.Web/sites@2021-01-15' existing = {
  name: backendApiName
  scope: resourceGroup(currentResourceGroup)
}

resource functionKey 'Microsoft.Web/sites/functions/keys@2021-01-15' existing = {
  name: '${backendApiName}/GetTodoItems/default'
  scope: resourceGroup(currentResourceGroup)
}

resource apim 'Microsoft.ApiManagement/service@2021-01-01-preview' existing = {
  name: apimName
}

resource namedValues 'Microsoft.ApiManagement/service/namedValues@2021-01-01-preview' = {
  parent: apim
  name: functionAppKeyName
  properties: {
    displayName: functionAppKeyName
    value: listKeys('${backendApiApp.id}/host/default','2019-08-01').functionKeys.default
  }
}

resource backendApi 'Microsoft.ApiManagement/service/backends@2021-01-01-preview' = {
  parent: apim
  name: backendApiName
  properties: {
    description: backendApiName
    resourceId: 'https://management.azure.com${backendApiApp.id}'
    credentials: {
      header:{
        'x-functions-key': [
          '{{${namedValues.properties.displayName}}}'
        ]
      }
    }
    url: 'https://${backendApiApp.properties.hostNames[0]}/api'
    protocol: 'http'
  }
}

resource authorizationServer 'Microsoft.ApiManagement/service/authorizationServers@2021-04-01-preview' = {
  name: 'auth0'
  parent: apim
  properties: {
    authorizationEndpoint: 'https://dev-f-rourrc.us.auth0.com/authorize'
    authorizationMethods: [
      'GET'
      'POST'
    ]
    bearerTokenSendingMethods: [
      'authorizationHeader'
    ]
    clientAuthenticationMethod: [
      'Body'
      'Basic'
    ]
    clientId: 'pn5l2sKfRWV3bRaS0creZ6uZLyBEMcow'
    clientRegistrationEndpoint: 'https://serverless-dev.azureedge.net'
    clientSecret: apiSecret
    description: 'auth0 identity provider'
    displayName: 'auth0'
    grantTypes: [
      'authorizationCode'
    ]
    tokenBodyParameters: []
    tokenEndpoint: 'https://dev-f-rourrc.us.auth0.com/oauth/token'
  }
}

resource api 'Microsoft.ApiManagement/service/apis@2021-01-01-preview' = {
  parent: apim
  name: apiName
  properties: {
    path: apiName
    displayName: apiName
    isCurrent: true
    subscriptionRequired: false
    protocols: [
      'https'
    ]
    authenticationSettings: {
      oAuth2: {
        authorizationServerId: 'auth0'
      }
    }
  }
}

resource apiPolicy 'Microsoft.ApiManagement/service/apis/policies@2021-01-01-preview' = {
  parent: api
  name: 'policy'
  properties: {
    format: 'rawxml'
    value: replace(loadTextContent('../content/cos-policy.xml'),'__ORIGIN__',originUrl)
  }
}
