using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Services;
using JetBrains.Annotations;

namespace Amazon.Lambda.Handlers
{
	[UsedImplicitly]
	public sealed class Handler2 : ILambdaHandler
	{
		private readonly ITestService collaborator;

		public Handler2(ITestService collaborator)
		{
			Check.NotNull(collaborator, nameof(collaborator));

			this.collaborator = collaborator;
		}

		public Task<object> ExecuteAsync(ILambdaContext context)
		{
			return Task.FromResult<object>(collaborator.GetMessage());
		}
	}
}