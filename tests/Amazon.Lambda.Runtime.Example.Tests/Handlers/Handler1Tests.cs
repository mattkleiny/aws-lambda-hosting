using System.Threading.Tasks;
using Amazon.Lambda.Handlers;
using Xunit;

namespace Amazon.Lambda.Runtime.Example.Tests.Handlers
{
  public class Handler1Tests : HandlerTestCase
  {
    [Fact]
    public async Task it_should_execute_ok()
    {
      var handler = Fixture.GetHandler<Handler1>();
      var result  = await handler.ExecuteAsync("Hello, there");

      Assert.NotNull(result);
    }
  }
}