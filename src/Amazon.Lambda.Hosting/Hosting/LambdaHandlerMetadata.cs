using System;
using System.Linq;
using System.Reflection;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Encapsulates metadata for a registered <see cref="ILambdaHandler"/>.</summary>
  public sealed class LambdaHandlerMetadata
  {
    internal static LambdaHandlerMetadata ForHandler<THandler>(string functionName)
      where THandler : class, ILambdaHandler
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      var method = typeof(THandler).GetMethod(nameof(ILambdaHandler.ExecuteAsync));
      var (inputType, outputType) = ExtractTypes(method);

      return new LambdaHandlerMetadata
      {
        HandlerType  = typeof(THandler),
        FunctionName = functionName,
        FriendlyName = typeof(THandler).Name,
        InputType    = inputType,
        OutputType   = outputType
      };
    }

    internal static LambdaHandlerMetadata ForFunction<THandler>(FunctionalDispatcher<THandler>.TargetFunction function)
      where THandler : class
    {
      Check.NotNull(function, nameof(function));

      var (inputType, outputType) = ExtractTypes(function.Method);

      return new LambdaHandlerMetadata
      {
        HandlerType  = typeof(FunctionalDispatcher<THandler>),
        FunctionName = function.FunctionName,
        FriendlyName = function.FriendlyName,
        InputType    = inputType,
        OutputType   = outputType
      };
    }

    // TODO: fix this up

    private static (Type inputType, Type outputType) ExtractTypes(MethodInfo method)
    {
      var inputType  = method.GetParameters().FirstOrDefault()?.ParameterType;
      var outputType = method.ReturnType;

      return (inputType, outputType);
    }

    private LambdaHandlerMetadata()
    {
    }

    public Type   HandlerType  { get; private set; }
    public string FunctionName { get; private set; }
    public string FriendlyName { get; private set; }
    public Type   InputType    { get; private set; }
    public Type   OutputType   { get; private set; }

    private bool Equals(LambdaHandlerMetadata other)
    {
      return string.Equals(FunctionName, other.FunctionName) && HandlerType == other.HandlerType;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is LambdaHandlerMetadata registration && Equals(registration);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (FunctionName.GetHashCode() * 397) ^ HandlerType.GetHashCode();
      }
    }
  }
}