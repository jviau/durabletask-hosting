// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using DurableTask.Core;
using DurableTask.Core.History;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection;

namespace DurableTask.Instrumentation.Middleware;

/// <summary>
/// Middleware to update the <see cref="Activity" /> for a DurableTask.
/// </summary>
public sealed class EnrichActivitySpanMiddleware : ITaskMiddleware
{
    /// <inheritdoc/>
    public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        if (Activity.Current is {} activity)
        {
            EnrichActivity(activity, context);
        }

        return next();
    }

    private static void EnrichActivity(Activity activity, DispatchMiddlewareContext context)
    {
        TaskScheduledEvent state = context.GetProperty<TaskScheduledEvent>();
        activity.DisplayName = SpanNameHelper.GetSpanName("activity", state.Name!, state.Version);
        activity.SetKind(ActivityKind.Server);
        activity.SetTag("durabletask.type", "activity");
        activity.SetTag("durabletask.task.name", state.Name);

        if (state.Version is not null)
        {
            activity.SetTag("durabletask.task.version", state.Version);
        }

        OrchestrationInstance instance = context.GetProperty<OrchestrationInstance>();
        activity.SetTag("durabletask.task.instance_id", instance.InstanceId);
        activity.SetTag("durabletask.task.execution_id", instance.ExecutionId);
    }
}
