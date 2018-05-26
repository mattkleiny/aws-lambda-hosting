using Amazon.ElastiCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with ElastiCache.</summary>
  public static class ElastiCacheHostingExtensions
  {
    /// <summary>Adds ElastiCache support to the host.</summary>
    public static IHostBuilder UseElastiCache(this IHostBuilder builder)
    {
      return builder.ConfigureServices((context, services) =>
      {
        services.AddScoped<IAmazonElastiCache>(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonElastiCacheConfig();

          if (options.AWS.DefaultEndpoint != null) config.RegionEndpoint       = options.AWS.DefaultEndpoint;
          if (options.RedirectTable.Contains("elasticache")) config.ServiceURL = options.RedirectTable["elasticache"].ToString();

          if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
          {
            return new AmazonElastiCacheClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
          }

          return new AmazonElastiCacheClient(config);
        });
      });
    }
  }
}