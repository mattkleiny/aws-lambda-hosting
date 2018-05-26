using Amazon.Lambda.Core;

namespace Amazon.Lambda.Internal
{
  /// <summary>A no-op <see cref="ILambdaLogger"/> i8mplementation.</summary>
  internal sealed class NullLambdaLogger : ILambdaLogger
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