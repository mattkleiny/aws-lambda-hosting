[![Build, Test and Package](https://github.com/mattkleiny/aws-lambda-hosting/actions/workflows/build-and-package.yml/badge.svg)](https://github.com/mattkleiny/aws-lambda-hosting/actions/workflows/build-and-package.yml)
# AWS Lambda Hosting

A mechanism for bootstrapping and integration testing .NET Core based AWS Lambda functions.

This utility provides the ability to bootstrap lambda functions either directly, from a CLI, from a test case or from the AWS Lambda environment.

It implements a configuration mechanism using .NET Core's [Generic Host Builder](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host);
it is is very similar to ASP.NET Core's `WebHostBuilder`, which this aids in familiarity and reducing the conceptual weight of the project.


