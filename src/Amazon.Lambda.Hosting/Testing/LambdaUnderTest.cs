using System;
using System.Threading;
using System.Threading.Tasks;
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
    LambdaContext    Context  { get; }
    THandler         Handler  { get; }
    IServiceProvider Services { get; }

    Task<object?> ExecuteAsync(object input, CancellationToken cancellationToken = default);
  }

  internal sealed class LambdaUnderTest<THandler> : ILambdaUnderTest<THandler>
    where THandler : class, ILambdaHandler
  {
    public LambdaUnderTest(LambdaContext context, THandler handler, IServiceProvider services)
    {
      Context  = context;
      Handler  = handler;
      Services = services;
    }

    public LambdaContext    Context  { get; }
    public THandler         Handler  { get; }
    public IServiceProvider Services { get; }

    public Task<object?> ExecuteAsync(object input, CancellationToken cancellationToken)
    {
      // limit the execution time of the lambda based on the remaining time in the context
      using var timeLimit    = new CancellationTokenSource(Context.RemainingTime);
      using var linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeLimit.Token);

      if (linkedTokens.IsCancellationRequested)
      {
        return Task.FromCanceled<object?>(linkedTokens.Token);
      }

      return Handler.ExecuteAsync(input, Context, linkedTokens.Token);
    }
  }
}