# Vio.DurableTask.Extensions

Extends `DurableTask.Core` with useful helpers, middleware, and new base classes.

## Getting Started

``` CSharp
<IHostBuilder>.ConfigureTaskHubWorker((context, builder) =>
{
    builder.AddDurableExtensions(<configure>);
});
```

## Summary

- Provides a mediator/ref pattern for invoking activities and orchestrations.
  - `IActivityRequest`, `IActivityRequest<TOutput>`
  - `IOrchestrationRequest`, `IOrchestrationRequest<TOutput>`
- Provides useful extension methods on `OrchestrationContext`
  - `context.RunAsync(TRequest)`, giving strongly typed input and output constraints.
  - `context.Delay(TimeSpan)`, convenient `Task.Delay`-like helper.
- Provides new activity and orchestration base classes
  - `ActivityBase`, `ActivityBase<TInput>` `ActivityBase<TInput, TOutput>`
  - `OrchestrationBase`, `OrchestrationBase<TInput>` `OrchestrationBase<TInput, TOutput>`
- Helpers setting custom `DataConverter`
  - `builder.AddDurableExtensions(options => options.DataConverter = <CustomConverter>);`
