using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with DynamoDB.</summary>
  public static class DynamoHostingExtensions
  {
    /// <summary>Adds DynamoDB support to the host.</summary>
    public static IHostBuilder UseDynamo(this IHostBuilder builder)
    {
      return builder.ConfigureServices((context, services) =>
      {
        services.AddScoped<IAmazonDynamoDB>(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonDynamoDBConfig();

          if (options.AWS.DefaultEndpoint != null) config.RegionEndpoint  = options.AWS.DefaultEndpoint;
          if (options.RedirectTable.Contains("dynamo")) config.ServiceURL = options.RedirectTable["dynamo"].ToString();

          if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
          {
            return new AmazonDynamoDBClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
          }

          return new AmazonDynamoDBClient(config);
        });
      });
    }
  }
}