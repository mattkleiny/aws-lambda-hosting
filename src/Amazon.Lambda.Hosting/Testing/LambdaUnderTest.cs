using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>The default <see cref="LambdaUnderTest{THandler}"/> implementation.</summary>
  internal sealed class LambdaUnderTest<THandler> : ILambdaUnderTest<THandler>
    where THandler : class, ILambdaHandler
  {
    public LambdaUnderTest(LambdaContext context, THandler handler, IServiceProvider services)
    {
      Check.NotNull(context,  nameof(context));
      Check.NotNull(handler,  nameof(handler));
      Check.NotNull(services, nameof(services));

      Context  = context;
      Handler  = handler;
      Services = services;
    }

    public LambdaContext    Context  { get; }
    public THandler         Handler  { get; }
    public IServiceProvider Services { get; }

    public Task<object> ExecuteAsync(object input, CancellationToken cancellationToken)
    {
      // limit the execution time of the lambda based on the remaining time in the context
      using (var timeLimit = new CancellationTokenSource(Context.RemainingTime))
      using (var linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeLimit.Token))
      {
        if (linkedTokens.IsCancellationRequested)
        {
          return Task.FromCanceled<object>(linkedTokens.Token);
        }

        return Handler.ExecuteAsync(input, Context, linkedTokens.Token);
      }
    }
  }
}