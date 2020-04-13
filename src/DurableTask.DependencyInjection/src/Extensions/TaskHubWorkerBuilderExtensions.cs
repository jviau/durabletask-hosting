// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderExtensions
    {
        /// <summary>
        /// Adds the provided activity type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The activity type to add, not null.</param>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity(this ITaskHubWorkerBuilder builder, Type type)
        {
            Check.NotNull(builder, nameof(builder));
            builder.AddActivity(new TaskActivityDescriptor(type));
            return builder;
        }

        /// <summary>
        /// Adds the provided activity type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <typeparam name="TActivity">The activity type to add.</typeparam>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(this ITaskHubWorkerBuilder builder)
            where TActivity : TaskActivity
            => AddActivity(builder, typeof(TActivity));

        /// <summary>
        /// Adds the provided activity middleware to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The activity middleware type to add, not null.</param>
        /// <returns>The original builder with activity middleware added.</returns>
        public static ITaskHubWorkerBuilder UseActivityMiddleware(this ITaskHubWorkerBuilder builder, Type type)
        {
            Check.NotNull(builder, nameof(builder));
            builder.UseActivityMiddleware(new TaskMiddlewareDescriptor(type));
            return builder;
        }

        /// <summary>
        /// Adds the provided activity middleware to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <typeparam name="TMiddleware">The activity middleware type to add.</typeparam>
        /// <returns>The original builder with activity middleware added.</returns>
        public static ITaskHubWorkerBuilder UseActivityMiddleware<TMiddleware>(this ITaskHubWorkerBuilder builder)
            where TMiddleware : ITaskMiddleware
            => UseActivityMiddleware(builder, typeof(TMiddleware));

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The orchestration type to add, not null.</param>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration(this ITaskHubWorkerBuilder builder, Type type)
        {
            Check.NotNull(builder, nameof(builder));
            builder.AddOrchestration(new TaskOrchestrationDescriptor(type));
            return builder;
        }

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(this ITaskHubWorkerBuilder builder)
            where TOrchestration : TaskOrchestration
            => AddOrchestration(builder, typeof(TOrchestration));

        /// <summary>
        /// Adds the provided orchestration middleware to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The orchestration middleware type to add, not null.</param>
        /// <returns>The original builder with orchestration middleware added.</returns>
        public static ITaskHubWorkerBuilder UseOrchestrationMiddleware(this ITaskHubWorkerBuilder builder, Type type)
        {
            Check.NotNull(builder, nameof(builder));
            builder.UseOrchestrationMiddleware(new TaskMiddlewareDescriptor(type));
            return builder;
        }

        /// <summary>
        /// Adds the provided orchestration middleware to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <typeparam name="TMiddleware">The orchestration middleware type to add.</typeparam>
        /// <returns>The original builder with orchestration middleware added.</returns>
        public static ITaskHubWorkerBuilder UseOrchestrationMiddleware<TMiddleware>(this ITaskHubWorkerBuilder builder)
            where TMiddleware : ITaskMiddleware
            => UseOrchestrationMiddleware(builder, typeof(TMiddleware));

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
                throw new InvalidOperationException($"OrchestrationService not set on ITaskHubWorkerBuilder.");
            }

            if (builder.OrchestrationService is IOrchestrationServiceClient client)
            {
                return new TaskHubClient(client);
            }

            throw new InvalidOperationException(
                $"Failed to add TaskHubClient. " +
                $"{builder.OrchestrationService.GetType()} does not implement {typeof(IOrchestrationServiceClient)}.");
        }
    }
}
