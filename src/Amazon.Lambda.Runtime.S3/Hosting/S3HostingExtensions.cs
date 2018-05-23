using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with S3.</summary>
  public static class DynamoHostingExtensions
  {
    /// <summary>Adds <see cref="AmazonS3Client"/> to the host.</summary>
    public static LambdaHostBuilder UseS3(this LambdaHostBuilder builder)
    {
      builder.ConfigureServices((context, services) =>
      {
        services.AddScoped(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonS3Config();

          if (options.RedirectTable.Contains("s3"))
          {
            config.ServiceURL = options.RedirectTable["s3"].ToString();
          }

          return new AmazonS3Client(config);
        });
      });

      return builder;
    }
  }
}