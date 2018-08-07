using Amazon.Lambda.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with Lambda.</summary>
  public static class HostingExtensions
  {
    /// <summary>Adds Lambda support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddLambda(this IServiceCollection services)
    {
      return services.AddSingleton<IAmazonLambda>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonLambdaConfig();

        if (options.RedirectTable.Contains(WellKnownService.Lambda))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.Lambda].ToString();
        }

        if (options.ProxyTable.Contains(WellKnownService.Lambda))
        {
          var uri = options.ProxyTable[WellKnownService.Lambda];

          config.ProxyHost = uri.Host;
          config.ProxyPort = uri.Port;
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonLambdaClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonLambdaClient(config);
      });
    }
  }
}