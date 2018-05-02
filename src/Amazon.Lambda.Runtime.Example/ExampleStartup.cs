using System.Threading.Tasks;
using Amazon.Lambda.Handlers;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda
{
	public sealed class ExampleStartup
	{
		public static Task Main(string[] args)
		{
			var host = new LambdaHostBuilder()
				.UseStartup<ExampleStartup>()
				.Build();

			using (host)
			{
				return host.ExecuteAsync<Handler1>();
			}
		}

		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<ITestService, TestService>();
		}

		[UsedImplicitly]
		public void Configure(IHostingEnvironment environment, IConfiguration configuration)
		{
			if (environment.IsDevelopment())
			{
				var metrics = configuration.GetSection("Metrics");
			}
		}
	}
}