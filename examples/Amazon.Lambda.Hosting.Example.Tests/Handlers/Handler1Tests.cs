using System.Threading.Tasks;
using Amazon.Lambda.Hosting.Example.Handlers;
using Xunit;

namespace Amazon.Lambda.Hosting.Example.Tests.Handlers
{
  public class Handler1Tests : HandlerTestCase
  {
    [Fact]
    public async Task it_should_execute_ok()
    {
      var handler = Fixture.GetHandler<Handler1>();
      var result  = await handler.ExecuteAsync(null);

      Assert.NotNull(result);
    }
  }
}