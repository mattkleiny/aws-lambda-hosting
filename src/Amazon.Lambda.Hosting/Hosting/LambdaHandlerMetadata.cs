using System;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Encapsulates metadata for a registered <see cref="ILambdaHandler"/>.</summary>
  public sealed class LambdaHandlerMetadata
  {
    // TODO: add input/output types

    internal static LambdaHandlerMetadata ForHandler<THandler>(string functionName)
      where THandler : class, ILambdaHandler
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      return new LambdaHandlerMetadata
      {
        HandlerType  = typeof(THandler),
        FunctionName = functionName,
        FriendlyName = typeof(THandler).Name,
      };
    }

    internal static LambdaHandlerMetadata ForFunction<THandler>(FunctionalDispatcher<THandler>.TargetFunction function)
      where THandler : class
    {
      Check.NotNull(function, nameof(function));

      return new LambdaHandlerMetadata
      {
        HandlerType  = typeof(FunctionalDispatcher<THandler>),
        FunctionName = function.FunctionName,
        FriendlyName = function.FriendlyName
      };
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