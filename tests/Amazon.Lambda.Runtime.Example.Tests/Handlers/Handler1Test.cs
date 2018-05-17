using System.Threading.Tasks;
using Amazon.Lambda.Handlers;
using Amazon.Lambda.Hosting;
using Amazon.Lambda.Testing;
using Xunit;

namespace Amazon.Lambda.Runtime.Example.Tests.Handlers
{
  public class Handler1Test
  {
    private LambdaTestFixture Fixture { get; } = new LambdaTestFixture(
      new LambdaHostBuilder()
        .UseStartup<ExampleStartup>()
        .WithHandler<Handler1>()
    );

    [Fact]
    public async Task it_should_execute_with_some_sns_event()
    {
      var response = await Fixture
        .GetHandler<Handler1>()
        .SendSnsEventAsync(new {Message = "Hello, World!"});

      Assert.NotNull(response);
    }
  }
}