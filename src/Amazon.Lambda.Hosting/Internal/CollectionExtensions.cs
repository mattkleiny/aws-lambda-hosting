using System;
using System.Collections.Generic;

namespace Amazon.Lambda.Internal
{
  internal static class CollectionExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> body)
    {
      foreach (var element in enumerable)
      {
        body(element);
      }
    }
  }
}