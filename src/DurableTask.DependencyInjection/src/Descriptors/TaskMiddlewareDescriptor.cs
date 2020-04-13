// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor wrapper for <see cref="ITaskMiddleware"/>.
    /// </summary>
    public sealed class TaskMiddlewareDescriptor : ServiceDescriptorWrapper
    {
        private TaskMiddlewareDescriptor(Type implementationType, ServiceDescriptor descriptor)
            : base(implementationType, descriptor)
        {
        }

        /// <summary>
        /// Creates a singleton middleware descriptor.
        /// </summary>
        /// <param name="type">The concrete <see cref="ITaskMiddleware"/> type.</param>
        /// <returns>A descriptor for a singleton taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Singleton(Type type)
        {
            Check.NotNull(type, nameof(type));
            Check.ConcreteType<ITaskMiddleware>(type, nameof(type));

            return new TaskMiddlewareDescriptor(type, ServiceDescriptor.Singleton(type));
        }

        /// <summary>
        /// Creates a singleton middleware descriptor.
        /// </summary>
        /// <typeparam name="TMiddleware">The concrete <see cref="ITaskMiddleware"/> type.</typeparam>
        /// <returns>A descriptor for a singleton taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Singleton<TMiddleware>()
            where TMiddleware : class, ITaskMiddleware
            => Singleton(typeof(TMiddleware));

        /// <summary>
        /// Creates a singleton middleware descriptor.
        /// </summary>
        /// <param name="instance">The implementation instance.</param>
        /// <returns>A descriptor for a singleton taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Singleton(ITaskMiddleware instance)
        {
            Check.NotNull(instance, nameof(instance));
            Check.ConcreteType<ITaskMiddleware>(instance.GetType(), nameof(instance));

            return new TaskMiddlewareDescriptor(
                instance.GetType(), ServiceDescriptor.Singleton(instance.GetType(), instance));
        }

        /// <summary>
        /// Creates a singleton middleware descriptor.
        /// </summary>
        /// <param name="factory">The factory to produce the singleton middleware.</param>
        /// <typeparam name="TMiddleware">The concrete <see cref="ITaskMiddleware"/> type.</typeparam>
        /// <returns>A descriptor for a singleton taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Singleton<TMiddleware>(Func<IServiceProvider, TMiddleware> factory)
            where TMiddleware : class, ITaskMiddleware
        {
            Check.NotNull(factory, nameof(factory));
            Check.ConcreteType<ITaskMiddleware>(typeof(TMiddleware), nameof(TMiddleware));

            return new TaskMiddlewareDescriptor(typeof(TMiddleware), ServiceDescriptor.Singleton(factory));
        }

        /// <summary>
        /// Creates a transient middleware descriptor.
        /// </summary>
        /// <param name="type">The concrete <see cref="ITaskMiddleware"/> type.</param>
        /// <returns>A descriptor for a transient taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Transient(Type type)
        {
            Check.NotNull(type, nameof(type));
            Check.ConcreteType<ITaskMiddleware>(type, nameof(type));

            return new TaskMiddlewareDescriptor(type, ServiceDescriptor.Transient(type, type));
        }

        /// <summary>
        /// Creates a transient middleware descriptor.
        /// </summary>
        /// <typeparam name="TMiddleware">The concrete <see cref="ITaskMiddleware"/> type.</typeparam>
        /// <returns>A descriptor for a transient taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Transient<TMiddleware>()
            where TMiddleware : class, ITaskMiddleware
            => Transient(typeof(TMiddleware));

        /// <summary>
        /// Creates a transient middleware descriptor.
        /// </summary>
        /// <param name="factory">The factory to produce the transient middleware.</param>
        /// <typeparam name="TMiddleware">The concrete <see cref="ITaskMiddleware"/> type.</typeparam>
        /// <returns>A descriptor for a transient taskhub middleware.</returns>
        public static TaskMiddlewareDescriptor Transient<TMiddleware>(Func<IServiceProvider, TMiddleware> factory)
            where TMiddleware : class, ITaskMiddleware
        {
            Check.NotNull(factory, nameof(factory));
            Check.ConcreteType<ITaskMiddleware>(typeof(TMiddleware), nameof(TMiddleware));

            return new TaskMiddlewareDescriptor(typeof(TMiddleware), ServiceDescriptor.Transient(factory));
        }
    }
}
