using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for simplifying testing with DynamoDB.</summary>
  public static class DynamoLambdaTestExtensions
  {
    /// <summary>Sends a <see cref="DynamoDBEvent"/> to the <see cref="ILambdaUnderTest{THandler}"/> with a default notification.</summary>
    public static Task<object> SendDynamoEventAsync<THandler>(this ILambdaUnderTest<THandler> target, CancellationToken token = default)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }
  }
}