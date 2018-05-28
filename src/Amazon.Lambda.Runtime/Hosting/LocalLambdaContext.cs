using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Internal;

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

    public string           AwsRequestId       { get; } = Guid.NewGuid().ToString();
    public IClientContext   ClientContext      { get; } = null;
    public string           FunctionName       { get; }
    public string           FunctionVersion    { get; } = "Unknown";
    public ICognitoIdentity Identity           { get; } = null;
    public string           InvokedFunctionArn { get; } = string.Empty;
    public ILambdaLogger    Logger             { get; } = ConsoleLambdaLogger.Instance;
    public string           LogGroupName       { get; } = string.Empty;
    public string           LogStreamName      { get; } = string.Empty;
    public int              MemoryLimitInMB    { get; } = int.MaxValue;
    public TimeSpan         RemainingTime      { get; } = TimeSpan.MaxValue;
  }
}