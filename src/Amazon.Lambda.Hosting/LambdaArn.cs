using System;
using System.Text.RegularExpressions;

namespace Amazon.Lambda
{
  /// <summary>Encapsulates a ARN for a lambda function in AWS and breaks down it's individual components.</summary>
  public sealed record LambdaArn(RegionEndpoint Region, long AccountId, string FunctionName, string? Qualifier = null)
  {
    private static readonly Regex Regex = new(@"^arn:aws:lambda:([a-zA-Z0-9\-]+):([0-9]+):function:([a-zA-Z0-9\-]+):?([a-zA-Z0-9\-\.]*)$");

    public static LambdaArn Parse(string arn)
    {
      if (!TryParse(arn, out var result))
      {
        throw new Exception($"Unable to parse {arn} into a lambda ARN");
      }

      return result;
    }

    public static bool TryParse(string arn, out LambdaArn result)
    {
      if (string.IsNullOrEmpty(arn))
      {
        result = default!;
        return false;
      }

      var match = Regex.Match(arn);
      if (!match.Success)
      {
        result = default!;
        return false;
      }

      if (!long.TryParse(match.Groups[2].Value, out var accountId))
      {
        result = default!;
        return false;
      }

      var region       = RegionEndpoint.GetBySystemName(match.Groups[1].Value);
      var functionName = match.Groups[3].Value;
      var qualifier    = match.Groups[4].Value;

      result = new LambdaArn(region, accountId, functionName, qualifier);
      return true;
    }

    public override string ToString()
    {
      var result = $"arn:aws:lambda:{Region.SystemName}:{AccountId}:function:{FunctionName}";

      if (!string.IsNullOrEmpty(Qualifier))
      {
        result += $":{Qualifier}";
      }

      return string.Intern(result);
    }
  }
}