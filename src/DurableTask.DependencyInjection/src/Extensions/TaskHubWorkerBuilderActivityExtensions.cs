// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderActivityExtensions
    {
        /// <summary>
        /// Adds the supplied activity type to the builder as a transient service.
        /// Must be of type <see cref="TaskActivity"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="type">The activity type to add.</param>
        /// <returns>The original builder, with the activity type added.</returns>
        public static ITaskHubWorkerBuilder AddActivity(this ITaskHubWorkerBuilder builder, Type type)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddActivity(TaskActivityDescriptor.Transient(type));
        }

        /// <summary>
        /// Adds the supplied activity type to the builder as a transient service.
        /// Must be of type <see cref="TaskActivity"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <typeparam name="TActivity">The task activity type to add.</typeparam>
        /// <returns>The original builder, with the activity type added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(this ITaskHubWorkerBuilder builder)
            where TActivity : TaskActivity
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddActivity(TaskActivityDescriptor.Transient<TActivity>());
        }

        /// <summary>
        /// Adds the supplied activity type to the builder as a transient service.
        /// Must be of type <see cref="TaskActivity"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="creator">The delegate to create the task activity.</param>
        /// <typeparam name="TActivity">The task activity type to add.</typeparam>
        /// <returns>The original builder, with the activity type added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(
            this ITaskHubWorkerBuilder builder, Func<IServiceProvider, TActivity> creator)
            where TActivity : TaskActivity
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddActivity(TaskActivityDescriptor.Transient(creator));
        }

        /// <summary>
        /// Adds the supplied activity type to the builder as a transient service.
        /// Must be of type <see cref="TaskActivity"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="type">The activity type to add.</param>
        /// <param name="name">The name of the task to add.</param>
        /// <param name="version">The version of the task to add.</param>
        /// <returns>The original builder, with the activity type added.</returns>
        public static ITaskHubWorkerBuilder AddActivity(
            this ITaskHubWorkerBuilder builder, Type type, string name, string version)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddActivity(TaskActivityDescriptor.Transient(type, name, version));
        }

        /// <summary>
        /// Adds the supplied activity type to the builder as a transient service.
        /// Must be of type <see cref="TaskActivity"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="name">The name of the task to add.</param>
        /// <param name="version">The version of the task to add.</param>
        /// <typeparam name="TActivity">The task activity type to add.</typeparam>
        /// <returns>The original builder, with the activity type added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(
            this ITaskHubWorkerBuilder builder, string name, string version)
            where TActivity : TaskActivity
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddActivity(TaskActivityDescriptor.Transient<TActivity>(name, version));
        }

        /// <summary>
        /// Adds the supplied activity type to the builder as a transient service.
        /// Must be of type <see cref="TaskActivity"/>.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="factory">The delegate to create the task activity.</param>
        /// <param name="name">The name of the task to add.</param>
        /// <param name="version">The version of the task to add.</param>
        /// <typeparam name="TActivity">The task activity type to add.</typeparam>
        /// <returns>The original builder, with the activity type added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(
            this ITaskHubWorkerBuilder builder, Func<IServiceProvider, TActivity> factory, string name, string version)
            where TActivity : TaskActivity
        {
            Check.NotNull(builder, nameof(builder));
            return builder.AddActivity(TaskActivityDescriptor.Transient(factory, name, version));
        }
    }
}
