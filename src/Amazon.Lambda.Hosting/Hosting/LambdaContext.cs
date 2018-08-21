using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Model;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A <see cref="ILambdaContext"/> for local execution.</summary>
  public sealed class LambdaContext : ILambdaContext
  {
    public static LambdaContext ForFunction(string functionName)
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      return new LambdaContext
      {
        FunctionName       = functionName,
        FunctionVersion    = "$LATEST",
        InvokedFunctionArn = null
      };
    }

    public static LambdaContext ForFunction(RegionEndpoint region, long accountId, string functionName, string qualifier = null)
    {
      Check.NotNull(region, nameof(region));
      Check.That(accountId > 0, "accountId > 0");
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      var arn = new LambdaARN(region, accountId, functionName, qualifier);

      return new LambdaContext
      {
        FunctionName       = arn.FunctionName,
        FunctionVersion    = arn.Qualifier ?? "$LATEST",
        InvokedFunctionArn = arn.ToString()
      };
    }

    public static LambdaContext ForARN(LambdaARN arn)
    {
      Check.NotNull(arn, nameof(arn));

      return new LambdaContext
      {
        FunctionName       = arn.FunctionName,
        FunctionVersion    = arn.Qualifier,
        InvokedFunctionArn = arn.ToString(),
      };
    }

    public string           AwsRequestId       { get; set; } = Guid.NewGuid().ToString();
    public IClientContext   ClientContext      { get; set; } = null;
    public string           FunctionName       { get; set; } = string.Empty;
    public string           FunctionVersion    { get; set; } = "$LATEST";
    public string           InvokedFunctionArn { get; set; } = string.Empty;
    public ICognitoIdentity Identity           { get; set; } = null;
    public ILambdaLogger    Logger             { get; set; } = ConsoleLambdaLogger.Instance;
    public string           LogGroupName       { get; set; } = string.Empty;
    public string           LogStreamName      { get; set; } = string.Empty;
    public int              MemoryLimitInMB    { get; set; } = 3000;                      // maximum memory limit in AWS
    public TimeSpan         RemainingTime      { get; set; } = TimeSpan.FromSeconds(300); // maximum execution time for a lambda in AWS
  }
}