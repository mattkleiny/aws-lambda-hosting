using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>The default <see cref="LambdaUnderTest{THandler}"/> implementation.</summary>
  internal sealed class LambdaUnderTest<THandler> : ILambdaUnderTest<THandler>
    where THandler : class, ILambdaHandler
  {
    public LambdaUnderTest(ILambdaContext context, THandler handler, IServiceProvider services)
    {
      Check.NotNull(context,  nameof(context));
      Check.NotNull(handler,  nameof(handler));
      Check.NotNull(services, nameof(services));

      Context  = context;
      Handler  = handler;
      Services = services;
    }

    public ILambdaContext   Context  { get; }
    public THandler         Handler  { get; }
    public IServiceProvider Services { get; }

    public Task<object> ExecuteAsync(object input, CancellationToken cancellationToken)
    {
      return Handler.ExecuteAsync(input, Context, cancellationToken);
    }
  }
}