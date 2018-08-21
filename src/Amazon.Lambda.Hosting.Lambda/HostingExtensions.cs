using Amazon.Lambda.Hosting.Embedding;
using Amazon.Lambda.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with Lambda.</summary>
  public static class LambdaHostingExtensions
  {
    /// <summary>Adds Lambda support to the <see cref="IServiceCollection"/> with optional local embedding, which permits call-backs into the local lambda host.</summary>
    public static IServiceCollection AddLambda(this IServiceCollection services, bool enableEmbedding = false)
    {
      return services.AddSingleton<IAmazonLambda>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var client  = BuildLambdaClient(options);

        if (enableEmbedding)
        {
          return new EmbeddedAmazonLambda(client, provider);
        }

        return client;
      });
    }

    /// <summary>Builds the default <see cref="IAmazonLambda"/> implementation.</summary>
    private static AmazonLambdaClient BuildLambdaClient(HostingOptions options)
    {
      var config = new AmazonLambdaConfig();

      if (options.RedirectTable.Contains(WellKnownService.Lambda))
      {
        config.ServiceURL = options.RedirectTable[WellKnownService.Lambda].ToString();
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
    }
  }
}