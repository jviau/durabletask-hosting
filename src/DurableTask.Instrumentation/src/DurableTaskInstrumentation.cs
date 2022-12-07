// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;
using DurableTask.Core;
using DurableTask.Core.Exceptions;
using DurableTask.Core.Settings;

namespace DurableTask.Instrumentation;

/// <summary>
/// Instrumentation handler for DUrableTask.
/// </summary>
internal class DurableTaskInstrumentation
{
    internal static readonly AssemblyName AssemblyName = typeof(DurableTaskInstrumentation).Assembly.GetName();
    internal static readonly string ClientSourceName = AssemblyName.Name + ".TaskHubClient";
    internal static readonly string WorkerSourceName = AssemblyName.Name + ".TaskHubWorker";
    internal static readonly string ActivityVersion = AssemblyName.Version.ToString();
    internal static readonly ActivitySource ClientSource = new(ClientSourceName, ActivityVersion);
    internal static readonly ActivitySource WorkerSource = new(WorkerSourceName, ActivityVersion);

    private static readonly HashSet<string> s_trackedPrefixes = new(StringComparer.Ordinal)
    {
        TraceConstants.Activity,
        TraceConstants.Client,
        TraceConstants.Orchestrator,
        "activity:",
        "orchestration:",
    };


    /// <summary>
    /// Initializes a new instance of the <see cref="DurableTaskInstrumentation"/> class.
    /// </summary>
    public DurableTaskInstrumentation()
    {
        CorrelationSettings.Current.Protocol = Protocol.W3CTraceContext;
        CorrelationSettings.Current.EnableDistributedTracing = true;
        CorrelationTraceClient.SetUp(Track(), Track(OnDependencyTelemetry), OnException);
    }

    private static Action<TraceContextBase> Track(Action<TraceContextBase, Activity>? inner = null)
    {
        return context =>
        {
            try
            {
                if (!ShouldTrack(context))
                {
                    return;
                }

                if (context.OperationName.StartsWith(TraceConstants.Client))
                {
                    OnClient(context);
                    return;
                }

                using ActivityContextShim shim = new(context, stop: true);
                if (Activity.Current is not {} activity)
                {
                    return;
                }

                activity.SetSource(WorkerSource);
                activity.SetTag("durabletask.task.depth", context.GetDepth());

                if (activity.Status is ActivityStatusCode.Unset)
                {
                    activity.SetStatus(ActivityStatusCode.Ok);
                }

                inner?.Invoke(context, activity);
            }
            catch (Exception ex)
            {
                DurableTaskInstrumentationEventSource.Log.FailedProcessTrace(ex);
                if (ex.IsFatal())
                {
                    throw;
                }
            }
        };
    }

    private static bool ShouldTrack(TraceContextBase context)
    {
        return !string.IsNullOrEmpty(context.OperationName)
            && s_trackedPrefixes.Any(s => context.OperationName.StartsWith(s));
    }

    private void OnDependencyTelemetry(TraceContextBase traceContext, Activity activity)
    {
        const string prefix = "DtOrchestrator ";
        if (!traceContext.OperationName.StartsWith(prefix))
        {
            DurableTaskInstrumentationEventSource.Log.UnexpectedClientSpanName(traceContext.OperationName);
            return;
        }

        // Unfortunately, client telemetry is very lacking here. We do not have any information
        // we can use to flush out the activity more. We do not even know what type it is.
        activity.DisplayName = SpanNameHelper.GetSpanName(
            "send_task", activity.OperationName.AsSpan(prefix.Length), null);
        activity.SetSource(WorkerSource);
        activity.SetKind(ActivityKind.Client);
    }

    private void OnException(Exception exception)
    {
        // Some exceptions are logged later. Restore context for them (if possible).
        using ActivityContextShim shim = new(CorrelationTraceContext.Current, stop: false);
        if (Activity.Current is not {} activity)
        {
            return;
        }

        Exception cause = exception.GetDurableFailureCause();
        if (cause is TaskFailureException taskFailure)
        {
            cause = taskFailure.InnerException;
        }

        if (cause is OrchestrationFailureException orchestrationFailure)
        {
            // DTFx does not set the root cause as in InnerException, so we try to stash it away in the
            // Activity manually.
            Exception? original = activity.GetException();
            cause = original ?? orchestrationFailure;
        }

        if (activity.Status is ActivityStatusCode.Unset)
        {
            activity.SetException(cause);
        }
    }

    private static void OnClient(TraceContextBase context)
    {
        if (context.TelemetryType == TelemetryType.Request
            || Activity.Current is not {} activity)
        {
            return;
        }

        // We unfortunately do not have many details on exactly what is being enqueued here.
        activity.SetSource(ClientSource);
        activity.SetKind(ActivityKind.Producer);
        activity.DisplayName = SpanNameHelper.GetSpanName("start_orchestration", "unknown", null);
        activity.SetTag("durabletask.type", "orchestration");
    }

    /// <summary>
    /// DTFx does not always have the activity set by the time it sends out the trace event. We will restore it
    /// temporarily to fix that.
    /// </summary>
    private readonly struct ActivityContextShim : IDisposable
    {
        private readonly Activity? _previous;
        private readonly bool _stop;

        public ActivityContextShim(TraceContextBase? context, bool stop)
        {
            _stop = stop;
            _previous = Activity.Current;
            Activity = context?.RestoreActivity();
            if (stop)
            {
                Activity?.SetEndTime(DateTime.UtcNow);
            }
        }

        public Activity? Activity { get; }

        public void Dispose()
        {
            if (_stop)
            {
                Activity?.Dispose();
            }

            Activity.Current = _previous;
        }
    }
}
