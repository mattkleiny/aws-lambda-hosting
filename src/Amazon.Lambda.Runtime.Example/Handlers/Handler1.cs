using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Services;
using JetBrains.Annotations;

namespace Amazon.Lambda.Handlers
{
	[UsedImplicitly]
	public sealed class Handler1 : ILambdaHandler
	{
		private readonly ITestService collaborator;

		public Handler1(ITestService collaborator)
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