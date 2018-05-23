using System;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A default <see cref="ILambdaContext"/> for local execution.</summary>
  public sealed class LocalLambdaContext : ILambdaContext
  {
    public LocalLambdaContext(string functionName)
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));
      
      FunctionName = functionName;
    }

    public string           AwsRequestId       { get; } = Guid.NewGuid().ToString();
    public IClientContext   ClientContext      { get; } = null; // TODO: mock this?
    public string           FunctionName       { get; }
    public string           FunctionVersion    { get; } = "Unknown";
    public ICognitoIdentity Identity           { get; } = null; // TODO: mock this?
    public string           InvokedFunctionArn { get; } = string.Empty;
    public ILambdaLogger    Logger             { get; } = null; // TODO: adapt this?
    public string           LogGroupName       { get; } = string.Empty;
    public string           LogStreamName      { get; } = string.Empty;
    public int              MemoryLimitInMB    { get; } = int.MaxValue;
    public TimeSpan         RemainingTime      { get; } = TimeSpan.MaxValue;
  }
}