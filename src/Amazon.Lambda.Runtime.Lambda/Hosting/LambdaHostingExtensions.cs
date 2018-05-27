using Amazon.Lambda.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Amazon.Lambda.Hosting
{
	public static class LambdaHostingExtensions
	{
		/// <summary>Adds Lambda support to the host.</summary>
		public static IHostBuilder UseLambda(this IHostBuilder builder)
		{
			return builder.ConfigureServices((context, services) =>
			{
				services.AddScoped<IAmazonLambda>(provider =>
				{
					var options = provider.GetRequiredService<IOptions<HostingOptions>>().Value;
					var config  = new AmazonLambdaConfig();

					if (options.RedirectTable.Contains(WellKnownService.Lambda))
					{
						config.ServiceURL = options.RedirectTable[WellKnownService.Lambda].ToString();
					}
          
					if (options.AWS.DefaultEndpoint != null)
					{
						config.RegionEndpoint = options.AWS.DefaultEndpoint;
					}

					if (!string.IsNullOrEmpty(options.AWS.AccessKey) || !string.IsNullOrEmpty(options.AWS.SecretKey))
					{
						return new AmazonLambdaClient(options.AWS.AccessKey, options.AWS.SecretKey, config);
					}

					return new AmazonLambdaClient(config);
				});
			});
		}
	}
}