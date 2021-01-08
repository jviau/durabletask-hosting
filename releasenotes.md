# Release 2.0.2-preview

## Breaking Changes
- Refactor of ITaskHubWorkerBuilder interface and extension methods.
- Descriptors base classes removed.

## Additions
- Can now call `AddTaskHubWorker`/`ConfigureTaskHubWorker` multiple times, same builder will be used.
- Can insert middleware at desired place, remove existing middleware, etc.
- DI ObjectFactory used for improved performance
- Support for func middleware and interface activities.
- Update to DTFx 2.4.0, add `ILoggerFactory` while building.

# Release 2.0.7-preview

# Additions
- Added support for open generics with [#14](https://github.com/jviau/durabletask-hosting/pull/14).
   - When enqueing a closed form for an open generic, use `Type.FullName` as the orchestration/activity name.
   - `OrchestrationContext` will use `Type.FullName` by default.
   - `TaskHubClient` will not - it must be explicitly provided.
