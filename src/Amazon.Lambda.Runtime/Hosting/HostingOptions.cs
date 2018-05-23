using Amazon.Lambda.Services;

namespace Amazon.Lambda.Hosting
{
  /// <summary>General options for hosting inside/outside of AWS.</summary>
  public sealed class HostingOptions
  {
    /// <summary>The <see cref="Services.RedirectTable"/> for the current environment.</summary>
    public RedirectTable RedirectTable { get; } = new RedirectTable();
  }
}