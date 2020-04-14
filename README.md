# DurableTask-Hosting

![.NET Core Build](https://github.com/jviau/durabletask-hosting/workflows/.NET%20Core/badge.svg)

A [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/) wrapper around the [azure/durabletask](https://github.com/azure/durabletask) framework.

## Getting Started

See [Samples](./samples/DurableTask.Samples) for a quick start example.

1. Add nuget package: `dotnet add package Ippetad.DurableTask.Hosting`
2. Add to your host builder:

``` CSharp
Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        // Can configure orchestrations, activities, and middleware in the service
        // container with any scope desired.
        services.AddSingleton<MySingletonOrchestration>();
        services.AddScoped<MyScopedMiddleware>(); // must implement "ITaskMiddleware"
    })
    .ConfigureTaskHubWorker((context, builder) =>
    {
        // add orchestration service
        builder.WithOrchestrationService(new LocalOrchestrationService());

        // add orchestration directly _not_ in service container. Will be treated as transient.
        builder.AddOrchestration<MyTransientOrchestration>();

        // will be fetched from service provider.
        builder.AddOrchestration<MySingletonOrchestration>();

        // will be fetched from service provider.
        builder.UseOrchestrationMiddleware<MyScopedMiddleware>();

        // same as orchestration: can be part of the services or not.
        builder.AddActivity<MyTransientActivity>();
    })
    .RunConsoleAsync(); // starts the worker.
```

All orchestrations, activities, and middleware will now be constructed via dependency injection.
