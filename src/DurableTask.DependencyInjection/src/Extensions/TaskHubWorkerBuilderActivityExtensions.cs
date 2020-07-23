// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using DurableTask.Core;
using DurableTask.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Taskhub <see cref="TaskActivity" /> extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderActivityExtensions
    {
        /// <summary>
        /// Adds the provided activity type to the builder.
        /// Includes <see cref="TaskAliasAttribute"/>.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The activity type to add, not null.</param>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity(this ITaskHubWorkerBuilder builder, Type type)
            => AddActivity(builder, type, includeAliases: true);

        /// <summary>
        /// Adds the provided activity type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The activity type to add, not null.</param>
        /// <param name="includeAliases">Include <see cref="TaskAliasAttribute"/>.</param>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity(
            this ITaskHubWorkerBuilder builder, Type type, bool includeAliases)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddActivity(new TaskActivityDescriptor(type));

            if (includeAliases)
            {
                foreach ((string name, string version) in type.GetTaskAliases())
                {
                    builder.AddActivity(new TaskActivityDescriptor(type, name, version));
                }
            }

            return builder;
        }

        /// <summary>
        /// Adds the provided activity type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="type">The activity type to add, not null.</param>
        /// <param name="name">The name of the activity. Not null or empty.</param>
        /// <param name="version">The version of the activity.static Not null.</param>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity(
            this ITaskHubWorkerBuilder builder, Type type, string name, string version)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNullOrEmpty(name, nameof(name));
            Check.NotNull(version, nameof(version));

            builder.AddActivity(new TaskActivityDescriptor(type, name, version));
            return builder;
        }

        /// <summary>
        /// Adds the provided activity type to the builder.
        /// Includes <see cref="TaskAliasAttribute"/>.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <typeparam name="TActivity">The activity type to add.</typeparam>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(this ITaskHubWorkerBuilder builder)
            where TActivity : TaskActivity
            => AddActivity(builder, typeof(TActivity));

        /// <summary>
        /// Adds the provided activity type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="includeAliases">Include <see cref="TaskAliasAttribute"/>.</param>
        /// <typeparam name="TActivity">The activity type to add.</typeparam>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(
            this ITaskHubWorkerBuilder builder, bool includeAliases)
            where TActivity : TaskActivity
            => AddActivity(builder, typeof(TActivity), includeAliases);

        /// <summary>
        /// Adds the provided activity type to the builder.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="name">The name of the activity. Not null or empty.</param>
        /// <param name="version">The version of the activity.static Not null.</param>
        /// <typeparam name="TActivity">The activity type to add.</typeparam>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivity<TActivity>(
            this ITaskHubWorkerBuilder builder, string name, string version)
            where TActivity : TaskActivity
            => AddActivity(builder, typeof(TActivity), name, version);

        /// <summary>
        /// Adds all <see cref="TaskActivity"/> in the provided assembly.
        /// Includes <see cref="TaskAliasAttribute"/>.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="assembly">The assembly to discover types from. Not null.</param>
        /// <param name="includePrivate">True to include private/protected/internal types, false for public only.</param>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivitiesFromAssembly(
            this ITaskHubWorkerBuilder builder, Assembly assembly, bool includePrivate = false)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(assembly, nameof(assembly));

            foreach (Type type in assembly.GetConcreteTypes<TaskActivity>(includePrivate))
            {
                AddActivity(builder, type);
            }

            return builder;
        }

        /// <summary>
        /// Adds all <see cref="TaskActivity"/> in the provided assembly.
        /// Includes <see cref="TaskAliasAttribute"/>.
        /// </summary>
        /// <param name="builder">The builder to add to, not null.</param>
        /// <param name="includePrivate">True to also include private/protected/internal types, false for public only.</param>
        /// <typeparam name="T">The type contained in the assembly to discover types from.</typeparam>
        /// <returns>The original builder with activity added.</returns>
        public static ITaskHubWorkerBuilder AddActivitiesFromAssembly<T>(
            this ITaskHubWorkerBuilder builder, bool includePrivate = false)
            => AddActivitiesFromAssembly(builder, typeof(T).Assembly, includePrivate);

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
    }
}
