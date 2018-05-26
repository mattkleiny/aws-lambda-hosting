using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
    public static async Task<object> SendS3EventAsync<THandler>(this ILambdaUnderTest<THandler> target, Action<S3EventNotification> customizer, CancellationToken cancellationToken = default)
      where THandler : class, ILambdaHandler
    {
      var region = target.Services.GetService<IOptions<HostingOptions>>()?.Value?.AWS?.DefaultEndpoint;

      var @event = new S3Event
      {
        Records = new List<S3EventNotification.S3EventNotificationRecord>
        {
          new S3EventNotification.S3EventNotificationRecord
          {
            AwsRegion = region?.SystemName,
            EventTime = DateTime.UtcNow.AddMinutes(-10),
          }
        }
      };

      return await target.ExecuteAsync(@event, cancellationToken);
    }
  }
}