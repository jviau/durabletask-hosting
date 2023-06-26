# Vio.DurableTask.DependencyInjection

Enables usage of [dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection) with DurableTask Framework.

## Getting Started

See [Vio.DurableTask.Hosting](https://www.nuget.org/packages/Vio.DurableTask.Hosting) for using this package with the .NET generic host builder.

## Features

- Registration of `TaskActivity` and `TaskOrchestration` to DI container, with lifetime control (Singleton/Scoped/Transient).
  - Activities and orchestrations not explicitly added to DI container are transient by default.
- Dependency-injected middleware for both activity or orchestration
  - `<ITaskHubWorkerBuilder>.UseOrchestrationMiddleware<TMiddleware>()`
  - `<ITaskHubWorkerBuilder>.UseActivityMiddleware<TMiddleware>()`
  - Middleware not explicitly added to DI container are transient by default.
- Builder style configuration via `.ConfigureTaskHubWorker`

## Service Scope

A new `IServiceScope` is created for the duration of every `OrchestrationInstance` run. This scope will be used for all actions, middleware, and the orchestration itself and disposed after both the middleware & orchestration pipeline has finished execution. Scopes are not preserved between runs of the same `OrchestrationInstance`.
