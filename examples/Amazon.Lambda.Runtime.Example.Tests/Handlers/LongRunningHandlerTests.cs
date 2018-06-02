using System.Threading.Tasks;
using Amazon.Lambda.Runtime.Example.Handlers;
using Amazon.Lambda.Testing;
using Xunit;

namespace Amazon.Lambda.Runtime.Example.Tests.Handlers
{
  public class LongRunningHandlerTests : HandlerTestCase
  {
    [Fact]
    public async Task it_should_execute_with_cancellation()
    {
      var handler = Fixture
        .GetHandler<LongRunningHandler>()
        .WithTimeLimit(2.Seconds());

      await Assert.ThrowsAsync<TaskCanceledException>(async () =>
      {
        await handler.SendS3EventAsync();
      });
    }
  }
}