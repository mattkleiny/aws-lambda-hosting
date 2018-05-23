using System;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for simplifying testing with DynamoDB.</summary>
  public static class DynamoLambdaTestExtensions
  {
    public static async Task<object> SendDynamoEventAsync<THandler>(this ILambdaUnderTest<THandler> target, object payload)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }
  }
}