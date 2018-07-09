using Amazon.Lambda.Services;

namespace Amazon.Lambda.Hosting
{
  /// <summary>General options for hosting inside/outside of AWS.</summary>
  public sealed class HostingOptions
  {
    /// <summary>The <see cref="MatchingStrategy"/> to use for determining the lambda handler to execute.</summary>
    public MatchingStrategy MatchingStrategy { get; set; } = MatchingStrategies.MatchByNameSuffix();

    /// <summary>The <see cref="AmazonSettings"/> for the current environment.</summary>
    public AmazonSettings AWS { get; } = new AmazonSettings();

    /// <summary>The <see cref="Services.RedirectTable"/> for the current environment.</summary>
    public RedirectTable RedirectTable { get; } = new RedirectTable();

    /// <summary>General options for AWS-based services.</summary>
    public sealed class AmazonSettings
    {
      /// <summary>A default secret key for AWS services.</summary>
      public string AccessKey { get; set; }

      /// <summary>A default secret key for AWS services.</summary>
      public string SecretKey { get; set; }

      /// <summary>The default <see cref="RegionEndpoint"/> to use for services.</summary>
      public RegionEndpoint DefaultEndpoint { get; set; }
    }
  }
}