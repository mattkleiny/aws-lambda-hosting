using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Handlers
{
  [LambdaFunction("lambda-runtime-example-handler-1")]
  public sealed class Handler1 : ILambdaHandler
  {
    private readonly HostingOptions options;

    public Handler1(IOptions<HostingOptions> options)
    {
      Check.NotNull(options, nameof(options));
      
      this.options = options.Value;
    }

    public Task<object> ExecuteAsync(object input, ILambdaContext context)
    {
      return Task.FromResult(options.RedirectTable["dynamo"] as object);
    }
  }
}