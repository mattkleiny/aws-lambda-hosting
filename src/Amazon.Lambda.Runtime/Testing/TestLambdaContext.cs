using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Diagnostics;
using Amazon.Lambda.Internal;

namespace Amazon.Lambda.Testing
{
  /// <summary>A <see cref="ILambdaContext"/> for testing.</summary>
  public sealed class TestLambdaContext : ILambdaContext
  {
    public TestLambdaContext()
    {
    }

    public TestLambdaContext(ILambdaContext other)
    {
      Check.NotNull(other, nameof(other));

      AwsRequestId       = other.AwsRequestId;
      ClientContext      = other.ClientContext;
      FunctionName       = other.FunctionName;
      FunctionVersion    = other.FunctionVersion;
      Identity           = other.Identity;
      InvokedFunctionArn = other.InvokedFunctionArn;
      Logger             = other.Logger;
      LogGroupName       = other.LogGroupName;
      LogStreamName      = other.LogStreamName;
      MemoryLimitInMB    = other.MemoryLimitInMB;
      RemainingTime      = other.RemainingTime;
    }
    
    public string           AwsRequestId       { get; set; } = Guid.NewGuid().ToString();
    public IClientContext   ClientContext      { get; set; }
    public string           FunctionName       { get; set; }
    public string           FunctionVersion    { get; set; } = "Unknown";
    public ICognitoIdentity Identity           { get; set; }
    public string           InvokedFunctionArn { get; set; } = string.Empty;
    public ILambdaLogger    Logger             { get; set; } = NullLambdaLogger.Instance;
    public string           LogGroupName       { get; set; } = string.Empty;
    public string           LogStreamName      { get; set; } = string.Empty;
    public int              MemoryLimitInMB    { get; set; } = int.MaxValue;
    public TimeSpan         RemainingTime      { get; set; } = TimeSpan.MaxValue;
  }
}