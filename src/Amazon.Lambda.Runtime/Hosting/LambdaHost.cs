using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  // TODO: add redirect table for internal AWS services 
  //			 i.e. [Service.DynamoDB] => "https://localhost:3000/"
  // TODO: add confiurable environment source property
  //			 i.e. .DiscerneEnvironmentFrom("SOME_ENVIRONMENT_VARIABLE")
  // TODO: add testing harness with easy payload pass-through
  //       i.e. Fixture.GetLambda<SomeLambd>().SendS3Event() or .SendSNSEvent()
  // TODO: add helpers for step functions, perhaps?
  // TODO: bootstrap the handler based on the lambda context function name, instead?
  // TODO: consider functional execution instead of handler classes
  //			 i.e. ExecuteAsync(object input, ILambdaContext context, IConfiguration configuration, AmazonSQSClient sqs);
  // TODO: don't forget the console and docker extension ideas
  // TODO: configure well-known and extensible pipeline for deserialization

  public sealed class LambdaHost : IDisposable
  {
    private readonly IHost host;

    internal LambdaHost(IHost host)
    {
      Check.NotNull(host, nameof(host));

      this.host = host;
    }

    public async Task<object> ExecuteAsync(object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      throw new NotImplementedException();
    }

    public void Dispose()
    {
      host.Dispose();
    }
  }
}