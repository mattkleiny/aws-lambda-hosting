using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Handlers
{
  [LambdaFunction("lambda-runtime-example-handler-2")]
  public sealed class Handler2 : ILambdaHandler
  {
    private readonly AmazonDynamoDBClient client;

    public Handler2(AmazonDynamoDBClient client)
    {
      Check.NotNull(client, nameof(client));
      
      this.client = client;
    }

    public Task<object> ExecuteAsync(object input, ILambdaContext context, CancellationToken token)
    {
      return Task.FromResult<object>($"Dynamo configured with {client.Config.ServiceURL}");
    }
  }
}