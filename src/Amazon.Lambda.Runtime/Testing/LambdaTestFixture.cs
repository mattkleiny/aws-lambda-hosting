using System;
using Amazon.Lambda.Hosting;

namespace Amazon.Lambda.Testing
{
	/// <summary>
	/// A mocked <see cref="IDisposable"/> for testing purposes.
	/// <para/>
	/// Builds a dummy AWS Lambda environment and provides hooks for execution of
	/// <see cref="ILambdaHandler"/>s from test cases.
	/// </summary>
	public sealed class LambdaTestFixture
	{
		public LambdaTestFixture(LambdaHostBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			throw new NotImplementedException();
		}
	}
}