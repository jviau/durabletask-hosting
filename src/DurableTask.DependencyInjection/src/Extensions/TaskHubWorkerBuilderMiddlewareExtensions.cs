// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
    /// </summary>
    public static class TaskHubWorkerBuilderMiddlewareExtensions
    {
        /// <summary>
        /// Adds the provided middleware to the activity pipeline as a singleton service.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="middleware">The middleware instance to add.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseActivityMiddleware(
            this ITaskHubWorkerBuilder builder, ITaskMiddleware middleware)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseActivityMiddleware(TaskMiddlewareDescriptor.Singleton(middleware));
        }

        /// <summary>
        /// Adds the provided middleware to the activity pipeline as a transient service.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="middlewareType">The type of middleware to use.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseActivityMiddleware(this ITaskHubWorkerBuilder builder, Type middlewareType)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseActivityMiddleware(TaskMiddlewareDescriptor.Transient(middlewareType));
        }

        /// <summary>
        /// Adds the provided middleware to the activity pipeline as a transient service.
        /// </summary>
        /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
        /// <param name="builder">The builder to add to.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseActivityMiddleware<TMiddleware>(this ITaskHubWorkerBuilder builder)
            where TMiddleware : class, ITaskMiddleware
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseActivityMiddleware(TaskMiddlewareDescriptor.Transient<TMiddleware>());
        }

        /// <summary>
        /// Adds the provided middleware to the activity pipeline as a transient service.
        /// </summary>
        /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="factory">The factory to create the middleware.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseActivityMiddleware<TMiddleware>(
            this ITaskHubWorkerBuilder builder, Func<IServiceProvider, TMiddleware> factory)
            where TMiddleware : class, ITaskMiddleware
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseActivityMiddleware(TaskMiddlewareDescriptor.Transient(factory));
        }

        /// <summary>
        /// Adds the provided middleware to the orchestration pipeline as a singleton service.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="middleware">The middleware instance to add.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseOrchestrationMiddleware(
            this ITaskHubWorkerBuilder builder, ITaskMiddleware middleware)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseOrchestrationMiddleware(TaskMiddlewareDescriptor.Singleton(middleware));
        }

        /// <summary>
        /// Adds the provided middleware to the orchestration pipeline as a transient service.
        /// </summary>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="middlewareType">The type mof middleware to use.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseOrchestrationMiddleware(this ITaskHubWorkerBuilder builder, Type middlewareType)
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseOrchestrationMiddleware(TaskMiddlewareDescriptor.Transient(middlewareType));
        }

        /// <summary>
        /// Adds the provided middleware to the orchestration pipeline as a transient service.
        /// </summary>
        /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
        /// <param name="builder">The builder to add to.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseOrchestrationMiddleware<TMiddleware>(this ITaskHubWorkerBuilder builder)
            where TMiddleware : class, ITaskMiddleware
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseOrchestrationMiddleware(TaskMiddlewareDescriptor.Transient<TMiddleware>());
        }

        /// <summary>
        /// Adds the provided middleware to the orchestration pipeline as a transient service.
        /// </summary>
        /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
        /// <param name="builder">The builder to add to.</param>
        /// <param name="factory">The factory to create the middleware.</param>
        /// <returns>The original builder, with middleware added.</returns>
        public static ITaskHubWorkerBuilder UseOrchestrationMiddleware<TMiddleware>(
            this ITaskHubWorkerBuilder builder, Func<IServiceProvider, TMiddleware> factory)
            where TMiddleware : class, ITaskMiddleware
        {
            Check.NotNull(builder, nameof(builder));
            return builder.UseOrchestrationMiddleware(TaskMiddlewareDescriptor.Transient(factory));
        }
    }
}
