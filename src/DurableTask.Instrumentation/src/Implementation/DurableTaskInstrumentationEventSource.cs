// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;

namespace DurableTask.Instrumentation;

[EventSource(Name = "Vio-DurableTask-Instrumentation")]
internal sealed class DurableTaskInstrumentationEventSource : EventSource
{
    public static readonly DurableTaskInstrumentationEventSource Log = new();

    [NonEvent]
    public void FailedProcessTrace(Exception ex)
    {
        if (IsEnabled(EventLevel.Error, EventKeywords.All))
        {
            FailedProcessTrace(ex.ToInvariantString());
        }
    }

    [Event(1, Message = "Failed to process DurableTask trace: '{0}'", Level = EventLevel.Error)]
    public void FailedProcessTrace(string ex) => WriteEvent(1, ex);

    [Event(2, Message = "Received client span with unknown name format: '{0}'", Level = EventLevel.Warning)]
    public void UnexpectedClientSpanName(string name) => WriteEvent(2, name);
}
