using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with S3.</summary>
  public static class S3HostingExtensions
  {
    /// <summary>Adds <see cref="AmazonS3Client"/> to the host.</summary>
    public static IHostBuilder UseS3(this IHostBuilder builder)
    {
      return builder.ConfigureServices((context, services) =>
      {
        services.AddScoped(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonS3Config();

          if (options.DefaultEndpoint != null) config.RegionEndpoint  = options.DefaultEndpoint;
          if (options.RedirectTable.Contains("s3")) config.ServiceURL = options.RedirectTable["s3"].ToString();
          
          if (!string.IsNullOrEmpty(options.AccessKey) || !string.IsNullOrEmpty(options.SecretKey))
          {
            return new AmazonS3Client(options.AccessKey, options.SecretKey, config);
          }

          return new AmazonS3Client(config);
        });
      });
    }
  }
}