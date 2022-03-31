using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PnP.Core.Admin.Model.SharePoint;
using PnP.Core.Auth.Services.Builder.Configuration;
using PnP.Core.Services;
using PnP.Core.Services.Builder.Configuration;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var clientConfig = new ClientConfig();
        context.Configuration.GetSection("ClientConfig").Bind(clientConfig);
        services.AddSingleton(clientConfig);

        var cert =
            new PnPCoreAuthenticationX509CertificateOptions
            {
                Certificate = new X509Certificate2(
                    Convert.FromBase64String(
                        clientConfig.Base64),
                    clientConfig.Password)
            };

        services.AddPnPCore(options =>
        {
            options.Sites.Add("SiteToWorkWith", new PnPCoreSiteOptions
            {
                SiteUrl = clientConfig.SiteUrl
            });
        });

        services.AddPnPCoreAuthentication(
            options =>
            {
                options.Credentials.Configurations.Add("x509certificate",
                    new PnPCoreAuthenticationCredentialConfigurationOptions
                    {
                        ClientId = clientConfig.ClientId,
                        TenantId = clientConfig.TenantId,
                        X509Certificate = cert
                    });

                options.Credentials.DefaultConfiguration = "x509certificate";

                options.Sites.Add("SiteToWorkWith",
                    new PnPCoreAuthenticationSiteOptions
                    {
                        AuthenticationProviderName = "x509certificate"
                    });
            });
    })
    .UseConsoleLifetime()
    .Build();

await host.StartAsync();

using var scope = host.Services.CreateScope();

var clientConfig = scope.ServiceProvider.GetRequiredService<ClientConfig>();

var pnpContextFactory = scope.ServiceProvider.GetRequiredService<IPnPContextFactory>();
using var context = await pnpContextFactory.CreateAsync("SiteToWorkWith");
var adminPortalUri = await context.GetSharePointAdmin().GetTenantAdminCenterUriAsync();

Console.WriteLine(adminPortalUri);
var entropy = Guid.NewGuid().ToString();

var communicationSiteToCreate =
    new CommunicationSiteOptions(
        new Uri($"{clientConfig.BaseUrl}/sites/{entropy.Substring(0, 8)}"),
        $"My communication site {entropy.Substring(0, 8)}")
    {
        Description = $"My site description {entropy}",
        Language = Language.English,
        Owner = clientConfig.Owner
    };

try
{
    using var newSiteContext =
        await context.GetSiteCollectionManager().CreateSiteCollectionAsync(communicationSiteToCreate);
    await newSiteContext.Web.LoadAsync(p => p.Title);
    Console.WriteLine(newSiteContext.Web.Title);
    Console.WriteLine(newSiteContext.Web.Url);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

await context.Web.LoadAsync(p => p.Title);
Console.WriteLine($"The title of the web is {context.Web.Title}");