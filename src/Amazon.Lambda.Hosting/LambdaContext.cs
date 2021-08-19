using System;
using System.Diagnostics;
using Amazon.Lambda.Core;

namespace Amazon.Lambda
{
  /// <summary>A utility <see cref="ILambdaContext"/> for local execution.</summary>
  public sealed record LambdaContext : ILambdaContext
  {
    public static LambdaContext ForFunction(string functionName)
    {
      Debug.Assert(!string.IsNullOrEmpty(functionName), "!string.IsNullOrEmpty(functionName)");

      return new LambdaContext
      {
        FunctionName       = functionName,
        FunctionVersion    = "$LATEST",
        InvokedFunctionArn = null
      };
    }

    public static LambdaContext ForFunction(RegionEndpoint region, long accountId, string functionName, string? qualifier = null)
    {
      Debug.Assert(!string.IsNullOrEmpty(functionName), "!string.IsNullOrEmpty(functionName)");

      var arn = new LambdaArn(region, accountId, functionName, qualifier);

      return ForFunction(arn);
    }

    public static LambdaContext ForFunction(LambdaArn arn)
    {
      return new LambdaContext
      {
        FunctionName       = arn.FunctionName,
        FunctionVersion    = arn.Qualifier ?? "$LATEST",
        InvokedFunctionArn = arn.ToString()
      };
    }

    public static LambdaContext ForArn(string arn)
    {
      Debug.Assert(!string.IsNullOrEmpty(arn), "!string.IsNullOrEmpty(arn)");

      return ForArn(LambdaArn.Parse(arn));
    }

    public static LambdaContext ForArn(LambdaArn arn)
    {
      return new LambdaContext
      {
        FunctionName       = arn.FunctionName,
        FunctionVersion    = arn.Qualifier,
        InvokedFunctionArn = arn.ToString(),
      };
    }

    public string            AwsRequestId       { get; set; } = Guid.NewGuid().ToString();
    public IClientContext?   ClientContext      { get; set; } = null;
    public string?           FunctionName       { get; set; } = null;
    public string?           FunctionVersion    { get; set; } = "$LATEST";
    public string?           InvokedFunctionArn { get; set; } = null;
    public ICognitoIdentity? Identity           { get; set; } = null;
    public ILambdaLogger     Logger             { get; set; } = ConsoleLambdaLogger.Instance;
    public string            LogGroupName       { get; set; } = string.Empty;
    public string            LogStreamName      { get; set; } = string.Empty;
    public int               MemoryLimitInMB    { get; set; } = 3000;                     // maximum memory limit in AWS
    public TimeSpan          RemainingTime      { get; set; } = TimeSpan.FromMinutes(15); // maximum execution time for a lambda in AWS

    /// <summary>A simple <see cref="ILambdaLogger"/> that writes straight to the standard console.</summary>
    private sealed class ConsoleLambdaLogger : ILambdaLogger
    {
      public static ILambdaLogger Instance { get; } = new ConsoleLambdaLogger();

      public void Log(string message)     => Console.Write(message);
      public void LogLine(string message) => Console.WriteLine(message);
    }
  }
}