using System;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A strongly-typed startup interface for the <see cref="HostingExtensions.UseStartup{TStartup}"/> method.</summary>
  public interface IStartup
  {
    /// <summary>Configures the application services.</summary>
    void ConfigureServices(IServiceCollection services);

    /// <summary>Configures the application after the services have been installed.</summary>
    void Configure(IServiceProvider services);
  }
}