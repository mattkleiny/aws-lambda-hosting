using System;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Diagnostics
{
  /// <summary>A <see cref="ILambdaLogger"/> that forwards to the local console.</summary>
  public sealed class ConsoleLambdaLogger : ILambdaLogger
  {
    public static readonly ILambdaLogger Instance = new ConsoleLambdaLogger();

    public void Log(string message)
    {
      Console.Write(message);
    }

    public void LogLine(string message)
    {
      Console.WriteLine(message);
    }
  }
}