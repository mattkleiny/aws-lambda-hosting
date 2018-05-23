using System;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Indicates a <see cref="ILambdaHandler"/> could not be resolved.</summary>
  public sealed class UnresolvedHandlerException : Exception
  {
    public UnresolvedHandlerException(string message)
      : base(message)
    {
    }
  }
}