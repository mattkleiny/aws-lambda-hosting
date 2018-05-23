using Amazon.Lambda.Services;

namespace Amazon.Lambda.Hosting
{
  /// <summary>General options for hosting inside/outside of AWS.</summary>
  public sealed class HostingOptions
  {
    // TODO: move these into an 'AWS Options' nested class
    
    /// <summary>A default secret key for AWS services.</summary>
    public string AccessKey { get; set; }
    
    /// <summary>A default secret key for AWS services.</summary>
    public string SecretKey { get; set; }
    
    /// <summary>The default <see cref="RegionEndpoint"/> to use for services.</summary>
    public RegionEndpoint DefaultEndpoint { get; set; }

    /// <summary>The <see cref="Services.RedirectTable"/> for the current environment.</summary>
    public RedirectTable RedirectTable { get; } = new RedirectTable();
  }
}