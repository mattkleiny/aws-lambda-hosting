using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for dispatching particular workloads and configurations to a <see cref="ILambdaUnderTest{THandler}"/>.</summary>
  public static class LambdaUnderTestExtensions
  {
    /// <summary>Attaches the given <see cref="ILambdaContext"/> options to the given <see cref="ILambdaUnderTest{THandler}"/>.</summary>
    public static ILambdaUnderTest<THandler> WithContext<THandler>(this ILambdaUnderTest<THandler> target, Action<TestLambdaContext> configurer)
      where THandler : class, ILambdaHandler
    {
      var context = new TestLambdaContext(target.Context);

      configurer(context);

      return new LambdaUnderTest<THandler>(context, target.Handler, target.Services);
    }
  }
}