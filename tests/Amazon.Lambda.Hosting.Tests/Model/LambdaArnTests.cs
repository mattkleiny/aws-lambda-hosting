using NUnit.Framework;

namespace Amazon.Lambda.Hosting.Tests.Model
{
  public class LambdaArnTests
  {
    [Test]
    public void it_should_produce_a_valid_ARN_from_components()
    {
      var arn = new LambdaArn(RegionEndpoint.APSoutheast2, 123456789, "test-function");

      Assert.AreEqual("arn:aws:lambda:ap-southeast-2:123456789:function:test-function", arn.ToString());
    }

    [Test]
    public void it_should_produce_a_valid_ARN_from_components_and_qualifier()
    {
      var arn = new LambdaArn(RegionEndpoint.APSoutheast2, 123456789, "test-function", "1.0");

      Assert.AreEqual("arn:aws:lambda:ap-southeast-2:123456789:function:test-function:1.0", arn.ToString());
    }

    [Theory]
    [TestCase("arn:aws:lambda:ap-southeast-2:123456789:function:test-function")]
    [TestCase("arn:aws:lambda:ap-southeast-2:123456789:function:test-function:1.0")]
    public void it_should_be_able_to_parse_simple_arns(string arn) => Assert.NotNull(LambdaArn.Parse(arn));
  }
}