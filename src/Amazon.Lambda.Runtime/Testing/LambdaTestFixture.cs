using System;
using Amazon.Lambda.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>
  /// A fixture for Lambda testing purposes.
  /// <para/>
  /// Builds a dummy AWS Lambda environment and provides hooks for execution of <see cref="ILambdaHandler"/>s from test cases.
  /// </summary>
  public sealed class LambdaTestFixture : IDisposable
  {
    private readonly IHost host;

    public LambdaTestFixture(IHostBuilder builder)
    {
      Check.NotNull(builder, nameof(builder));

      host = builder.Build();
    }

    /// <summary>Retrieves a <see cref="THandler"/> in a <see cref="ILambdaUnderTest{THandler}"/> scenario.</summary>
    public ILambdaUnderTest<THandler> GetHandler<THandler>()
      where THandler : class, ILambdaHandler
    {
      var context = new TestLambdaContext();
      var handler = host.Services.GetService<THandler>();

      return new LambdaUnderTest<THandler>(context, handler);
    }

    public void Dispose()
    {
      host.Dispose();
    }
  }
}