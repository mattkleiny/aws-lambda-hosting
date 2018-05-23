using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>The default <see cref="LambdaUnderTest{THandler}"/> implementation.</summary>
  internal sealed class LambdaUnderTest<THandler> : ILambdaUnderTest<THandler>
    where THandler : class, ILambdaHandler
  {
    public LambdaUnderTest(ILambdaContext context, THandler handler)
    {
      Check.NotNull(context, nameof(context));
      Check.NotNull(handler, nameof(handler));

      Context = context;
      Handler = handler;
    }

    public ILambdaContext Context { get; }
    public THandler       Handler { get; }

    public Task<object> ExecuteAsync(object input)
    {
      return Handler.ExecuteAsync(input, Context);
    }
  }
}