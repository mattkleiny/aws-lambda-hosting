using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Services;

namespace Amazon.Lambda.Handlers
{
  [LambdaFunction("lambda-runtime-example-handler-2")]
  public sealed class Handler2 : ILambdaHandler
  {
    private readonly ITestService collaborator;

    public Handler2(ITestService collaborator)
    {
      Check.NotNull(collaborator, nameof(collaborator));

      this.collaborator = collaborator;
    }

    public async Task<object> ExecuteAsync(object input, ILambdaContext context)
    {
      return await collaborator.GetMessageAsync();
    }
  }
}