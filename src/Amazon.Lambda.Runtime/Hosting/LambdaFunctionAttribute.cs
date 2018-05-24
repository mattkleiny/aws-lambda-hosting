using System;
using JetBrains.Annotations;

namespace Amazon.Lambda.Hosting
{
  /// <summary>
  /// Denotes the associated <see cref="ILambdaHandler"/> is mapped to the given Lambda <see cref="FunctionName"/>.
  /// </summary>
  [MeansImplicitUse]
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class LambdaFunctionAttribute : Attribute
  {
    public LambdaFunctionAttribute(string functionName)
    {
      FunctionName = functionName;
    }

    public string FunctionName { get; }
  }
}