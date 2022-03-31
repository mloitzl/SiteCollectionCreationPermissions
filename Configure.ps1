$app = Register-PnPAzureADApp `
    -ApplicationName  "PoC.SiteCollectionCreationPermissions" `
    -Tenant <YourTenant>.onmicrosoft.com `
    -CertificatePassword (ConvertTo-SecureString -String "<YourPassword>" -AsPlainText -Force) `
    -OutPath "PoC.SiteCollectionCreationPermissions.pfx" `
    -GraphApplicationPermissions User.Read.All `
    -SharePointApplicationPermissions User.Read.All `
    -DeviceLogin

# dotnet user-secrets set clientconfig:base64 $app.Base64Encoded
# dotnet user-secrets set clientconfig:password <YourPassword>
# 