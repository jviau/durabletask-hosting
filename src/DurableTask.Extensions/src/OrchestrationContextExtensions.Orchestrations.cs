// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Extensions.Abstractions;

namespace DurableTask.Extensions;

/// <summary>
/// Extensions for <see cref="OrchestrationContext" />.
/// </summary>
public static partial class OrchestrationContextExtensions
{
    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TResult">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TResult> SendAsync<TResult>(
        this OrchestrationContext context,
        IOrchestrationRequest<TResult> request,
        bool fireAndForget)
        => context.SendAsync(request, instanceId: null, fireAndForget);

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TResult">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TResult> SendAsync<TResult>(
        this OrchestrationContext context,
        IOrchestrationRequest<TResult> request,
        string? instanceId,
        bool fireAndForget)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        Dictionary<string, string>? tags = null;
        if (fireAndForget)
        {
            // see https://github.com/Azure/durabletask/blob/e1d9ecf497c0d4c2af1be6096cd1563123f86bbe/src/DurableTask.Core/OrchestrationTags.cs#L37
            tags = new Dictionary<string, string>
            {
                [OrchestrationTags.FireAndForget] = OrchestrationTags.FireAndForget,
            };
        }

        return context.SendCoreAsync(request, instanceId, tags);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TResult">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="retryOptions">The retry options. Not null.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TResult> SendAsync<TResult>(
        this OrchestrationContext context, IOrchestrationRequest<TResult> request, RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));
        return context.SendCoreAsync(request, instanceId: null, retryOptions);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TResult">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="retryOptions">The retry options. Optional.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TResult> SendAsync<TResult>(
        this OrchestrationContext context,
        IOrchestrationRequest<TResult> request,
        string instanceId,
        RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        return context.SendCoreAsync(request, instanceId, retryOptions);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task SendAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        bool fireAndForget)
        => context.SendAsync(request, instanceId: null, fireAndForget);

    /// <summary>
    /// Schedules a sub orchestration and doesn't wait for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task SendAndForgetAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        string? instanceId = null)
        => context.SendAsync(request, instanceId, fireAndForget: true);

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task SendAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        string? instanceId,
        bool fireAndForget)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        Dictionary<string, string>? tags = null;
        if (fireAndForget)
        {
            // see https://github.com/Azure/durabletask/blob/e1d9ecf497c0d4c2af1be6096cd1563123f86bbe/src/DurableTask.Core/OrchestrationTags.cs#L37
            tags = new Dictionary<string, string>
            {
                [OrchestrationTags.FireAndForget] = OrchestrationTags.FireAndForget,
            };
        }

        return context.SendCoreAsync(request, instanceId, tags);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="retryOptions">The retry options. Not null.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task SendAsync(
        this OrchestrationContext context, IOrchestrationRequest request, RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        return context.SendCoreAsync(request, instanceId: null, retryOptions);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="retryOptions">The retry options. Optional.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task SendAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        string instanceId,
        RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        return context.SendCoreAsync(request, instanceId, retryOptions);
    }

    private static Task<TResult> SendCoreAsync<TResult>(
        this OrchestrationContext context,
        IOrchestrationRequest<TResult> request,
        string? instanceId,
        IDictionary<string, string>? orchestrationTags)
    {
        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        return context.CreateSubOrchestrationInstance<TResult>(
                descriptor.Name,
                descriptor.Version,
                instanceId,
                request,
                tags: orchestrationTags);
    }

    private static Task<TResult> SendCoreAsync<TResult>(
        this OrchestrationContext context,
        IOrchestrationRequest<TResult> request,
        string? instanceId,
        RetryOptions? retryOptions)
    {
        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        if (retryOptions is not null)
        {
            return context.CreateSubOrchestrationInstanceWithRetry<TResult>(
                descriptor.Name, descriptor.Version, instanceId, retryOptions, request);
        }

        return context.CreateSubOrchestrationInstance<TResult>(
                descriptor.Name, descriptor.Version, instanceId, request);
    }
}
