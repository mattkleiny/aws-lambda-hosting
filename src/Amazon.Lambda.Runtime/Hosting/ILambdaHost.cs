using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Represents a host for <see cref="ILambdaHandler"/> execution.</summary>
  public interface ILambdaHost : IDisposable
  {
    /// <summary>Executes the appropriate handler for the given input and context.</summary>
    Task<object> ExecuteAsync(object input, ILambdaContext context);
  }
}