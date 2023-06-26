// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;
using DurableTask.Core;
using DurableTask.Reflection;

namespace DurableTask.Instrumentation;

/// <summary>
/// Extensions for <see cref="TraceContextBase"/>.
/// </summary>
internal static class DurableTraceContextExtensions
{
    private static readonly Func<TraceContextBase, Activity> s_activityGetter;
    private static readonly Action<TraceContextBase, Activity> s_activitySetter;

    static DurableTraceContextExtensions()
    {
        PropertyInfo propertyInfo = typeof(TraceContextBase)
            .GetProperty("CurrentActivity", BindingFlags.NonPublic | BindingFlags.Instance);

        s_activityGetter = propertyInfo.CreateGetter<TraceContextBase, Activity>();
        s_activitySetter = propertyInfo.CreateSetter<TraceContextBase, Activity>();
    }

    /// <summary>
    /// Gets the depth of a trace context. This is the measurement for how far away it is from
    /// the top-level orchestration.
    /// </summary>
    /// <param name="context">The orchestration context.</param>
    /// <returns>The depth of an orchestration or activity. 0 for top-level.</returns>
    public static int GetDepth(this TraceContextBase context)
    {
        Check.NotNull(context);
        return context.OrchestrationTraceContexts.Count / 2;
    }

    /// <summary>
    /// Gets the current activity for the <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The context. Not null.</param>
    /// <returns>The activity.</returns>
    /// <remarks>Accesses an internal property.</remarks>
    public static Activity GetActivity(this TraceContextBase context)
    {
        Check.NotNull(context);
        return s_activityGetter(context);
    }

    /// <summary>
    /// Gets the current activity for the <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The context. Not null.</param>
    /// <param name="activity">The activity to set. May be null.</param>
    /// <remarks>Accesses an internal property.</remarks>
    public static void SetActivity(this TraceContextBase context, Activity activity)
    {
        Check.NotNull(context);
        s_activitySetter(context, activity);
    }

    /// <summary>
    /// Restores the activity of a W3CTraceContext.
    /// </summary>
    /// <param name="context">The context to restore the activity from.</param>
    /// <returns>The restored activity.</returns>
    public static Activity RestoreActivity(this TraceContextBase context)
    {
        if (context is W3CTraceContext w3c)
        {
            return w3c.RestoreActivity();
        }

        throw new ArgumentException("Expected W3CTraceContext");
    }

    /// <summary>
    /// Restores the activity of a W3CTraceContext.
    /// </summary>
    /// <param name="context">The context to restore the activity from.</param>
    /// <returns>The restored activity.</returns>
    /// <remarks>
    /// Activity tracking in DTFx Orchestrations is interesting. It does not follow the expected behavior of almost
    /// all other activity usages from other libraries. Essentially they do not use <see cref="Activity"/>
    /// correctly, but instead use their own <see cref="CorrelationTraceContext"/> and
    /// <see cref="TraceContextBase"/>. So we have to do a bunch of workarounds to shim their behavior over to
    /// expected activity behavior.
    ///
    /// 1) We 'restore' the activity during re-runs (if necessary), so telemetry initiated here will pick it up.
    ///    This uses a lot of reflection to perform the restore.
    /// 2) We start the activity, but do not stop it. The activity will either be GC'd, or actually stopped
    ///    when DTFx sends its DiagnosticListener write event.
    /// 3) Additionally, we also cached some information in TraceState, so we parse that out and set tags. This
    ///    restoration actually happens in multiple ways due to overlap / redundancy in the design. This is
    ///    intentional as they cover different scenarios. Most of the time IContextInfo is available and the info
    ///    can be fetched from there. Other times only the Activity is available, and others only the TraceContext.
    ///    So we are always restoring it, just to cover all the different scenarios without needing to constantly
    ///    test which is and is not available.
    /// </remarks>
    public static Activity RestoreActivity(this W3CTraceContext context)
    {
        Check.NotNull(context);
        Activity? activity = context.GetActivity();
        if (activity is not null)
        {
            Activity.Current = activity;
            return activity;
        }

        activity = Activity.Current;
        if (activity is not null)
        {
            context.SetActivity(activity);
            return activity;
        }

        // "TraceParent" is this spans full id - not the parent id.
        var spanContext = ActivityContext.Parse(context.TraceParent, context.TraceState);
        activity = new Activity(context.OperationName);
        activity.SetStartTime(context.StartTime.UtcDateTime);
        activity.SetIdFormat(ActivityIdFormat.W3C);
        if (!string.IsNullOrEmpty(context.ParentSpanId))
        {
            var parentSpanId = ActivitySpanId.CreateFromString(context.ParentSpanId.AsSpan());
            activity.SetParentId(spanContext.TraceId, parentSpanId, spanContext.TraceFlags);
        }
        else
        {
            activity.SetTraceId(spanContext.TraceId.ToHexString());
            activity.ActivityTraceFlags = spanContext.TraceFlags;
        }

        activity.Start(); // Activity.Current now set.

        // These access private fields and MUST be called after starting the activity.
        activity.SetId(context.TraceParent);
        activity.SetSpanId(spanContext.SpanId.ToHexString());

        // Track this activity back onto the DTFx context so that we can consume it later when tracing.
        context.SetActivity(activity);
        return activity;
    }
}
