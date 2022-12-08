// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection;

namespace DurableTask.Instrumentation.Middleware;

/// <summary>
/// Middleware to enrich the <see cref="Activity" /> for a DurableTask.
/// </summary>
public sealed class EnrichOrchestrationSpanMiddleware : ITaskMiddleware
{
    /// <inheritdoc/>
    public async Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        Check.NotNull(context);
        Check.NotNull(next);

        using (StartActivity())
        {
            if (Activity.Current is { } activity)
            {
                EnrichActivity(activity, context);
            }

            await next();
        }
    }

    private static void EnrichActivity(Activity activity, DispatchMiddlewareContext context)
    {
        OrchestrationRuntimeState state = context.GetProperty<OrchestrationRuntimeState>();
        activity.DisplayName = SpanNameHelper.GetSpanName("orchestration", state.Name, state.Version);
        activity.SetKind(ActivityKind.Server);
        activity.SetTag("durabletask.type", "orchestration");
        activity.SetTag("durabletask.task.name", state.Name);

        if (state.Version is not null)
        {
            activity.SetTag("durabletask.task.version", state.Version);
        }

        activity.SetTag("durabletask.task.instance_id", state.OrchestrationInstance!.InstanceId);
        activity.SetTag("durabletask.task.execution_id", state.OrchestrationInstance!.ExecutionId);
    }

    private static IDisposable? StartActivity()
    {
        // If Activity.Current is not set, and we have a DTFx context, then restore the activity.
        if (Activity.Current is null && CorrelationTraceContext.Current is W3CTraceContext current)
        {
            current.RestoreActivity();
            return ActivityRestorer.Instance;
        }

        return null;
    }

    private class ActivityRestorer : IDisposable
    {
        public static readonly ActivityRestorer Instance = new();

        public void Dispose()
        {
            Activity.Current = null;
        }
    }
}
