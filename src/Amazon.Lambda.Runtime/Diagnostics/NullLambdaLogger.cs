using Amazon.Lambda.Core;

namespace Amazon.Lambda.Diagnostics
{
  /// <summary>A no-op <see cref="ILambdaLogger"/> implementation.</summary>
  public sealed class NullLambdaLogger : ILambdaLogger
  {
    public static readonly ILambdaLogger Instance = new NullLambdaLogger();

    public void Log(string message)
    {
      // no-op
    }

    public void LogLine(string message)
    {
      // no-op
    }
  }
}