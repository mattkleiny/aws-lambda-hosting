[![aws-lambda-runtime MyGet Build Status](https://www.myget.org/BuildSource/Badge/aws-lambda-runtime?identifier=0c851967-560e-49a9-9705-ba70883a76d9)](https://www.myget.org/)
# AWS Lambda Runtime

A mechanism for bootstrapping and integration testing .NET Core based AWS Lambda functions.

Features:

* The ability to bootstrap lambda functions either directly, from a CLI, from a test case or from the AWS Lambda environment.
* A configuration mechanism using ASP.NET Core's [Generic Host Builder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1).
* Extensions for interacting with common AWS services; e.g. S3, Dynamo, CloudWatch, Kinesis, etc.
