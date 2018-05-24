using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Hosting;

namespace Amazon.Lambda.Hosting
{
  /// <summary>A dispatcher for functional Lambda invocation with injected parameters.</summary>
  internal sealed class FunctionalDispatcher<TTarget> : ILambdaHandler
  {
    private readonly IHost            host;
    private readonly TargetFunction[] functions;

    public FunctionalDispatcher(IHost host)
    {
      Check.NotNull(host, nameof(host));

      this.host = host;

      // TODO: share this logic with the hosting extensions
      functions = typeof(TTarget)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(method => method.GetCustomAttribute<LambdaFunctionAttribute>() != null)
        .Select(method => new TargetFunction(method, method.GetCustomAttribute<LambdaFunctionAttribute>().FunctionName))
        .ToArray();
    }

    public Task<object> ExecuteAsync(object input, ILambdaContext context)
    {
      Check.NotNull(context, nameof(context));

      foreach (var function in functions)
      {
        if (context.FunctionName.Equals(function.FunctionName, StringComparison.OrdinalIgnoreCase))
        {
          return function.Invoke(input, context, host.Services);
        }
      }
      
      throw new InvalidOperationException($"Unable to resolve functional handler for {context.FunctionName}");
    }

    /// <summary>A wrapper for a functional invocation target.</summary>
    private sealed class TargetFunction
    {
      public TargetFunction(MethodInfo method, string functionName)
      {
        Method       = method;
        FunctionName = functionName;
      }

      public MethodInfo Method       { get; }
      public string     FunctionName { get; }

      public Task<object> Invoke(object input, ILambdaContext context, IServiceProvider services)
      {
        // TODO: discover all of the parameters to the method, use this to request services from the provider
        // TODO: invoke the method with the input/context parameters, if required
        // TODO: cast up/down from an asynchronous version, if required
        
        throw new NotImplementedException();
      }
    }
  }
}