// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;
using DurableTask.Reflection;

namespace DurableTask.Instrumentation;

internal static class ActivityExtensions
{
    /// <summary>
    /// The exception key.
    /// </summary>
    private const string ExceptionKey = "__Exception__";

    private static readonly Action<Activity, string> s_idSet;
    private static readonly Action<Activity, string> s_spanIdSet;
    private static readonly Action<Activity, string> s_traceIdSet;
    private static readonly Action<Activity, ActivityKind> s_setKind;
    private static readonly Action<Activity, ActivitySource> s_setSource;

    static ActivityExtensions()
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        s_idSet = typeof(Activity).GetField("_id", flags).CreateSetter<Activity, string>();
        s_spanIdSet = typeof(Activity).GetField("_spanId", flags).CreateSetter<Activity, string>();
        s_traceIdSet = typeof(Activity).GetField("_traceId", flags).CreateSetter<Activity, string>();
        s_setKind = typeof(Activity).GetProperty("Kind").CreateSetter<Activity, ActivityKind>();
        s_setSource = typeof(Activity).GetProperty("Source").CreateSetter<Activity, ActivitySource>();
    }

    /// <summary>
    /// Sets the id for the <paramref name="activity"/>.
    /// </summary>
    /// <param name="activity">The activity to set the id on.</param>
    /// <param name="id">The id to set.</param>
    /// <remarks>Accesses a private field.</remarks>
    public static void SetId(this Activity activity, string id)
    {
        Check.NotNull(activity);
        s_idSet(activity, id);
    }

    /// <summary>
    /// Sets the span id for the <paramref name="activity"/>.
    /// </summary>
    /// <param name="activity">The activity to set the span id on.</param>
    /// <param name="spanId">The span id to set.</param>
    /// <remarks>Accesses a private field.</remarks>
    public static void SetSpanId(this Activity activity, string spanId)
    {
        Check.NotNull(activity);
        s_spanIdSet(activity, spanId);
    }

    /// <summary>
    /// Sets the trace id for the <paramref name="activity"/>.
    /// </summary>
    /// <param name="activity">The activity to set the span id on.</param>
    /// <param name="traceId">The trace id to set.</param>
    /// <remarks>Accesses a private field.</remarks>
    public static void SetTraceId(this Activity activity, string traceId)
    {
        Check.NotNull(activity);
        s_traceIdSet(activity, traceId);
    }

    /// <summary>
    /// Sets the <see cref="ActivityKind" /> for the <paramref name="activity" />.
    /// </summary>
    /// <param name="activity">The activity to set the kind for.</param>
    /// <param name="kind">The activity kind.</param>
    /// <remarks>Accesses a private setter.</remarks>
    public static void SetKind(this Activity activity, ActivityKind kind)
    {
        Check.NotNull(activity);
        s_setKind(activity, kind);
    }

    /// <summary>
    /// Sets the <see cref="ActivitySource" /> for the <paramref name="activity" />.
    /// </summary>
    /// <param name="activity">The activity to set the source for.</param>
    /// <param name="source">The activity source.</param>
    /// <remarks>Accesses a private setter.</remarks>
    public static void SetSource(this Activity activity, ActivitySource source)
    {
        Check.NotNull(activity);
        s_setSource(activity, source);
    }

    /// <summary>
    /// Sets an error on the activity.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <param name="exception">The exception to set.</param>
    /// <param name="description">The optional description. Exception message will be used if null.</param>
    public static void SetException(this Activity activity, Exception exception, string? description = null)
    {
        Check.NotNull(activity);
        Check.NotNull(exception);

        description ??= exception.Message;
        activity.SetStatus(ActivityStatusCode.Error, description);
        activity.SetTag("exception.type", exception.GetType());
        activity.SetCustomProperty(ExceptionKey, exception);
    }

    /// <summary>
    /// Gets the exception set on this activity, if available.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The found exception, or null.</returns>
    public static Exception? GetException(this Activity activity)
    {
        Check.NotNull(activity);
        object? ex = activity.GetCustomProperty(ExceptionKey);
        return ex as Exception;
    }
}
