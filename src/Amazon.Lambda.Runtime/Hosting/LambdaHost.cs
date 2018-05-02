using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda.Hosting
{
	// TODO: add redirect table for internal AWS services 
	//			 i.e. [Service.DynamoDB] => "https://localhost:3000/"
	// TODO: add confiurable environment source property
	//			 i.e. .DiscerneEnvironmentFrom("SOME_ENVIRONMENT_VARIABLE")
	// TODO: add testing harness with easy payload pass-through
	//       i.e. Fixture.GetLambda<SomeLambd>().SendS3Event() or .SendSNSEvent()
	// TODO: add helpers for step functions, perhaps?
	// TODO: bootstrap the handler based on the lambda context function name, instead?

	public sealed class LambdaHost : IDisposable
	{
		private readonly HostingEnvironment environment;
		private readonly ServiceProvider    provider;
		private readonly IConfigurationRoot configuration;
		private readonly ILambdaContext     context;

		/// <summary>
		/// Instantiates a new <see cref="LambdaHost"/> for the given startup
		/// handler object using the given optional <see cref="ILambdaContext"/>;
		/// if the context is not provided, a mock one will be instantiated.
		/// </summary>
		internal static LambdaHost New(object startup, ILambdaContext context)
		{
			Check.NotNull(startup, nameof(startup));

			var services    = new ServiceCollection();
			var environment = new HostingEnvironment(); // TODO: discover the source environment here, somehow

			ReflectiveConfigurer.ConfigureServices(startup, services);

			if (context != null)
			{
				services.AddSingleton(context);
			}

			var provider      = services.BuildServiceProvider();
			var configuration = new ConfigurationBuilder().Build();

			try
			{
				ReflectiveConfigurer.ConfigureEnvironment(startup, environment, provider, configuration);

				return new LambdaHost(environment, provider, configuration, context);
			}
			catch
			{
				provider.Dispose();

				throw;
			}
		}

		private LambdaHost(HostingEnvironment environment, ServiceProvider provider, IConfigurationRoot configuration, ILambdaContext context)
		{
			Check.NotNull(environment,   nameof(environment));
			Check.NotNull(provider,      nameof(provider));
			Check.NotNull(configuration, nameof(configuration));

			this.environment   = environment;
			this.provider      = provider;
			this.configuration = configuration;
			this.context       = context ?? new LambdaContext();
		}

		public Task<object> ExecuteAsync<THandler>()
			where THandler : class, ILambdaHandler
		{
			using (var scope = provider.CreateScope())
			{
				var handler = scope.ServiceProvider.GetService<THandler>() ?? Activator.CreateInstance<THandler>();

				return handler.ExecuteAsync(context);
			}
		}

		public void Dispose()
		{
			provider.Dispose();
		}

		private sealed class HostingEnvironment : IHostingEnvironment
		{
			// TODO: implement these
			public string ApplicationName { get; } = "Test";
			public string EnvironmentName { get; } = "Development";
		}

		private sealed class LambdaContext : ILambdaContext
		{
			public string           AwsRequestId       { get; }
			public IClientContext   ClientContext      { get; }
			public string           FunctionName       { get; }
			public string           FunctionVersion    { get; }
			public ICognitoIdentity Identity           { get; }
			public string           InvokedFunctionArn { get; }
			public ILambdaLogger    Logger             { get; }
			public string           LogGroupName       { get; }
			public string           LogStreamName      { get; }
			public int              MemoryLimitInMB    { get; }
			public TimeSpan         RemainingTime      { get; }
		}
	}
}