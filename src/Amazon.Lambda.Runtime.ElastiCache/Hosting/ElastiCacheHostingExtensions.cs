using Amazon.ElastiCache;
using Amazon.Lambda.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with ElastiCache.</summary>
  public static class ElastiCacheHostingExtensions
  {
    /// <summary>Adds ElastiCache support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection UseElastiCache(this IServiceCollection services)
    {
      return services.AddSingleton<IAmazonElastiCache>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonElastiCacheConfig();

        if (options.RedirectTable.Contains(WellKnownService.ElastiCache))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.ElastiCache].ToString();
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonElastiCacheClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonElastiCacheClient(config);
      });
    }
  }
}