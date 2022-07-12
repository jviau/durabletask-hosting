# Release 2.1.6-preview

- Fix `ObjectDisposedException` race condition during orchestration middleware
  - The fix was to remove the attempt to re-use `IServiceProvider` during sessions of the same orchestration.
  - The above would only impact if the DTFx orchestration/activity was registered with "Scoped" lifetime.
  - Doing the above is **strongly not recommended**. It would lead to undeterministic statefulness of your orchestration or activity.
- Fixed generic type parsing to properly handle nested generics when there are multiple generic arguments.
- Fix `WrapperOrchestrationContext.IsReplaying` not properly updating.
