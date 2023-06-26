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
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TOutput">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TOutput> RunAsync<TOutput>(
        this OrchestrationContext context,
        IOrchestrationRequest<TOutput> request,
        bool fireAndForget)
        => context.RunAsync(request, instanceId: null, fireAndForget);

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TOutput">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TOutput> RunAsync<TOutput>(
        this OrchestrationContext context,
        IOrchestrationRequest<TOutput> request,
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

        return context.RunCoreAsync(request, instanceId, tags);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TOutput">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="retryOptions">The retry options. Not null.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TOutput> RunAsync<TOutput>(
        this OrchestrationContext context, IOrchestrationRequest<TOutput> request, RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));
        return context.RunCoreAsync(request, instanceId: null, retryOptions);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <typeparam name="TOutput">The result type of the orchestration request.</typeparam>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="retryOptions">The retry options. Optional.</param>
    /// <returns>The output of the orchestration.</returns>
    public static Task<TOutput> RunAsync<TOutput>(
        this OrchestrationContext context,
        IOrchestrationRequest<TOutput> request,
        string instanceId,
        RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        return context.RunCoreAsync(request, instanceId, retryOptions);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task RunAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        bool fireAndForget)
        => context.RunAsync(request, instanceId: null, fireAndForget);

    /// <summary>
    /// Schedules a sub orchestration and doesn't wait for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task RunAndForgetAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        string? instanceId = null)
        => context.RunAsync(request, instanceId, fireAndForget: true);

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="fireAndForget">A flag indicating if this orchestration is a fire and forget execution.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task RunAsync(
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

        return context.RunCoreAsync(request, instanceId, tags);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="retryOptions">The retry options. Not null.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task RunAsync(
        this OrchestrationContext context, IOrchestrationRequest request, RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        return context.RunCoreAsync(request, instanceId: null, retryOptions);
    }

    /// <summary>
    /// Schedules a sub orchestration and waits for completion.
    /// </summary>
    /// <param name="context">The orchestration context. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use.</param>
    /// <param name="retryOptions">The retry options. Optional.</param>
    /// <returns>A task that completes when the orchestration has finished.</returns>
    public static Task RunAsync(
        this OrchestrationContext context,
        IOrchestrationRequest request,
        string instanceId,
        RetryOptions? retryOptions = null)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNull(request, nameof(request));

        return context.RunCoreAsync(request, instanceId, retryOptions);
    }

    private static Task<TOutput> RunCoreAsync<TOutput>(
        this OrchestrationContext context,
        IOrchestrationRequest<TOutput> request,
        string? instanceId,
        IDictionary<string, string>? orchestrationTags)
    {
        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        Verify.NotNull(descriptor, Strings.NullDescriptor);
        return context.CreateSubOrchestrationInstance<TOutput>(
                descriptor.Name,
                descriptor.Version,
                instanceId,
                request.GetInput(),
                tags: orchestrationTags);
    }

    private static Task<TOutput> RunCoreAsync<TOutput>(
        this OrchestrationContext context,
        IOrchestrationRequest<TOutput> request,
        string? instanceId,
        RetryOptions? retryOptions)
    {
        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        Verify.NotNull(descriptor, Strings.NullDescriptor);
        if (retryOptions is not null)
        {
            return context.CreateSubOrchestrationInstanceWithRetry<TOutput>(
                descriptor.Name, descriptor.Version, instanceId, retryOptions, request.GetInput());
        }

        return context.CreateSubOrchestrationInstance<TOutput>(
                descriptor.Name, descriptor.Version, instanceId, request.GetInput());
    }
}
