using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Services;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A strategy for matching lambda executions to a particular context.</summary>
  public delegate bool MatchingStrategy(LambdaHandlerMetadata metadata, ILambdaContext context);

  /// <summary>General options for hosting inside/outside of AWS.</summary>
  public sealed class HostingOptions
  {
    /// <summary>The <see cref="MatchingStrategy"/> to use for determining the lambda handler to execute.</summary>
    public MatchingStrategy MatchingStrategy { get; set; } = MatchingStrategies.MatchByNameSuffix();

    /// <summary>The <see cref="AmazonSettings"/> for the current environment.</summary>
    public AmazonSettings AWS { get; } = new();

    /// <summary>The <see cref="Services.RedirectTable"/> for the current environment.</summary>
    public RedirectTable RedirectTable { get; } = new();

    /// <summary>The <see cref="Services.RedirectTable"/> for proxied network communication in the current environment.</summary>
    public RedirectTable ProxyTable { get; } = new();

    /// <summary>General options for AWS-based services.</summary>
    public sealed class AmazonSettings
    {
      /// <summary>The access key for AWS services.</summary>
      public string? AccessKey { get; set; }

      /// <summary>The secret key for AWS services.</summary>
      public string? SecretKey { get; set; }

      /// <summary>The default <see cref="RegionEndpoint"/> to use for services.</summary>
      public RegionEndpoint? DefaultEndpoint { get; set; }
    }
  }

  public static class MatchingStrategies
  {
    public static MatchingStrategy MatchByName(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => (metadata, context) => context.FunctionName.Equals(metadata.FunctionName, comparison);

    public static MatchingStrategy MatchByNamePrefix(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => (metadata, context) => context.FunctionName.StartsWith(metadata.FunctionName, comparison);

    public static MatchingStrategy MatchByNameSuffix(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => (metadata, context) => context.FunctionName.EndsWith(metadata.FunctionName, comparison);

    public static MatchingStrategy MatchByArn(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => (metadata, context) => context.InvokedFunctionArn.Equals(metadata.FunctionName, comparison);

    public static MatchingStrategy MatchByArnSuffix(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => (metadata, context) => context.InvokedFunctionArn.EndsWith(metadata.FunctionName, comparison);

    public static MatchingStrategy MatchByParsedArnFunctionName(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => (metadata, context) => LambdaArn.Parse(context.InvokedFunctionArn).FunctionName.Equals(metadata.FunctionName, comparison);
  }
}