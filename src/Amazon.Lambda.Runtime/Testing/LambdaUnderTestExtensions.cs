using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for dispatching particular workloads and configurations to a <see cref="ILambdaUnderTest{THandler}"/>.</summary>
  public static class LambdaUnderTestExtensions
  {
    // TODO: make this more efficient with less copying of the context
    
    /// <summary>Attaches the given <see cref="ILambdaContext"/> options to the given <see cref="ILambdaUnderTest{THandler}"/>.</summary>
    public static ILambdaUnderTest<THandler> WithContext<THandler>(this ILambdaUnderTest<THandler> target, Action<TestLambdaContext> configurer)
      where THandler : class, ILambdaHandler
    {
      var context = new TestLambdaContext(target.Context);

      configurer(context);

      return new LambdaUnderTest<THandler>(context, target.Handler, target.Services);
    }

    /// <summary>Adds a time limit to the <see cref="ILambdaUnderTest{THandler}"/>, adjusting it's <see cref="TestLambdaContext"/> as appropriate.</summary>
    public static ILambdaUnderTest<THandler> WithTimeLimit<THandler>(this ILambdaUnderTest<THandler> target, TimeSpan timeLimit)
      where THandler : class, ILambdaHandler
    {
      return target.WithContext(context =>
      {
        context.RemainingTime = timeLimit;
      });
    }
  }
}