using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A <see cref="ILambdaContext"/> for local execution.</summary>
  public sealed class LocalLambdaContext : ILambdaContext
  {
    public LocalLambdaContext(string functionName)
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      FunctionName = functionName;
    }

    public string           AwsRequestId       { get; set; } = Guid.NewGuid().ToString();
    public IClientContext   ClientContext      { get; set; } = null;
    public string           FunctionName       { get; set; }
    public string           FunctionVersion    { get; set; } = "$LATEST";
    public ICognitoIdentity Identity           { get; set; } = null;
    public string           InvokedFunctionArn { get; set; } = string.Empty;
    public ILambdaLogger    Logger             { get; set; } = ConsoleLambdaLogger.Instance;
    public string           LogGroupName       { get; set; } = string.Empty;
    public string           LogStreamName      { get; set; } = string.Empty;
    public int              MemoryLimitInMB    { get; set; } = 3000;                      // maximum memory limit in AWS
    public TimeSpan         RemainingTime      { get; set; } = TimeSpan.FromSeconds(300); // maximum execution time for a lambda in AWS
  }
}