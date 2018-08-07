using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;

namespace Amazon.Lambda.Testing
{
  /// <summary>A <see cref="ILambdaContext"/> for testing.</summary>
  public sealed class TestLambdaContext : ILambdaContext
  {
    public string           AwsRequestId       { get; set; } = Guid.NewGuid().ToString();
    public IClientContext   ClientContext      { get; set; } = null;
    public string           FunctionName       { get; set; } = string.Empty;
    public string           FunctionVersion    { get; set; } = "$LATEST";
    public ICognitoIdentity Identity           { get; set; } = null;
    public string           InvokedFunctionArn { get; set; } = string.Empty;
    public ILambdaLogger    Logger             { get; set; } = NullLambdaLogger.Instance;
    public string           LogGroupName       { get; set; } = string.Empty;
    public string           LogStreamName      { get; set; } = string.Empty;
    public int              MemoryLimitInMB    { get; set; } = 3000;                      // maximum memory limit in AWS
    public TimeSpan         RemainingTime      { get; set; } = TimeSpan.FromSeconds(300); // maximum execution time for a lambda in AWS
  }
}