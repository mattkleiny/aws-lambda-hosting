using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Model;

namespace Amazon.Lambda.Hosting.Embedding
{
  /// <summary>An <see cref="AmazonLambdaDecorator"/> that permits resolution of locally-installed lambda handlers before delegating to the actual AWS infrastructure.</summary>
  internal sealed class EmbeddedAmazonLambda : AmazonLambdaDecorator
  {
    private readonly IServiceProvider services;

    public EmbeddedAmazonLambda(IAmazonLambda client, IServiceProvider services)
      : base(client)
    {
      Check.NotNull(services, nameof(services));

      this.services = services;
    }

    public override async Task<InvokeResponse> InvokeAsync(InvokeRequest request, CancellationToken cancellationToken = default)
    {
      Check.NotNull(request, nameof(request));

      // TODO: what about payload deserialization?

      var input   = request.Payload;
      var context = LambdaContext.ForFunction(request.FunctionName);

      if (!services.TryResolveLambdaHandler(input, context, out var handler))
      {
        return await base.InvokeAsync(request, cancellationToken);
      }

      var result = await handler.ExecuteAsync(input, context, cancellationToken);
      var bytes  = Encoding.UTF8.GetBytes(result.ToString());

      // TODO: what about payload serialization?

      return new InvokeResponse
      {
        Payload        = new MemoryStream(bytes),
        HttpStatusCode = HttpStatusCode.OK
      };
    }
  }
}