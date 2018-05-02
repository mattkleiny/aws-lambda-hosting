using JetBrains.Annotations;

namespace Amazon.Lambda.Services
{
	[UsedImplicitly]
	public sealed class TestService : ITestService
	{
		public string GetMessage()
		{
			return "Hello, World!";
		}
	}
}