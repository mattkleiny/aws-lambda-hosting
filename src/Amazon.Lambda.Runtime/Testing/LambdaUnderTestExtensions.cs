using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for dispatching particular workloads and configurations to a <see cref="ILambdaUnderTest{THandler}"/>.</summary>
  public static class LambdaUnderTestExtensions
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
  }
}