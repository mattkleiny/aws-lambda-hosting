using System;
using System.Threading.Tasks;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
  /// <summary>Extensions for simplifying interaction with S3.</summary>
  public static class S3LambdaTestExtensions
  {
    public static async Task<object> SendS3EventAsync<THandler>(this ILambdaUnderTest<THandler> target, object payload)
      where THandler : class, ILambdaHandler
    {
      throw new NotImplementedException();
    }
  }
}