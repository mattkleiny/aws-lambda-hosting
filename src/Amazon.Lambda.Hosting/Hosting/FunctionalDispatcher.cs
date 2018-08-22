using System;
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
        .Select(method => new TargetFunction(method, method.GetCustomAttribute<LambdaFunctionAttribute>().FunctionName))
        .ToArray();

    public FunctionalDispatcher(IHost host, IOptions<HostingOptions> hostingOptions, TargetFunction[] functions)
    {
      Check.NotNull(host,      nameof(host));
      Check.NotNull(functions, nameof(functions));

      this.host      = host;
      this.functions = functions;

      isMatch = hostingOptions.Value.MatchingStrategy;
    }

    public async Task<object> ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      Check.NotNull(context, nameof(context));

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
      private readonly MethodInfo      method;
      private readonly ParameterInfo[] parameters;

      public TargetFunction(MethodInfo method, string functionName)
      {
        Check.NotNull(method, nameof(method));
        Check.NotNullOrEmpty(functionName, nameof(functionName));

        this.method = method;
        parameters  = method.GetParameters().ToArray();

        FunctionName = functionName;
        Metadata     = LambdaHandlerMetadata.ForFunction(this);
      }

      public string FunctionName { get; }
      public string FriendlyName => method.Name;

      /// <summary>A <see cref="LambdaHandlerMetadata"/> just to keep the matching strategies consistent.</summary>
      internal LambdaHandlerMetadata Metadata { get; }

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

        var handler = method.IsStatic ? null : services.GetRequiredService<THandler>();
        var result  = method.Invoke(handler, arguments.ToArray());

        return result is Task ? result : Task.FromResult(result);
      }
    }
  }
}