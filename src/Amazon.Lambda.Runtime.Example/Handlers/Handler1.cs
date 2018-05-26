using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.S3;

namespace Amazon.Lambda.Runtime.Example.Handlers
{
  [LambdaFunction("handler-1")]
  public sealed class Handler1 : ILambdaHandler
  {
    private readonly IAmazonS3 client;

    public Handler1(IAmazonS3 client)
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