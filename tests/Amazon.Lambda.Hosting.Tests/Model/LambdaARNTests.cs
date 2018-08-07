using Amazon.Lambda.Model;
using Xunit;

namespace Amazon.Lambda.Hosting.Tests.Model
{
  public class LambdaARNTests
  {
    [Fact]
    public void it_should_produce_a_valid_ARN_from_components()
    {
      var arn = new LambdaARN(RegionEndpoint.APSoutheast2, 123456789, "test-function");

      Assert.Equal("arn:aws:lambda:ap-southeast-2:123456789:function:test-function", arn.ToString());
    }

    [Fact]
    public void it_should_produce_a_valid_ARN_from_components_and_qualifier()
    {
      var arn = new LambdaARN(RegionEndpoint.APSoutheast2, 123456789, "test-function", "1.0");

      Assert.Equal("arn:aws:lambda:ap-southeast-2:123456789:function:test-function:1.0", arn.ToString());
    }

    [Theory]
    [InlineData("arn:aws:lambda:ap-southeast-2:123456789:function:test-function")]
    [InlineData("arn:aws:lambda:ap-southeast-2:123456789:function:test-function:1.0")]
    public void it_should_be_able_to_parse_simple_arns(string arn) => Assert.NotNull(LambdaARN.Parse(arn));
  }
}