using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>
  /// Denotes a <see cref="ILambdaHandler"/> that is bootstrapped for testing.
  /// <para/>
  /// Provides a convenience interface for execution of the Lambda itself as well as a common extension point for configuration.
  /// </summary>
  public interface ILambdaUnderTest<out THandler>
    where THandler : class, ILambdaHandler
  {
    ILambdaContext Context { get; }
    THandler       Handler { get; }

    Task<object> ExecuteAsync(object input);
  }
}