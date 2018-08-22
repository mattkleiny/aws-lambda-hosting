using Amazon.DynamoDBv2;
using Amazon.Lambda.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with DynamoDB.</summary>
  public static class HostingExtensions
  {
    /// <summary>Adds DynamoDB support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddDynamo(this IServiceCollection services)
    {
      return services.AddScoped<IAmazonDynamoDB>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonDynamoDBConfig();

        if (options.RedirectTable.Contains(WellKnownService.Dynamo))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.Dynamo].ToString();
        }

        if (options.ProxyTable.Contains(WellKnownService.Dynamo))
        {
          var uri = options.ProxyTable[WellKnownService.Dynamo];

          config.ProxyHost = uri.Host;
          config.ProxyPort = uri.Port;
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonDynamoDBClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonDynamoDBClient(config);
      });
    }
  }
}