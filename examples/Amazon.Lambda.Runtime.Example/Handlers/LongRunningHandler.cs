using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Runtime.Example.Handlers
{
  [LambdaFunction("long-running-handler")]
  public sealed class LongRunningHandler : ILambdaHandler
  {
    public async Task<object> ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      await Task.Delay(10.Minutes(), cancellationToken);

      return "Hello, World!";
    }
  }
}