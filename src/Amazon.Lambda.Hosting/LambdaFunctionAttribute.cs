using System;
using JetBrains.Annotations;

namespace Amazon.Lambda
{
  /// <summary>Denotes the associated <see cref="ILambdaHandler"/> is mapped to the given lambda function name.</summary>
  [MeansImplicitUse]
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public sealed class LambdaFunctionAttribute : Attribute
  {
    public LambdaFunctionAttribute(string functionName)
    {
      FunctionName = functionName;
    }

    public string FunctionName { get; }
  }
}