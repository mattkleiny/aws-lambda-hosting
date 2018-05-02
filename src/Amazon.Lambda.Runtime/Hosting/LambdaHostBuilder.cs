using System;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Hosting
{
	/// <summary>
	/// Configures a <see cref="IDisposable"/> for AWS Lambda handler execution.
	/// <para/>
	/// The hosting configuration mimics the ASP.NET Core initialization mechanism,
	/// for it's flexibility and familiarity, and permits execution of handlers
	/// either via a testing harness, in-situ or from the AWS production runtime.
	/// </summary>
	public sealed class LambdaHostBuilder
	{
		internal object         Startup       { get; private set; }
		internal ILambdaContext LambdaContext { get; private set; }

		public LambdaHostBuilder UseStartup<TStartup>()
			where TStartup : class, new()
		{
			Startup = new TStartup();
			return this;
		}

		/// <summary>
		/// Provides the <see cref="ILambdaContext"/> to pass through to handlers
		/// and the DI system. If no context is provided, a mock one will be created
		/// that mimics the AWS Lambda environment as best as possible.
		/// </summary>
		public LambdaHostBuilder UseLambdaContext(ILambdaContext context)
		{
			Check.NotNull(context, nameof(context));

			LambdaContext = context;
			return this;
		}

		public LambdaHost Build()
		{
			Check.NotNull(Startup, nameof(Startup));

			return LambdaHost.New(Startup, LambdaContext);
		}
	}
}