using System;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Encapsulates a registration for a <see cref="ILambdaHandler"/> with it's associated <see cref="FunctionName"/>.</summary>
  internal sealed class LambdaHandlerRegistration
  {
    public LambdaHandlerRegistration(string functionName, Type handlerType)
    {
      Check.NotNullOrEmpty(functionName, nameof(functionName));
      Check.NotNull(handlerType, nameof(handlerType));

      FunctionName = functionName;
      HandlerType  = handlerType;
    }

    public string FunctionName { get; }
    public Type   HandlerType  { get; }

    private bool Equals(LambdaHandlerRegistration other)
    {
      return string.Equals(FunctionName, other.FunctionName) && HandlerType == other.HandlerType;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is LambdaHandlerRegistration registration && Equals(registration);
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