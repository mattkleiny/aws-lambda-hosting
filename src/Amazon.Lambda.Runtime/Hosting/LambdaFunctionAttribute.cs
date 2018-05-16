using System;
using JetBrains.Annotations;

namespace Amazon.Lambda.Hosting
{
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