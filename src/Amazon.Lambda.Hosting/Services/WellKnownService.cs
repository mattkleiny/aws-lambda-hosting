using System;
using System.Diagnostics.CodeAnalysis;

namespace Amazon.Lambda.Services
{
  /// <summary>Denotes a well known service, either in AWS or elsewhere, for use in redirects.</summary>
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  public readonly record struct WellKnownService(string Key)
  {
    public static WellKnownService Dynamo        { get; } = new("aws-dynamo");
    public static WellKnownService ElastiCache   { get; } = new("aws-elasticache");
    public static WellKnownService S3            { get; } = new("aws-s3");
    public static WellKnownService SNS           { get; } = new("aws-sns");
    public static WellKnownService SQS           { get; } = new("aws-sqs");
    public static WellKnownService Lambda        { get; } = new("aws-lambda");
    public static WellKnownService StepFunctions { get; } = new("aws-stepfunctions");

    public override int GetHashCode()
    {
      return string.GetHashCode(Key, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(WellKnownService other)
    {
      return string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
    }
  }
}