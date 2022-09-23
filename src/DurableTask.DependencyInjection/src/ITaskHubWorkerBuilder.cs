// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection;

/// <summary>
/// A builder for hosting a durable task worker.
/// </summary>
public interface ITaskHubWorkerBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where durable task services are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets or sets the <see cref="IOrchestrationService"/> to use. If this is null, it will be fetched from the
    /// service provider.
    /// </summary>
    [Obsolete("Add IOrchestrationService to the IServiceCollection as a singleton instead.")]
    IOrchestrationService? OrchestrationService { get; set; }

    /// <summary>
    /// Gets the activity middleware.
    /// </summary>
    IList<TaskMiddlewareDescriptor> ActivityMiddleware { get; }

    /// <summary>
    /// Gets the orchestration middleware.
    /// </summary>
    IList<TaskMiddlewareDescriptor> OrchestrationMiddleware { get; }

    /// <summary>
    /// Gets the activities.
    /// </summary>
    IList<TaskActivityDescriptor> Activities { get; }

    /// <summary>
    /// Gets the orchestrations.
    /// </summary>
    IList<TaskOrchestrationDescriptor> Orchestrations { get; }
}
