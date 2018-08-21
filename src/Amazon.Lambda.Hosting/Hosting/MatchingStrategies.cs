using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A strategy for matching lambda executions to a particular context.</summary>
  public delegate bool MatchingStrategy(LambdaHandlerMetadata metadata, ILambdaContext context);

  /// <summary>The default <see cref="MatchingStrategy"/>s.</summary>
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
      => (metadata, context) => LambdaARN.Parse(context.InvokedFunctionArn).FunctionName.Equals(metadata.FunctionName, comparison);
  }
}