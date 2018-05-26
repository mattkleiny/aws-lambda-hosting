using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Handlers
{
  [LambdaFunction("handler-2")]
  public sealed class Handler2 : ILambdaHandler
  {
    private readonly IAmazonDynamoDB client;

    public Handler2(IAmazonDynamoDB client)
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