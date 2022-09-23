// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;
using DurableTask.Extensions.Abstractions;
using DurableTask.Extensions.Properties;

namespace DurableTask.Core;

/// <summary>
/// Extensions for <see cref="TaskHubClient" />.
/// </summary>
public static class TaskHubClientExtensions
{
    /// <summary>
    /// Starts an orchestration.
    /// </summary>
    /// <param name="client">The task hub client.</param>
    /// <param name="request">The orchestration request.</param>
    /// <returns>The scheduled orchestration instance.</returns>
    public static Task<OrchestrationInstance> StartOrchestrationAsync(
        this TaskHubClient client, IOrchestrationRequestBase request)
    {
        Check.NotNull(client);
        Check.NotNull(request);

        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        Verify.NotNull(descriptor, Strings.NullDescriptor);
        return client.CreateOrchestrationInstanceAsync(descriptor.Name, descriptor.Version, request);
    }

    /// <summary>
    /// Starts an orchestration with the provided instance ID.
    /// </summary>
    /// <param name="client">The task hub client.</param>
    /// <param name="instanceId">The orchestration instanceId to use.</param>
    /// <param name="request">The orchestration request.</param>
    /// <returns>The scheduled orchestration instance.</returns>
    public static Task<OrchestrationInstance> StartOrchestrationAsync(
        this TaskHubClient client, string instanceId, IOrchestrationRequestBase request)
    {
        Check.NotNull(client);
        Check.NotNull(request);
        Check.NotNullOrEmpty(instanceId);

        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        Verify.NotNull(descriptor, Strings.NullDescriptor);
        return client.CreateOrchestrationInstanceAsync(descriptor.Name, descriptor.Version, instanceId, request);
    }

    /// <summary>
    /// Starts an orchestration with the provided instance ID and tags.
    /// </summary>
    /// <param name="client">The task hub client. Not null.</param>
    /// <param name="request">The orchestration request. Not null.</param>
    /// <param name="instanceId">The orchestration instance id to use. Not null.</param>
    /// <param name="tags">The tags for this orchestration. Not null.</param>
    /// <returns>The scheduled orchestration instance for managing this orchestration.</returns>
    public static Task<OrchestrationInstance> StartOrchestrationAsync(
        this TaskHubClient client,
        IOrchestrationRequestBase request,
        string instanceId,
        IDictionary<string, string> tags)
    {
        Check.NotNull(client, nameof(client));
        Check.NotNull(request, nameof(request));
        Check.NotNullOrEmpty(instanceId, nameof(instanceId));
        Check.NotNull(tags, nameof(tags));

        TaskOrchestrationDescriptor descriptor = request.GetDescriptor();
        Verify.NotNull(descriptor, Strings.NullDescriptor);
        return client.CreateOrchestrationInstanceAsync(
            descriptor.Name, descriptor.Version, instanceId, request, tags);
    }
}
