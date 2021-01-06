# Release 2.0-preview

## Breaking Changes
- Refactor of ITaskHubWorkerBuilder interface and extension methods.
- Descriptors base classes removed.

## Additions
- Can now call `AddTaskHubWorker`/`ConfigureTaskHubWorker` multiple times, same builder will be used.
- Can insert middleware at desired place, remove existing middleware, etc.
- DI ObjectFactory used for improved performance
- Support for func middleware and interface activities.
- Update to DTFx 2.4.0, add `ILoggerFactory` while building.
