using System;
using System.Timers;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;

namespace Amazon.Lambda.Hosting.Configuration
{
  /// <summary>A <see cref="IConfigurationSource"/> that talks to <see cref="IAmazonSimpleSystemsManagement"/>.</summary>
  internal sealed class ParameterStoreConfigurationSource : IConfigurationSource, IDisposable
  {
    private readonly RegionEndpoint endpoint;
    private readonly bool           isOptional;
    private readonly Timer          timer;

    public ParameterStoreConfigurationSource(string basePath, bool isOptional, TimeSpan? reloadInterval, RegionEndpoint endpoint)
    {
      this.isOptional = isOptional;
      this.endpoint   = endpoint;

      BasePath = basePath;

      if (reloadInterval.HasValue)
      {
        timer = new Timer(reloadInterval.Value.TotalMilliseconds);

        timer.Elapsed += (sender, args) => ShouldReload?.Invoke();
        timer.Start();
      }
    }

    public string BasePath { get; }

    public event Action<Exception> ExceptionObserved;
    public event Action            ShouldReload;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
      return new ParameterStoreConfigurationProvider(this);
    }

    public void Dispose()
    {
      timer.Dispose();
    }

    /// <summary>Builds a <see cref="IAmazonSimpleSystemsManagement"/> client.</summary>
    internal IAmazonSimpleSystemsManagement BuildClient() => new AmazonSimpleSystemsManagementClient(endpoint);

    /// <summary>Notifies of an <see cref="System.Exception"/> in a <see cref="ParameterStoreConfigurationProvider"/>, and relays it out to listeners.</summary>
    internal void NotifyException(Exception exception)
    {
      ExceptionObserved?.Invoke(exception);

      if (!isOptional)
      {
        throw new Exception("Failed to refresh SSM configuration settings. See inner exception for details.", exception);
      }
    }
  }
}