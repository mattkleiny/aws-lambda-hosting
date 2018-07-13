using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for simplifying interaction with S3.</summary>
  public static class S3LambdaTestExtensions
  {
    /// <summary>Sends an <see cref="S3Event"/> to the <see cref="ILambdaUnderTest{THandler}"/> with a default notification.</summary>
    public static Task<object> SendS3EventAsync<THandler>(this ILambdaUnderTest<THandler> target, CancellationToken cancellationToken = default)
      where THandler : class, ILambdaHandler
    {
      return target.SendS3EventAsync(_ => { }, cancellationToken);
    }

    /// <summary>Sends an <see cref="S3Event"/> to the <see cref="ILambdaUnderTest{THandler}"/> with a customizable <see cref="S3EventNotification"/>.</summary>
    public static Task<object> SendS3EventAsync<THandler>(this ILambdaUnderTest<THandler> target, Action<S3EventNotification> customizer, CancellationToken cancellationToken = default)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }
  }
}