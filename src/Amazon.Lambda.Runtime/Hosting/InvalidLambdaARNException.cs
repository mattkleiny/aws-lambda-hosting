using System;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Indicates a failure to parse a <see cref="LambdaARN"/>.</summary>
  public sealed class InvalidLambdaARNException : Exception
  {
    public InvalidLambdaARNException(string arn)
      : base($"'{arn}' is not a valid lambda ARN")
    {
    }
  }
}