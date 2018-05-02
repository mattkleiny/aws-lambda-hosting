# AWS Lambda Runtime

A mechanism for bootstrapping and integration testing .NET Core based AWS Lambda functions.

Requirements:

* The ability to bootstrap lambda functions either in-situ (from abitrary code), from a test case or from the AWS Lambda
  environment.
* Configuration mechanisms very similar to ASP.NET Core's WebHostBuilder; this aids in familiarity and reducing the
  conceptual weight of the project.
* Managed configuration for common AWS services; e.g. S3, Dynamo, CloudWatch, Kinesis, etc. This will allow for a very 
  configuration mechanism for these services when in a mocked/test environment.

Extensions:

* An extension for docker integration with the ability to spin up a docker-compose environment around execution. This
  will allow for local testing of otherwise cloud-bound activities such as Dynamo or S3.
* An extension for a CLI menu to simplify projects with many individual lambda handlers. Each handler should be made
  available to the menu either via manual configuration or via automatic discovery over some data-source 
  (such as CloudFormation).