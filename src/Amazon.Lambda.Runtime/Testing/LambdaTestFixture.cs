using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>
  /// A fixture for <see cref="LambdaHost"/> for testing purposes.
  /// <para/>
  /// Builds a dummy AWS Lambda environment and provides hooks for execution of
  /// <see cref="ILambdaHandler"/>s from test cases.
  /// </summary>
  public sealed class LambdaTestFixture
  {
    public LambdaTestFixture(LambdaHostBuilder builder)
    {
      Check.NotNull(builder, nameof(builder));

      throw new NotImplementedException();
    }

    public ILambdaUnderTest<THandler> GetHandler<THandler>()
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }
  }

  public interface ILambdaUnderTest<out THandler>
    where THandler : class, ILambdaHandler
  {
    ILambdaContext Context { get; }
    THandler       Handler { get; }

    Task<object> ExecuteAsync();
  }

  public static class LambdaTestExtensions
  {
    public static ILambdaUnderTest<THandler> WithContext<THandler>(this ILambdaUnderTest<THandler> target, ILambdaContext context)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }

    public static ILambdaUnderTest<THandler> WithDerivedContext<THandler>(this ILambdaUnderTest<THandler> target, ILambdaContext context)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }

    public static async Task<object> SendDynamoEventAsync<THandler>(this ILambdaUnderTest<THandler> target, object payload)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }

    public static async Task<object> SendSnsEventAsync<THandler>(this ILambdaUnderTest<THandler> target, object payload)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }

    public static async Task<object> SendS3EventAsync<THandler>(this ILambdaUnderTest<THandler> target, object payload)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }
  }
}