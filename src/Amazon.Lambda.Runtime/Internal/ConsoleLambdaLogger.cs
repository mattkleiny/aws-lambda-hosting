using System;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Internal
{
  /// <summary>A <see cref="ILambdaLogger"/> that forwards to the local console.</summary>
  internal sealed class ConsoleLambdaLogger : ILambdaLogger
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