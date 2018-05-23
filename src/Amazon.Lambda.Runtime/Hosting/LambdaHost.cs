using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>The default <see cref="ILambdaHost"/> implementation.</summary>
  internal sealed class LambdaHost : ILambdaHost
  {
    private readonly IHost host;

    public LambdaHost(IHost host)
    {
      Check.NotNull(host, nameof(host));

      this.host = host;
    }

    public async Task<object> ExecuteAsync(object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      var handler = ResolveLambdaHandler(context.FunctionName);

      return await handler.ExecuteAsync(input, context);
    }

    /// <summary>Resolves the appropraite <see cref="ILambdaHandler"/> for the given <see cref="functionName"/>.</summary>
    private ILambdaHandler ResolveLambdaHandler(string functionName)
    {
      var registrations = host.Services.GetServices<LambdaHandlerRegistration>();

      foreach (var registration in registrations)
      {
        if (registration.FunctionName.Equals(functionName, StringComparison.OrdinalIgnoreCase))
        {
          return (ILambdaHandler) host.Services.GetService(registration.HandlerType);
        }
      }

      throw new UnresolvedHandlerException($"Unable to locate an appropriate handler for the function {functionName}");
    }

    public void Dispose()
    {
      host.Dispose();
    }
  }
}