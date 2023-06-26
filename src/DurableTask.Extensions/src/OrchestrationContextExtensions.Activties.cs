// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Extensions.Properties;

namespace DurableTask.Extensions;

/// <summary>
/// Extensions for <see cref="OrchestrationContext" />.
/// </summary>
public static partial class OrchestrationContextExtensions
{
    /// <summary>
    /// Schedules an activity for the supplied input.
    /// </summary>
    /// <typeparam name="TResult">The result type of the activity.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The request. Not null.</param>
    /// <param name="retryOptions">The retry policy. Optional.</param>
    /// <returns>The result of the activity.</returns>
    public static Task<TResult> SendAsync<TResult>(
        this OrchestrationContext context, IActivityRequest<TResult> request, RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));
        return context.SendCoreAsync(request, retryOptions);
    }

    /// <summary>
    /// Schedules an activity for the supplied input.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The request. Not null.</param>
    /// <param name="retryOptions">The retry policy. Optional.</param>
    /// <returns>A task that completes when the activity is finished.</returns>
    public static Task SendAsync(
        this OrchestrationContext context, IActivityRequest request, RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));
        return context.SendCoreAsync(request, retryOptions);
    }

    private static Task<TResult> SendCoreAsync<TResult>(
        this OrchestrationContext context, IActivityRequest<TResult> request, RetryOptions? retryOptions)
    {
        TaskActivityDescriptor descriptor = request.GetDescriptor();
        Verify.NotNull(descriptor, Strings.NullDescriptor);
        if (retryOptions is null)
        {
            return context.ScheduleTask<TResult>(descriptor.Name, descriptor.Version, request.GetInput());
        }

        return context.ScheduleWithRetry<TResult>(
            descriptor.Name, descriptor.Version, retryOptions, request.GetInput());
    }
}
