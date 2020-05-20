// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using DurableTask.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Taskhub <see cref="TaskOrchestration" /> extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderOrchestrationExtensions
    {
        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// Includes <see cref="TaskAliasAttribute"/>.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The orchestration type to add, not null.</param>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration(this ITaskHubWorkerBuilder builder, Type type)
            => AddOrchestration(builder, type, includeAliases: true);

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The orchestration type to add, not null.</param>
        /// <param name="includeAliases">Include <see cref="TaskAliasAttribute"/>.</param>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration(
            this ITaskHubWorkerBuilder builder, Type type, bool includeAliases)
        {
            Check.NotNull(builder, nameof(builder));
            builder.AddOrchestration(new TaskOrchestrationDescriptor(type));

            if (includeAliases)
            {
                foreach ((string name, string version) in type.GetTaskAliases())
                {
                    builder.AddOrchestration(new TaskOrchestrationDescriptor(type, name, version));
                }
            }

            return builder;
        }

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// Includes <see cref="TaskAliasAttribute"/>.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(this ITaskHubWorkerBuilder builder)
            where TOrchestration : TaskOrchestration
            => AddOrchestration(builder, typeof(TOrchestration));

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="includeAliases">Include <see cref="TaskAliasAttribute"/>.</param>
        /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
            this ITaskHubWorkerBuilder builder, bool includeAliases)
            where TOrchestration : TaskOrchestration
            => AddOrchestration(builder, typeof(TOrchestration), includeAliases);

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The orchestration type to add, not null.</param>
        /// <param name="name">The name of the orchestration. Not null or empty.</param>
        /// <param name="version">The version of the orchestration.static Not null.</param>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration(
            this ITaskHubWorkerBuilder builder, Type type, string name, string version)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNullOrEmpty(name, nameof(name));
            Check.NotNull(version, nameof(version));

            builder.AddOrchestration(new TaskOrchestrationDescriptor(type, name, version));
            return builder;
        }

        /// <summary>
        /// Adds the provided orchestration type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="name">The name of the orchestration. Not null or empty.</param>
        /// <param name="version">The version of the orchestration.static Not null.</param>
        /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
        /// <returns>The original builder with orchestration added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
            this ITaskHubWorkerBuilder builder, string name, string version)
            where TOrchestration : TaskOrchestration
            => AddOrchestration(builder, typeof(TOrchestration), name, version);

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
    }
}
