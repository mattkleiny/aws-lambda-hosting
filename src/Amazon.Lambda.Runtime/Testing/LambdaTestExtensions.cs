using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for dispatching particular workloads and configurations to a <see cref="ILambdaUnderTest{THandler}"/>.</summary>
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