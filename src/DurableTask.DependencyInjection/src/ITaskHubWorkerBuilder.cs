// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DurableTask.DependencyInjection;

/// <summary>
/// A builder for hosting a durable task worker.
/// </summary>
public interface ITaskHubWorkerBuilder 
{
    /// <summary>
    /// Gets the name of this builder.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where durable task services are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets or sets the build target for this builder. The provided type <b>must derive from</b>
    /// <see cref="BaseTaskHubWorker" />. This is the hosted service which will ultimately be ran on host startup.
    /// </summary>
    Type? BuildTarget { get; set; }

    IOrchestrationService? OrchestrationService { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IOrchestrationService"/> to use. If this is null, it will be fetched from the
    /// service provider.
    /// </summary>
    Func<IServiceProvider, IOrchestrationService>? OrchestrationServiceFactory { get; set; }

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


    /// <summary>
    /// Build the hosted service which runs the worker.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The built hosted service.</returns>
    IHostedService Build(IServiceProvider serviceProvider);
}
