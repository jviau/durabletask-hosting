// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;

namespace DurableTask.Instrumentation;

/// <summary>
/// Instrumentation event source.
/// </summary>
[EventSource(Name = "Vio-DurableTask-Instrumentation")]
internal sealed class DurableTaskInstrumentationEventSource : EventSource
{
    /// <summary>
    /// The event source instance.
    /// </summary>
    public static readonly DurableTaskInstrumentationEventSource Log = new();

    /// <summary>
    /// Event when we have failed to process a distributed trace.
    /// </summary>
    /// <param name="ex">The failing exception.</param>
    [NonEvent]
    public void FailedProcessTrace(Exception ex)
    {
        if (IsEnabled(EventLevel.Error, EventKeywords.All))
        {
            FailedProcessTrace(ex.ToInvariantString());
        }
    }

    /// <summary>
    /// Event when we have failed to process a distributed trace.
    /// </summary>
    /// <param name="ex">The failing exception message.</param>
    [Event(1, Message = "Failed to process DurableTask trace: '{0}'", Level = EventLevel.Error)]
    public void FailedProcessTrace(string ex) => WriteEvent(1, ex);

    /// <summary>
    /// Event when we encountered an unexpected client span name.
    /// </summary>
    /// <param name="name">The unexpected span name.</param>
    [Event(2, Message = "Received client span with unknown name format: '{0}'", Level = EventLevel.Warning)]
    public void UnexpectedClientSpanName(string name) => WriteEvent(2, name);
}
