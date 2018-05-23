using System;
using System.Collections;
using System.Collections.Generic;
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

  public sealed class RedirectTable : IEnumerable<KeyValuePair<string, Uri>>
  {
    private readonly IDictionary<string, Uri> entries = new Dictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);

    public Uri this[string service]
    {
      set
      {
        Check.That(value.IsAbsoluteUri, "The URI provided for {service} needs to be absolute");

        entries[service] = value;
      }
    }

    public Uri this[WellKnownService service]
    {
      set => this[ResolveService(service)] = value;
    }

    private static string ResolveService(WellKnownService service)
    {
      switch (service)
      {
        case WellKnownService.Dynamo: return "https://dynamodb.aws.com";
        case WellKnownService.S3:     return "https://s3.aws.com";

        default:
          throw new ArgumentException($"An unrecognized service was provided: {service}");
      }
    }

    public IEnumerator<KeyValuePair<string, Uri>> GetEnumerator() => entries.GetEnumerator();
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
  }

  public enum WellKnownService
  {
    Dynamo,
    S3,
  }
}