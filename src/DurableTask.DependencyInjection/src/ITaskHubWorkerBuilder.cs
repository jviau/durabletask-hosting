// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
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
        /// Gets or sets the <see cref="IOrchestrationService"/> to use.
        /// </summary>
        IOrchestrationService OrchestrationService { get; set; }

        /// <summary>
        /// Adds the provided descriptor of an activity to the builder.
        /// </summary>
        /// <param name="descriptor">The activity descriptor to add.</param>
        /// <returns>This instance, for chaining calls.</returns>
        ITaskHubWorkerBuilder AddActivity(TaskActivityDescriptor descriptor);

        /// <summary>
        /// Adds the provided middleware for task activities.
        /// </summary>
        /// <param name="descriptor">The middleware descriptor to add.</param>
        /// <returns>This instance, for chaining calls.</returns>
        ITaskHubWorkerBuilder UseActivityMiddleware(TaskMiddlewareDescriptor descriptor);

        /// <summary>
        /// Adds the provided descriptor to the builder.
        /// </summary>
        /// <param name="descriptor">The descriptor to add.</param>
        /// <returns>This instance, for chaining calls.</returns>
        ITaskHubWorkerBuilder AddOrchestration(TaskOrchestrationDescriptor descriptor);

        /// <summary>
        /// Adds the provided middleware for task orchestrations.
        /// </summary>
        /// <param name="descriptor">The middleware descriptor to add.</param>
        /// <returns>This instance, for chaining calls.</returns>
        ITaskHubWorkerBuilder UseOrchestrationMiddleware(TaskMiddlewareDescriptor descriptor);
    }
}
