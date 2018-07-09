using Amazon.Lambda.Services;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with ElastiCache.</summary>
  public static class SNSHostingExtensions
  {
    /// <summary>Adds SNS support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddSNS(this IServiceCollection services)
    {
      return services.AddSingleton<IAmazonSimpleNotificationService>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonSimpleNotificationServiceConfig();

        if (options.RedirectTable.Contains(WellKnownService.SNS))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.SNS].ToString();
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonSimpleNotificationServiceClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonSimpleNotificationServiceClient(config);
      });
    }
  }
}