using System;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Indicates a <see cref="ILambdaHandler"/> could not be resolved from the application.</summary>
  public sealed class UnresolvedHandlerException : Exception
  {
    public UnresolvedHandlerException(string message)
      : base(message)
    {
    }
  }
}