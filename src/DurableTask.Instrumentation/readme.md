# Vio.DurableTask.Instrumentation

This library bridges the existing DurableTask distributed tracing with OpenTelemetry SDK. See [DurableTask.Instrumentation.Samples](../../samples/DurableTask.Instrumentation.Samples/) for an example on what it provides.

## ⚠ Important: This library only works for DurableTask.AzureStorage orchestration service ⚠

## Emitted Spans

| Name | Kind | Description |
| - | - | - |
| `start_orchestration` | `Producer` | The `TaskHubClient` starting a new orchestration |
| `orchestration:{orchestration_name}@{orchestration_version}` | `Server` | `TaskHubWorker` running a `TaskOrchestration`. |
| `orchestration:{orchestration_name}` | `Client` | A `TaskOrchestration` waiting on a sub-`TaskOrchestration` |
| `activity:{activity_name}@{activity_version}` | `Server` | `TaskHubWorker` running a `TaskActivity`. |
| `activity:{activity_name}` | `Client` | A `TaskOrchestration` waiting on a `TaskActivity`. |

## Known Limitations

- `TaskHubClient` client spans do not contain orchestration name
- `TaskHubWorker` client spans do not contain many tags, version, or properly track failed orchestrations.
  - The server span for the failed orchestration will report `ERROR` correctly.
- The following spans are not included due to not being supported by DurableTask.Core:
  - `TaskHubClient` or `TaskOrchestration` sending an event
  - `TaskOrchestration` receiving an event
  - `TaskOrchestration` timers
