// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A builder for configuring and adding a <see cref="BaseTaskHubClient" /> to the service container.
    /// </summary>
    public interface ITaskHubClientBuilder
    {
        /// <summary>
        /// Gets the name of the client being built.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        IServiceCollection Services { get; }

        Func<IServiceProvider, IOrchestrationServiceClient>? OrchestrationServiceFactory { get; set; }

        /// <summary>
        /// Gets or sets the target of this builder. The provided type <b>must derive from</b>
        /// <see cref="BaseTaskHubClient" />. This is the type that will ultimately be built by
        /// <see cref="Build(IServiceProvider)" />.
        /// </summary>
        Type? BuildTarget { get; set; }

        /// <summary>
        /// Builds this instance, yielding the built <see cref="BaseTaskHubClient" />.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The built client.</returns>
        BaseTaskHubClient Build(IServiceProvider serviceProvider);
    }
}
