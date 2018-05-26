using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.S3;

namespace Amazon.Lambda.Handlers
{
  [LambdaFunction("lambda-runtime-example-handler-1")]
  public sealed class Handler1 : ILambdaHandler
  {
    private readonly AmazonS3Client client;

    public Handler1(AmazonS3Client client)
    {
      Check.NotNull(client, nameof(client));

      this.client = client;
    }

    public Task<object> ExecuteAsync(object input, ILambdaContext context, CancellationToken token)
    {
      return Task.FromResult<object>($"S3 configured with {client.Config.ServiceURL}");
    }
  }
}