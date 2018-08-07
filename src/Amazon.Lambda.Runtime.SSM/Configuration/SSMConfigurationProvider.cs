using System;
using System.Collections.Generic;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;

namespace Amazon.Lambda.Configuration
{
  /// <summary>A <see cref="IConfigurationProvider"/> that talks to <see cref="IAmazonSimpleSystemsManagement"/>.</summary>
  internal sealed class SSMConfigurationProvider : ConfigurationProvider
  {
    private readonly SSMConfigurationSource source;

    public SSMConfigurationProvider(SSMConfigurationSource source)
    {
      this.source = source;

      source.ShouldReload += Reload;
    }

    public override void Load() => Reload();

    /// <summary>Reloads the data for this provider from it's source.</summary>
    private void Reload()
    {
      try
      {
        Data.Clear();

        foreach (var parameter in FetchParameters())
        {
          Data.Add(Canonicalize(parameter.Name), parameter.Value);
        }
      }
      catch (Exception e)
      {
        source.NotifyException(e);
      }
      finally
      {
        OnReload();
      }
    }

    /// <summary>Fethces all the <see cref="Parameter"/>s from the base path, recursively.</summary>
    /// TODO: make this recursive or something, the imperative-ness is hurting my eyes.
    private IEnumerable<Parameter> FetchParameters()
    {
      using (var client = source.BuildClient())
      {
        string nextToken = null;

        while (true)
        {
          var task = client.GetParametersByPathAsync(new GetParametersByPathRequest
          {
            Path           = source.BasePath,
            MaxResults     = 10,
            Recursive      = true,
            WithDecryption = true,
            NextToken      = nextToken
          });

          var response = task.Result;

          foreach (var parameter in response.Parameters)
          {
            yield return parameter;
          }

          if (!string.IsNullOrEmpty(response.NextToken))
          {
            nextToken = response.NextToken;
          }
          else
          {
            break;
          }
        }
      }
    }

    /// <summary>Canonicalizes the given key name into something that can be used from <see cref="IConfiguration"/>.</summary>
    private string Canonicalize(string key) => key.Replace(source.BasePath, "").TrimStart('/').Replace("/", ":");
  }
}