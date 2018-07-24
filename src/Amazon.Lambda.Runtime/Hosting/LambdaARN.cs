using System.Text.RegularExpressions;

namespace Amazon.Lambda.Hosting
{
  /// <summary>Encapsulates a ARN for a lambda function in AWS and encapsulates it's individual components.</summary>
  public sealed class LambdaARN
  {
    private static readonly Regex Regex = new Regex(@"^arn:aws:lambda:([a-zA-Z0-9\-]+):([0-9]+):function:([a-zA-Z0-9\-]+):?([a-zA-Z0-9\-\.]*)$");

    /// <summary>Parses a <see cref="LambdaARN"/> from the given string.</summary>
    public static LambdaARN Parse(string arn)
    {
      Check.NotNullOrEmpty(arn, nameof(arn));

      var match = Regex.Match(arn);
      if (!match.Success)
      {
        throw new InvalidLambdaARNException(arn);
      }

      var region       = RegionEndpoint.GetBySystemName(match.Groups[1].Value);
      var accountId    = long.Parse(match.Groups[2].Value);
      var functionName = match.Groups[3].Value;
      var qualifier    = match.Groups[4].Value;

      return new LambdaARN(region, accountId, functionName, qualifier);
    }

    public LambdaARN(RegionEndpoint region, long accountId, string functionName)
      : this(region, accountId, functionName, null)
    {
    }

    public LambdaARN(RegionEndpoint region, long accountId, string functionName, string qualifier)
    {
      Check.NotNull(region, nameof(region));
      Check.That(accountId > 0, "accountId > 0");
      Check.NotNullOrEmpty(functionName, nameof(functionName));

      Region       = region;
      AccountId    = accountId;
      FunctionName = functionName;
      Qualifier    = qualifier;
    }

    public RegionEndpoint Region       { get; }
    public long           AccountId    { get; }
    public string         FunctionName { get; }
    public string         Qualifier    { get; }

    public override string ToString()
    {
      var result = $"arn:aws:lambda:{Region.SystemName}:{AccountId}:function:{FunctionName}";

      if (!string.IsNullOrEmpty(Qualifier))
      {
        result += $":{Qualifier}";
      }

      return string.Intern(result);
    }

    private bool Equals(LambdaARN other)
    {
      return Equals(Region, other.Region)               &&
        AccountId == other.AccountId                    &&
        string.Equals(FunctionName, other.FunctionName) &&
        string.Equals(Qualifier,    other.Qualifier);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is LambdaARN arn && Equals(arn);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Region != null ? Region.GetHashCode() : 0;

        hashCode = (hashCode * 397) ^ AccountId.GetHashCode();
        hashCode = (hashCode * 397) ^ (FunctionName != null ? FunctionName.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (Qualifier    != null ? Qualifier.GetHashCode() : 0);

        return hashCode;
      }
    }
  }
}