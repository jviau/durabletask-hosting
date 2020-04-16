// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;
using Dynamitey;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderExtensions
    {
        /// <summary>
        /// Sets the provided <paramref name="orchestrationService"/> to the <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The task hub builder.</param>
        /// <param name="orchestrationService">The orchestration service to use.</param>
        /// <returns>The original builder, with orchestration service set.</returns>
        public static ITaskHubWorkerBuilder WithOrchestrationService(
            this ITaskHubWorkerBuilder builder, IOrchestrationService orchestrationService)
        {
            Check.NotNull(builder, nameof(builder));
            builder.OrchestrationService = orchestrationService;
            return builder;
        }

        /// <summary>
        /// Adds <see cref="TaskHubClient"/> to the service collection.
        /// </summary>
        /// <param name="builder">The builder to add the client from.</param>
        /// <returns>The original builder, with <see cref="TaskHubClient"/> added to the service collection.</returns>
        public static ITaskHubWorkerBuilder AddClient(this ITaskHubWorkerBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
            builder.Services.TryAddSingleton(_ => ClientFactory(builder));
            return builder;
        }

        private static TaskHubClient ClientFactory(ITaskHubWorkerBuilder builder)
        {
            if (builder.OrchestrationService == null)
            {
                throw new InvalidOperationException(Strings.OrchestrationInstanceNull);
            }

            if (builder.OrchestrationService is IOrchestrationServiceClient client)
            {
                return new TaskHubClient(client);
            }

            throw new InvalidOperationException(
                Strings.NotOrchestrationServiceClient(builder.OrchestrationService.GetType()));
        }
    }
}
