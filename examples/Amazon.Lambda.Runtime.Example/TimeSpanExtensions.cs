using System;

namespace Amazon.Lambda.Runtime.Example
{
  public static class TimeSpanExtensions
  {
    public static TimeSpan Milliseconds(this int amount) => TimeSpan.FromMilliseconds(amount);
    public static TimeSpan Seconds(this int amount)      => TimeSpan.FromSeconds(amount);
    public static TimeSpan Minutes(this int amount)      => TimeSpan.FromMinutes(amount);
    public static TimeSpan Hours(this int amount)        => TimeSpan.FromHours(amount);
    public static TimeSpan Days(this int amount)         => TimeSpan.FromDays(amount);
  }
}