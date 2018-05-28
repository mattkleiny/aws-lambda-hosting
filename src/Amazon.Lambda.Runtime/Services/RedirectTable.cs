using System;
using System.Collections;
using System.Collections.Generic;

namespace Amazon.Lambda.Services
{
  /// <summary>
  /// A simple table for providing redirects for common application services.
  /// <para/>
  /// You can use this to set-up a single instance of a service inside the DI container and then
  /// redirect it's endpoint based on your hosting environment.
  /// </summary>
  public sealed class RedirectTable : IEnumerable<KeyValuePair<string, Uri>>
  {
    private readonly IDictionary<string, Uri> entries = new Dictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Determines if the given service contains an entry in the table.</summary>
    public bool Contains(string service) => entries.ContainsKey(service);

    /// <summary>Determines if the given <see cref="WellKnownService"/> contains an entry in the table.</summary>
    public bool Contains(WellKnownService service) => Contains(ResolveService(service));

    /// <summary>Sets the redirect <see cref="Uri"/> for the given service.</summary>
    public Uri this[string service]
    {
      get
      {
        if (!entries.TryGetValue(service, out var uri))
        {
          throw new ArgumentException($"The given service {service} is not mapped in the redirect table.");
        }

        return uri;
      }
      set
      {
        Check.That(value.IsAbsoluteUri, "The URI provided for {service} needs to be absolute");

        entries[service] = value;
      }
    }

    /// <summary>Sets the redirect <see cref="Uri"/> for the given <see cref="WellKnownService"/> .</summary>
    public Uri this[WellKnownService service]
    {
      get => this[ResolveService(service)];
      set => this[ResolveService(service)] = value;
    }

    private static string ResolveService(WellKnownService service)
    {
      switch (service)
      {
        case WellKnownService.Dynamo:      return "dynamo";
        case WellKnownService.ElastiCache: return "elasticache";
        case WellKnownService.S3:          return "s3";
        case WellKnownService.SNS:         return "sns";
        case WellKnownService.SQS:         return "sqs";
        case WellKnownService.Lambda:      return "lambda";

        default:
          throw new ArgumentException($"An unrecognized service was provided: {service}");
      }
    }

    public IEnumerator<KeyValuePair<string, Uri>> GetEnumerator() => entries.GetEnumerator();
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
  }
}