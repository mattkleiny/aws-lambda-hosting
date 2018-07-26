using Amazon.Lambda.Services;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with S3.</summary>
  public static class S3HostingExtensions
  {
    /// <summary>Adds S3 support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddS3(this IServiceCollection services)
    {
      return services.AddSingleton<IAmazonS3>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonS3Config();

        if (options.RedirectTable.Contains(WellKnownService.S3))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.S3].ToString();
        }

        if (options.ProxyTable.Contains(WellKnownService.S3))
        {
          var uri = options.ProxyTable[WellKnownService.S3];

          config.ProxyHost = uri.Host;
          config.ProxyPort = uri.Port;
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonS3Client(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonS3Client(config);
      });
    }
  }
}