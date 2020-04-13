// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderOrchestrationExtensions
    {
        /// <summary>
        /// Adds the supplied orchestration type to the builder as a transient service.
        /// Must be of type <see cref="TaskOrchestration"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="type">The orchestration type to add.</param>
        /// <returns>The original builder, with the orchestration type added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration(this ITaskHubWorkerBuilder builder, Type type)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddOrchestration(TaskOrchestrationDescriptor.Transient(type));
        }

        /// <summary>
        /// Adds the supplied orchestration type to the builder as a transient service.
        /// Must be of type <see cref="TaskOrchestration"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <typeparam name="TOrchestration">The task orchestration type to add.</typeparam>
        /// <returns>The original builder, with the orchestration type added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(this ITaskHubWorkerBuilder builder)
            where TOrchestration : TaskOrchestration
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddOrchestration(TaskOrchestrationDescriptor.Transient<TOrchestration>());
        }

        /// <summary>
        /// Adds the supplied orchestration type to the builder as a transient service.
        /// Must be of type <see cref="TaskOrchestration"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="type">The orchestration type to add.</param>
        /// <param name="name">The name of the task to add.</param>
        /// <param name="version">The version of the task to add.</param>
        /// <returns>The original builder, with the orchestration type added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration(
            this ITaskHubWorkerBuilder builder, Type type, string name, string version)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddOrchestration(TaskOrchestrationDescriptor.Transient(name, version, type));
        }

        /// <summary>
        /// Adds the supplied orchestration type to the builder as a transient service.
        /// Must be of type <see cref="TaskOrchestration"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="name">The name of the task to add.</param>
        /// <param name="version">The version of the task to add.</param>
        /// <typeparam name="TOrchestration">The task orchestration type to add.</typeparam>
        /// <returns>The original builder, with the orchestration type added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
            this ITaskHubWorkerBuilder builder, string name, string version)
            where TOrchestration : TaskOrchestration
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddOrchestration(TaskOrchestrationDescriptor.Transient<TOrchestration>(name, version));
        }

        /// <summary>
        /// Adds the supplied orchestration type to the builder as a transient service.
        /// Must be of type <see cref="TaskOrchestration"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="creator">The delegate to create the task orchestration.</param>
        /// <typeparam name="TOrchestration">The task orchestration type to add.</typeparam>
        /// <returns>The original builder, with the orchestration type added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
            this ITaskHubWorkerBuilder builder, Func<IServiceProvider, TOrchestration> creator)
            where TOrchestration : TaskOrchestration
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddOrchestration(TaskOrchestrationDescriptor.Transient(creator));
        }

        /// <summary>
        /// Adds the supplied orchestration type to the builder as a transient service.
        /// Must be of type <see cref="TaskOrchestration"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="factory">The delegate to create the task orchestration.</param>
        /// <param name="name">The name of the task to add.</param>
        /// <param name="version">The version of the task to add.</param>
        /// <typeparam name="TOrchestration">The task orchestration type to add.</typeparam>
        /// <returns>The original builder, with the orchestration type added.</returns>
        public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
            this ITaskHubWorkerBuilder builder, Func<IServiceProvider, TOrchestration> factory, string name, string version)
            where TOrchestration : TaskOrchestration
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddOrchestration(TaskOrchestrationDescriptor.Transient(name, version, factory));
        }
    }
}
