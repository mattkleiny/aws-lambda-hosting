using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A dispatcher for functional Lambda invocation with injected parameters.</summary>
  internal sealed class FunctionalDispatcher<THandler> : ILambdaHandler
    where THandler : class
  {
    private readonly IHost            host;
    private readonly TargetFunction[] functions;
    private readonly MatchingStrategy isMatch;

    /// <summary>Discovers all of the <see cref="TargetFunction"/>s associated with <see cref="THandler"/>.</summary>
    public static TargetFunction[] DiscoverFunctions() =>
      // ReSharper disable once InvokeAsExtensionMethod
      Enumerable.Concat(
          typeof(THandler).GetMethods(BindingFlags.Public | BindingFlags.Instance),
          typeof(THandler).GetMethods(BindingFlags.Public | BindingFlags.Static)
        )
        .Where(method => method.GetCustomAttribute<LambdaFunctionAttribute>() != null)
        .Select(method => new TargetFunction(method, method.GetCustomAttribute<LambdaFunctionAttribute>()!.FunctionName))
        .ToArray();

    public FunctionalDispatcher(IHost host, IOptions<HostingOptions> hostingOptions, TargetFunction[] functions)
    {
      this.host      = host;
      this.functions = functions;

      isMatch = hostingOptions.Value.MatchingStrategy;
    }

    public async Task<object?> ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      foreach (var function in functions)
      {
        if (isMatch(function.Metadata, context))
        {
          return await function.Invoke(input, context, host.Services, cancellationToken);
        }
      }

      throw new UnresolvedHandlerException($"Unable to resolve functional handler for {context.FunctionName}");
    }

    /// <summary>A wrapper for a functional invocation target.</summary>
    internal sealed class TargetFunction
    {
      private readonly ParameterInfo[] parameters;

      public TargetFunction(MethodInfo method, string functionName)
      {
        Debug.Assert(!string.IsNullOrEmpty(functionName), "!string.IsNullOrEmpty(functionName)");

        parameters = method.GetParameters().ToArray();

        Method       = method;
        FunctionName = functionName;

        Metadata = LambdaHandlerMetadata.ForFunction(this);
      }

      public MethodInfo Method       { get; }
      public string     FunctionName { get; }
      public string     FriendlyName => Method.Name;

      internal LambdaHandlerMetadata Metadata { get; }

      // TODO: remove the dynamic dispatch

      /// <summary>Invokes the underlying method, injecting it's parameters as required.</summary>
      public dynamic Invoke(object input, ILambdaContext context, IServiceProvider services, CancellationToken cancellationToken)
      {
        var arguments = parameters.Select(parameter =>
        {
          if ("input".Equals(parameter.Name, StringComparison.OrdinalIgnoreCase)) return input;

          if (parameter.ParameterType == typeof(object)) return input;
          if (parameter.ParameterType == typeof(ILambdaContext)) return context;
          if (parameter.ParameterType == typeof(IServiceProvider)) return services;
          if (parameter.ParameterType == typeof(CancellationToken)) return cancellationToken;

          return services.GetRequiredService(parameter.ParameterType);
        });

        var handler = Method.IsStatic ? null : services.GetRequiredService<THandler>();
        var result  = Method.Invoke(handler, arguments.ToArray());

        return result is Task ? result : Task.FromResult(result);
      }
    }
  }
}