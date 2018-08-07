[![aws-toolkit MyGet Build Status](https://www.myget.org/BuildSource/Badge/aws-toolkit?identifier=9cf127f4-ad3d-42b7-9e0f-7a3e352fc864)](https://www.myget.org/)
# AWS Lambda Hosting

A mechanism for bootstrapping and integration testing .NET Core based AWS Lambda functions.

## Features

* The ability to bootstrap lambda functions either directly, from a CLI, from a test case or from the AWS Lambda environment.
* A configuration mechanism using ASP.NET Core's [Generic Host Builder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1),
  that is very similar to ASP.NET Core's WebHostBuilder; this aids in familiarity and reducing the conceptual weight of the project.
* Extensions for interacting with common AWS services; e.g. S3, Dynamo, CloudWatch, Kinesis, etc.

## Benchmarks

A project like this will add overhead to your lambda invocations. If your lambda is primarily distributed, it's usually dwarfed by whatever I/O you'll be performing.

Below are the benchmarks run on my laptop with various caching models, compared with a baseline that doesn't use the host at all.

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.16299.431 (1709/FallCreatorsUpdate/Redstone3)
Intel Core i7-7820HQ CPU 2.90GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
Frequency=2835936 Hz, Resolution=352.6173 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                 Method |         Mean |         Error |       StdDev |
|----------------------- |-------------:|--------------:|-------------:|
| WithTransientHostAsync | 546,120.5 ns | 13,907.897 ns | 39,680.01 ns |
|    WithCachedHostAsync |   2,020.8 ns |     40.027 ns |     91.16 ns |
|       WithoutHostAsync |     359.3 ns |      7.344 ns |     16.88 ns |
