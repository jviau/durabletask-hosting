# Overview

The sample includes a basic scenario of using a host builder to configure a durable task host, and register some basic orchestrations and activities.

## Bonus Samples

The sample includes some bonus scenarios I have found useful when working with DurableTask Framework.

### 1. Orchestration Session Data

#### Use Case

Say you want some non-orchestration or activity specific data that is carried through the whole execution of a single orchestration. The orchestration or activities have no use to directly interact with this data, but it is there for some other purpose such as a logging correlation id.

#### Solution

DurableTask offers no official session data. However, the framework does make use of data contracts and `ExtensionData`. Using this functionality, we can store our session data in the `OrchestrationInstance.ExtensionData` and be able to access it for the whole life of the orchestration. See [OrchestrationInstanceEx.cs](./DurableTask.Samples/SessionData/OrchestrationInstanceEx.cs).

#### UPDATE

DurableTask [now supports](https://github.com/Azure/durabletask/pull/804) propagating orchestration tags to activities. This means they can now be used for metadata propagation instead of the workaround above.
