// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Extensions;

/// <summary>
/// Extensions for <see cref="OrchestrationContext" />.
/// </summary>
public static partial class OrchestrationContextExtensions
{
    /// <summary>
    /// Delays the orchestration until the specified time <paramref name="until" />.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="until">The delay until time.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>A task that completes when the delay time has passed.</returns>
    /// <remarks> DTFx has an undocumented delay limit of 7 days max. To workaround this, any delay greater than 7 days will be performed in max of 6 day increments.</remarks>
    public static Task Delay(
        this OrchestrationContext context, DateTimeOffset until, CancellationToken cancellation = default)
    {
        Check.NotNull(context, nameof(context));
        return context.Delay(until, string.Empty, cancellation);
    }

    /// <summary>
    /// Delays the orchestration until the specified time <paramref name="until" />.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="until">The delay until time.</param>
    /// <param name="state">The time state.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <typeparam name="T">The time state type.</typeparam>
    /// <returns>A task that completes when the delay time has passed.</returns>
    /// <remarks> DTFx has an undocumented delay limit of 7 days max. To workaround this, any delay greater than 7 days will be performed in max of 6 day increments.</remarks>
    public static async Task<T> Delay<T>(
        this OrchestrationContext context, DateTimeOffset until, T state, CancellationToken cancellation = default)
    {
        Check.NotNull(context, nameof(context));
        DateTime utc = context.CurrentUtcDateTime;
        while (until.UtcDateTime > utc.AddDays(6))
        {
            await context.CreateTimer(utc.AddDays(6), state, cancellation);
            utc = context.CurrentUtcDateTime;
        }

        return await context.CreateTimer(until.UtcDateTime, state, cancellation);
    }

    /// <summary>
    /// Delays the orchestration for the specified <see cref="TimeSpan" /> <paramref name="delay" />.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="delay">The delay amount.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>A task that completes when the delay time has passed.</returns>
    /// <remarks> DTFx has an undocumented delay limit of 7 days max. To workaround this, any delay greater than 7 days will be performed in max of 6 day increments.</remarks>
    public static Task Delay(
        this OrchestrationContext context, TimeSpan delay, CancellationToken cancellation = default)
    {
        Check.NotNull(context, nameof(context));
        return context.Delay(delay, string.Empty, cancellation);
    }

    /// <summary>
    /// Delays the orchestration for the specified <see cref="TimeSpan" /> <paramref name="delay" />.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="delay">The delay amount.</param>
    /// <param name="state">The time state.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <typeparam name="T">The time state type.</typeparam>
    /// <returns>A task that completes when the delay time has passed.</returns>
    /// <remarks> DTFx has an undocumented delay limit of 7 days max. To workaround this, any delay greater than 7 days will be performed in max of 6 day increments.</remarks>
    public static Task<T> Delay<T>(
        this OrchestrationContext context, TimeSpan delay, T state, CancellationToken cancellation = default)
    {
        Check.NotNull(context, nameof(context));
        return context.Delay(context.CurrentUtcDateTime.Add(delay), state, cancellation);
    }
}
