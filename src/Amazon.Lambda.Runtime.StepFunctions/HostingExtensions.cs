using Amazon.Lambda.Services;
using Amazon.StepFunctions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with ElastiCache.</summary>
  public static class HostingExtensions
  {
    /// <summary>Adds StepFunction support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddStepFunctions(this IServiceCollection services)
    {
      return services.AddSingleton<IAmazonStepFunctions>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonStepFunctionsConfig();

        if (options.RedirectTable.Contains(WellKnownService.StepFunctions))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.StepFunctions].ToString();
        }

        if (options.ProxyTable.Contains(WellKnownService.StepFunctions))
        {
          var uri = options.ProxyTable[WellKnownService.StepFunctions];

          config.ProxyHost = uri.Host;
          config.ProxyPort = uri.Port;
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonStepFunctionsClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonStepFunctionsClient(config);
      });
    }
  }
}