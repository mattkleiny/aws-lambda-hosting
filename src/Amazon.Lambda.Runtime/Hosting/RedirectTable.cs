using System;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda.Hosting
{
  public static class RedirectExtensions
  {
    public static IServiceCollection ConfigureHostingOptions(this IServiceCollection services, Action<HostingOptions> configurer)
    {
      throw new NotImplementedException();
    }
  }

  public class HostingOptions
  {
    public void AddRedirectTable(RedirectTable table)
    {
      throw new NotImplementedException();
    }
  }

  public sealed class RedirectTable
  {
    public Uri this[string key]
    {
      set => throw new NotImplementedException();
    }

    public Uri this[WellKnownService type]
    {
      set => throw new NotImplementedException();
    }
  }

  public enum WellKnownService
  {
    DynamoDB,
    S3,
  }
}