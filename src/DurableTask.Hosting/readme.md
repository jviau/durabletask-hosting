# Vio.DurableTask.Hosting

Allows for configuring and running the `DurableTask.Core.TaskHubWorker` via an `IHostedService`.

## Getting Started

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

## Features

- Automatic start/stop of `TaskHubWorker` via `IHostedService`
- Builder style configuration via `.ConfigureTaskHubWorker`
