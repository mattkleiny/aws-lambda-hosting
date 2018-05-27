using Amazon.Lambda.Services;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with ElastiCache.</summary>
  public static class SQSHostingExtensions
  {
    /// <summary>Adds SQS support to the host.</summary>
    public static IHostBuilder UseSQS(this IHostBuilder builder)
    {
      return builder.ConfigureServices((context, services) =>
      {
        services.AddScoped<IAmazonSQS>(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonSQSConfig();

          if (options.RedirectTable.Contains(WellKnownService.SQS))
          {
            config.ServiceURL = options.RedirectTable[WellKnownService.SQS].ToString();
          }

          if (options.AWS.DefaultEndpoint != null)
          {
            config.RegionEndpoint = options.AWS.DefaultEndpoint;
          }

          if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
          {
            return new AmazonSQSClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
          }

          return new AmazonSQSClient(config);
        });
      });
    }
  }
}