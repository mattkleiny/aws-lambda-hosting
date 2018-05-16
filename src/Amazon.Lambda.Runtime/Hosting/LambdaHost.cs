﻿using System;
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
	// TODO: consider functional execution instead of handler classes
	//			 i.e. ExecuteAsync(object input, ILambdaContext context, IConfiguration configuration, AmazonSQSClient sqs);

	public sealed class LambdaHost
	{
		private readonly object startup;

		internal LambdaHost(object startup)
		{
			Check.NotNull(startup, nameof(startup));

			this.startup = startup;
		}

		public Task<object> ExecuteAsync(object input, ILambdaContext context)
		{
			Check.NotNull(context, nameof(context));

			// TODO: discover the source environment here, somehow
			var environment   = new HostingEnvironment();
			var configuration = new ConfigurationBuilder().Build();

			using (var services = Conventions.ConfigureServices(startup, context))
			{
				Conventions.ConfigureEnvironment(startup, environment, services, configuration);

				// TODO: resolve and execute the handler
				throw new NotImplementedException();
			}
		}

		private sealed class HostingEnvironment : IHostingEnvironment
		{
			// TODO: implement these
			public string ApplicationName { get; } = "Test";
			public string EnvironmentName { get; } = "Development";
		}
	}
}