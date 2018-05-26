using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with S3.</summary>
  public static class S3HostingExtensions
  {
    /// <summary>Adds S3 support to the host.</summary>
    public static IHostBuilder UseS3(this IHostBuilder builder)
    {
      return builder.ConfigureServices((context, services) =>
      {
        services.AddScoped<IAmazonS3>(provider =>
        {
          var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
          var config  = new AmazonS3Config();

          if (options.AWS.DefaultEndpoint != null) config.RegionEndpoint  = options.AWS.DefaultEndpoint;
          if (options.RedirectTable.Contains("s3")) config.ServiceURL = options.RedirectTable["s3"].ToString();
          
          if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
          {
            return new AmazonS3Client(options.AWS.AccessKey, options.AWS.SecretKey, config);
          }

          return new AmazonS3Client(config);
        });
      });
    }
  }
}