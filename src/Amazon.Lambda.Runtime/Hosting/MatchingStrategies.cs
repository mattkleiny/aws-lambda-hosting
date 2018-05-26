﻿using System;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A strategy for matching lambda executions to a particular context.</summary>
  public delegate bool MatchingStrategy(LambdaHandlerRegistration registration, object input, ILambdaContext context);

  /// <summary>The default <see cref="MatchingStrategy"/>s.</summary>
  public static class MatchingStrategies
  {
    public static MatchingStrategy MatchByName(StringComparison comparison)
    {
      return (registration, input, context) => context.FunctionName.Equals(registration.FunctionName, comparison);
    }

    public static MatchingStrategy MatchByNamePrefix(StringComparison comparison)
    {
      return (registration, input, context) => context.FunctionName.StartsWith(registration.FunctionName, comparison);
    }

    public static MatchingStrategy MatchByNameSuffix(StringComparison comparison)
    {
      return (registration, input, context) => context.FunctionName.EndsWith(registration.FunctionName, comparison);
    }
  }
}