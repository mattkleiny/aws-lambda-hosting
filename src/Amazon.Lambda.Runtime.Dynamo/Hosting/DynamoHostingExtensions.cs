using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with DynamoDB.</summary>
  public static class DynamoHostingExtensions
  {
    /// <summary>Adds <see cref="AmazonDynamoDBClient"/> to the host.</summary>
    public static IHostBuilder UseDynamo(this IHostBuilder builder)
    {
      return builder.ConfigureServices((context, services) =>
      {
        services.AddScoped(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonDynamoDBConfig();

          if (options.DefaultEndpoint != null) config.RegionEndpoint      = options.DefaultEndpoint;
          if (options.RedirectTable.Contains("dynamo")) config.ServiceURL = options.RedirectTable["dynamo"].ToString();

          if (!string.IsNullOrEmpty(options.AccessKey) || !string.IsNullOrEmpty(options.SecretKey))
          {
            return new AmazonDynamoDBClient(options.AccessKey, options.SecretKey, config);
          }

          return new AmazonDynamoDBClient(config);
        });
      });
    }
  }
}