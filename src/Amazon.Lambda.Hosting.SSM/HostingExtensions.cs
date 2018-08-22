using System;
using Amazon.Lambda.Hosting.Configuration;
using Amazon.Lambda.Services;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Extensions for simplifying interaction with SSM.</summary>
  public static class HostingExtensions
  {
    /// <summary>Adds SSM support to the <see cref="IServiceCollection"/>.</summary>
    public static IServiceCollection AddSSM(this IServiceCollection services)
    {
      return services.AddScoped<IAmazonSimpleSystemsManagement>(provider =>
      {
        var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
        var config  = new AmazonSimpleSystemsManagementConfig();

        if (options.RedirectTable.Contains(WellKnownService.SSM))
        {
          config.ServiceURL = options.RedirectTable[WellKnownService.SSM].ToString();
        }

        if (options.ProxyTable.Contains(WellKnownService.SSM))
        {
          var uri = options.ProxyTable[WellKnownService.SSM];

          config.ProxyHost = uri.Host;
          config.ProxyPort = uri.Port;
        }

        if (options.AWS.DefaultEndpoint != null)
        {
          config.RegionEndpoint = options.AWS.DefaultEndpoint;
        }

        if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
        {
          return new AmazonSimpleSystemsManagementClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
        }

        return new AmazonSimpleSystemsManagementClient(config);
      });
    }

    /// <summary>Adds the SSM parameter store as a configuration source to the given <see cref="IConfigurationBuilder"/>.</summary>
    public static IConfigurationBuilder AddParameterStore(
      this IConfigurationBuilder builder,
      string basePath,
      bool optional = false,
      TimeSpan? reloadAfter = null,
      Action<Exception> onException = null,
      RegionEndpoint endpoint = null
    )
    {
      var source = new ParameterStoreConfigurationSource(basePath, optional, reloadAfter, endpoint ?? AWSConfigs.RegionEndpoint);

      if (onException != null)
      {
        source.ExceptionObserved += onException;
      }

      builder.Add(source);

      return builder;
    }
  }
}