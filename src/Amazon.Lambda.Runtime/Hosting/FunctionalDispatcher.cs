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

    public Task<object> ExecuteAsync(object input, ILambdaContext context, CancellationToken cancellationToken)
    {
      Check.NotNull(context, nameof(context));

      foreach (var function in functions)
      {
        if (isMatch(function.Registration, input, context))
        {
          return function.Invoke(input, context, host.Services, cancellationToken);
        }
      }

      throw new UnresolvedHandlerException($"Unable to resolve functional handler for {context.FunctionName}");
    }

    /// <summary>A wrapper for a functional invocation target.</summary>
    internal sealed class TargetFunction
    {
      private readonly MethodInfo                 method;
      private readonly ParameterInfo[]            parameters;
      private readonly Func<object, Task<object>> extractor;

      public TargetFunction(MethodInfo method, string functionName)
      {
        Check.NotNull(method, nameof(method));
        Check.NotNullOrEmpty(functionName, nameof(functionName));

        this.method = method;
        parameters  = method.GetParameters().ToArray();

        FunctionName = functionName;
        Registration = new LambdaHandlerRegistration(
          functionName: functionName,
          typeof(THandler),
          friendlyName: method.Name
        );

        extractor = CreateResultExtractor(method);
      }

      public string FunctionName { get; }
      public string FriendlyName => method.Name;

      /// <summary>A <see cref="LambdaHandlerRegistration"/> just to keep the matching strategies consistent.</summary>
      internal LambdaHandlerRegistration Registration { get; }

      /// <summary>Invokes the underlying method, injecting it's parameters as required.</summary>
      public async Task<object> Invoke(object input, ILambdaContext context, IServiceProvider services, CancellationToken cancellationToken)
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

        // static methods don't require a 'this' parameter, and can simply specify null
        var handler = !method.IsStatic ? services.GetRequiredService<THandler>() : null;
        var result  = method.Invoke(handler, arguments.ToArray());

        return await extractor(result);
      }

      /// <summary>
      /// Builds a delegate which is specialized for the result type of our handler method invocation, capable of extracting
      /// the relevant result type from the returned method invocation and awaiting/unpacking as necessary.
      /// </summary>
      /// <remarks>
      /// The TResult in <see cref="Task{TResult}"/> is invariant, which means we can't simply check to see if the result
      /// 'is Task of object' and await the result.  
      /// </remarks>
      private static Func<object, Task<object>> CreateResultExtractor(MethodInfo method)
      {
        // check to see if the result is a Task, and extract it's type parameter if necessary.
        Type ExtractResultType(Type type)
        {
          if (typeof(Task).IsAssignableFrom(type) && type.IsGenericType)
          {
            return type.GenericTypeArguments[0];
          }

          return type;
        }

        var resultType = ExtractResultType(method.ReturnType);

        var genericMethod     = typeof(TargetFunction).GetMethod(nameof(ExtractResultAsync), BindingFlags.Static | BindingFlags.NonPublic);
        var specializedMethod = genericMethod.MakeGenericMethod(resultType);

        return (Func<object, Task<object>>) specializedMethod.CreateDelegate(typeof(Func<object, Task<object>>));
      }

      /// <summary>Extracts the result of the given <see cref="output"/> with the given expected inner type <see cref="T"/>.</summary>
      private static async Task<object> ExtractResultAsync<T>(object output)
      {
        switch (output)
        {
          case Task<T> task:
            return await task;

          case Task task:
            await task;
            return default;

          default:
            return output;
        }
      }
    }
  }
}