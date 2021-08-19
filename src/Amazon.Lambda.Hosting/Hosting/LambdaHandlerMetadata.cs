using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Encapsulates metadata for a registered <see cref="ILambdaHandler"/>.</summary>
  public sealed record LambdaHandlerMetadata(Type HandlerType, string FunctionName, string FriendlyName, Type? InputType, Type? OutputType)
  {
    internal static LambdaHandlerMetadata ForHandler<THandler>(string functionName)
      where THandler : class, ILambdaHandler
    {
      Debug.Assert(!string.IsNullOrEmpty(functionName), "!string.IsNullOrEmpty(functionName)");

      var method = typeof(THandler).GetMethod(nameof(ILambdaHandler.ExecuteAsync));
      if (method == null)
      {
        throw new Exception("Unable to locate ExecuteAsync method on ILambdaHandler; has the interface changed");
      }

      var (inputType, outputType) = ExtractTypes(method);

      return new LambdaHandlerMetadata(
        HandlerType: typeof(THandler),
        FunctionName: functionName,
        FriendlyName: typeof(THandler).Name,
        InputType: inputType,
        OutputType: outputType
      );
    }

    internal static LambdaHandlerMetadata ForFunction<THandler>(FunctionalDispatcher<THandler>.TargetFunction function)
      where THandler : class
    {
      var (inputType, outputType) = ExtractTypes(function.Method);

      return new LambdaHandlerMetadata(
        HandlerType: typeof(FunctionalDispatcher<THandler>),
        FunctionName: function.FunctionName,
        FriendlyName: function.FriendlyName,
        InputType: inputType,
        OutputType: outputType
      );
    }

    private static (Type? inputType, Type? outputType) ExtractTypes(MethodInfo method)
    {
      var inputType  = method.GetParameters().FirstOrDefault()?.ParameterType;
      var outputType = method.ReturnType;

      return (inputType, outputType);
    }
  }
}