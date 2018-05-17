﻿using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Amazon.Lambda.Hosting
{
  public interface ILambdaHandler
  {
    Task<object> ExecuteAsync(object input, ILambdaContext context);
  }
}