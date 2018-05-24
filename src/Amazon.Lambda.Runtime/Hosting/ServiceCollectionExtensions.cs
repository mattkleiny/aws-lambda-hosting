using System;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Lambda-specific extensions for <see cref="IServiceCollection"/> configuration.</summary>
  public static class ServiceCollectionExtensions
  {
    /// <summary>Configures the <see cref="HostingOptions"/> for the application.</summary>
    public static IServiceCollection ConfigureHostingOptions(this IServiceCollection services, Action<HostingOptions> configurer)
    {
      Check.NotNull(configurer, nameof(configurer));
      
      services.Configure(configurer);

      return services;
    }
  }
}