[![aws-lambda-runtime MyGet Build Status](https://www.myget.org/BuildSource/Badge/aws-lambda-runtime?identifier=0c851967-560e-49a9-9705-ba70883a76d9)](https://www.myget.org/)
# AWS Lambda Runtime

A mechanism for bootstrapping and integration testing .NET Core based AWS Lambda functions.

Features:

* The ability to bootstrap lambda functions either in-situ (from abitrary code), from a test case or from the AWS Lambda
  environment.
* Configuration mechanisms using ASP.NET Core's [Generic Host Builder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1); 
  this aids in familiarity and reducing the conceptual weight of the project.
* Managed configuration for common AWS services; e.g. S3, Dynamo, CloudWatch, Kinesis, etc. This will allow for a very 
  simple reconfiguration mechanism for these services when in a mocked/test environment.
* An extension for a CLI menu to simplify projects with many individual lambda handlers. Each handler is made available 
  to the menu either via manual configuration or via automatic discovery over some data-source (such as CloudFormation).