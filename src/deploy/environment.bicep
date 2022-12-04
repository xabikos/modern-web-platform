// targetScope = 'subscription'

// @description('Name of the resourceGroup to create')
// param rgName string
// @description('Location for the resourceGroup')
// param rgLocation string


// resource test_group 'Microsoft.Resources/resourceGroups@2021-04-01' = {
//   name: rgName
//   location: rgLocation
// }

@description('Describes plan\'s pricing tier and instance size. Check details at https://azure.microsoft.com/en-us/pricing/details/app-service/')
@allowed([
  'F1'
  'D1'
  'B1'
  'B2'
  'B3'
  'S1'
  'S2'
  'S3'
  'P1'
  'P2'
  'P3'
  'P4'
])
param skuName string = 'B1'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The environment the deployment happens')
@allowed(['dev', 'prod'])
param environment string

@description('The admin user of the SQL Server')
param sqlAdministratorLogin string
@description('The password of the admin user of the SQL Server')
@secure()
param sqlAdministratorPassword string

@description('The key to access the SendGrid account')
param sendGridKey string

@description('The sid of the Twilio account')
param twilioAccountSid string
@description('The token for authentication against Twilio')
param twilioAuthToken string
@description('The phone number the SMS is send from')
param twilioFromNumber string

// SQL Server and databases region

var sqlserverName = 'sqlServer-${environment}-xab'

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: sqlserverName
  location: location
  properties: {
    version: '12.0'
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
    publicNetworkAccess: 'Enabled'
  }
}

resource allowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

var databaseName = 'identity'
resource identitySqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'GP_S_Gen5'
    capacity: 2
    tier: 'GeneralPurpose'
    family: 'Gen5'
  }
}

// Log Analytics workspace and AppInsights
var logAnalyticsWorkspaceName = 'loganalytis-${environment}-xab'
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 90
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

var appInsightsName = 'appInsights-${environment}-xab'
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

// App service plans and App Services region

resource servicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'xab-plan'
  location: location
  kind: 'linux'
  sku: {
    name: skuName
    tier: 'Basic'
    size: 'B1'
    capacity: 1
  }
}

var identitySiteName = 'identity-xab-${environment}'
var coreApiSiteName = 'api-cellarrtas-${environment}'
var websiteName = 'web-xab-${environment}'

var commonAppSettings = [
  { name: 'SendGridKey', value: sendGridKey }
  { name: 'Services__Identity__Url', value: '${identitySiteName}.azurewebsites.net' }
  { name: 'Services__CoreDomainAPI__Url', value: '${coreApiSiteName}.azurewebsites.net' }
  { name: 'Services__PublicWebApp__PostLogoutRedirectUris', value: '${websiteName}.azurewebsites.net/signout-callback-oidc' }
  { name: 'Services__PublicWebApp__RedirectURI', value: '${websiteName}.azurewebsites.net/signin-oidc' }
  { name: 'Services__PublicWebApp__Secret', value: 'random-secret${environment}' }
  { name: 'Services__PublicWebApp__Url', value: '${websiteName}.azurewebsites.net' }
  { name: 'Services__Twilio__AccountSid', value: twilioAccountSid }
  { name: 'Services__Twilio__AuthToken', value: twilioAuthToken }
  { name: 'Services__Twilio__FromNumber', value: twilioFromNumber }
]

resource identitySite 'Microsoft.Web/sites@2022-03-01' = {
  name: identitySiteName
  location: location
  properties: {
    serverFarmId: servicePlan.id
    siteConfig: {
      http20Enabled: true
      appSettings: commonAppSettings
    }
    httpsOnly: true
  }
}

resource identityConnectionString 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: identitySite
  name: 'connectionstrings'
  kind: 'string'
  properties: {
    DefaultConnection: {
      value: 'Server=${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};User Id=${sqlAdministratorLogin}@${sqlServer.properties.fullyQualifiedDomainName};Password=${sqlAdministratorPassword};'
      type: 'SQLAzure'
    }
  }
}

