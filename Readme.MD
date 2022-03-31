# Getting started

## Register App

```powershell
$app = Register-PnPAzureADApp `
    -ApplicationName  "PoC.SiteCollectionCreationPermissions" `
    -Tenant <YourTenant>.onmicrosoft.com `
    -CertificatePassword (ConvertTo-SecureString -String "<YourPassword>" -AsPlainText -Force) `
    -OutPath "PoC.SiteCollectionCreationPermissions.pfx" `
    -GraphApplicationPermissions User.Read.All `
    -SharePointApplicationPermissions User.Read.All `
    -DeviceLogin
```

## Add User Secrets

```sh
$ dotnet user-secrets set clientconfig:base64 $app.Base64Encoded
$ dotnet user-secrets set clientconfig:password <YourPassword>
```

## Adjust appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ClientConfig": {
    "TenantId": "<TenantId>",
    "ClientId": "<ClientId>",
    "SiteUrl": "https://<tenant>.sharepoint.com/sites/<whatever>",
    "BaseUrl": "https://<tenant>.sharepoint.com/",
    "Owner": "<user>@<tenant>.onmicrosoft.com"
  }
}
```
 