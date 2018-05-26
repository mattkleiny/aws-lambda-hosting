using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A dispatcher for functional Lambda invocation with injected parameters.</summary>
  internal sealed class FunctionalDispatcher<THandler> : ILambdaHandler
  {
    private readonly IHost            host;
    private readonly TargetFunction[] functions;

    /// <summary>Discovers all of the <see cref="TargetFunction"/>s associated with <see cref="THandler"/>.</summary>
    public static TargetFunction[] DiscoverFunctions() => typeof(THandler)
      .GetMethods(BindingFlags.Public | BindingFlags.Instance)
      .Where(method => method.GetCustomAttribute<LambdaFunctionAttribute>() != null)
      .Select(method => new TargetFunction(method, method.GetCustomAttribute<LambdaFunctionAttribute>().FunctionName))
      .ToArray();

    public FunctionalDispatcher(IHost host, TargetFunction[] functions)
    {
      Check.NotNull(host,      nameof(host));
      Check.NotNull(functions, nameof(functions));

      this.host      = host;
      this.functions = functions;
    }

    public Task<object> ExecuteAsync(object input, ILambdaContext context, CancellationToken token)
    {
      Check.NotNull(context, nameof(context));

      foreach (var function in functions)
      {
        if (context.FunctionName.Equals(function.FunctionName, StringComparison.OrdinalIgnoreCase))
        {
          return function.Invoke(input, context, host.Services, token);
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
      }

      public string FunctionName { get; }
      public string FriendlyName => method.Name;

      /// <summary>Invokes the underlying method, injecting it's parameters as required.</summary>
      public Task<object> Invoke(object input, ILambdaContext context, IServiceProvider services, CancellationToken token)
      {
        IEnumerable<object> PopulateParameters()
        {
          foreach (var parameter in parameters)
          {
            if (parameter.ParameterType      == typeof(object)) yield return input;
            else if (parameter.ParameterType == typeof(ILambdaContext)) yield return context;
            else if (parameter.ParameterType == typeof(IServiceProvider)) yield return services;
            else if (parameter.ParameterType == typeof(CancellationToken)) yield return token;
            else yield return services.GetRequiredService(parameter.ParameterType);
          }
        }

        var handler = services.GetRequiredService<THandler>();
        var result  = method.Invoke(handler, PopulateParameters().ToArray());

        if (result is Task<object> task)
        {
          return task;
        }

        return Task.FromResult(result);
      }
    }
  }
}