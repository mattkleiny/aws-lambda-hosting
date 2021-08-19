using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
    private readonly Dictionary<string, Uri> entries = new(StringComparer.OrdinalIgnoreCase);

    public bool Contains(string service)           => entries.ContainsKey(service);
    public bool Contains(WellKnownService service) => Contains(service.Key);

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
        Debug.Assert(value.IsAbsoluteUri, $"The URI provided for {service} needs to be absolute");

        entries[service] = value;
      }
    }

    public Uri this[WellKnownService service]
    {
      get => this[service.Key];
      set => this[service.Key] = value;
    }

    public IEnumerator<KeyValuePair<string, Uri>> GetEnumerator() => entries.GetEnumerator();
    IEnumerator IEnumerable.                      GetEnumerator() => GetEnumerator();
  }
}