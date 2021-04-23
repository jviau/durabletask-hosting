# Release 2.1.2-preview

- Consume `IOrchestrationService` via dependency injection.
  - `WithOrchestrationService` now adds to `IServiceCollection` as singleton.
  - `AddClient` now first looks for `IOrchestrationServiceClient`, then falls back to casting `IOrchestrationService`.
  - `ITaskHubWorkerBuilder.OrchestrationService` marked as obsolete.
