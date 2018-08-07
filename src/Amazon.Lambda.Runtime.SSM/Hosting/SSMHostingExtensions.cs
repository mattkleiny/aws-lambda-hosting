using Amazon.Lambda.Services;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with SSM.</summary>
  public static class SQSHostingExtensions
  {
    /// <summary>Adds SSM support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddSSM(this IServiceCollection services)
    {
      return services.AddSingleton<IAmazonSimpleSystemsManagement>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonSimpleSystemsManagementConfig();

        if (options.RedirectTable.Contains(WellKnownService.SSM))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.SSM].ToString();
        }

        if (options.ProxyTable.Contains(WellKnownService.SSM))
        {
          var uri = options.ProxyTable[WellKnownService.SSM];

          config.ProxyHost = uri.Host;
          config.ProxyPort = uri.Port;
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonSimpleSystemsManagementClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonSimpleSystemsManagementClient(config);
      });
    }
  }
}