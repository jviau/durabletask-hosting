// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core.Middleware;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for <see cref="ITaskMiddleware"/>.
    /// </summary>
    public sealed class TaskMiddlewareDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMiddlewareDescriptor"/> class.
        /// </summary>
        /// <param name="type">The type to describe.</param>
        public TaskMiddlewareDescriptor(Type type)
        {
            Check.NotNull(type, nameof(type));
            Check.ConcreteType<ITaskMiddleware>(type, nameof(type));

            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMiddlewareDescriptor"/> class.
        /// </summary>
        /// <param name="func">The func to invoke for this middelware.</param>
        public TaskMiddlewareDescriptor(Func<DispatchMiddlewareContext, Func<Task>, Task> func)
        {
            Func = Check.NotNull(func, nameof(func));
        }

        /// <summary>
        /// Gets the type held by this descriptor.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the func to invoke for this middleware.
        /// </summary>
        public Func<DispatchMiddlewareContext, Func<Task>, Task> Func { get; }

        /// <summary>
        /// Creates a new <see cref="TaskMiddlewareDescriptor"/> with the provided type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ITaskMiddleware"/>.</typeparam>
        /// <returns>A new ,iddleware descriptor.</returns>
        public static TaskMiddlewareDescriptor Create<T>()
            where T : ITaskMiddleware
            => new TaskMiddlewareDescriptor(typeof(T));

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Type is object)
            {
                return $"Type: {Type}";
            }
            else if (Func is object)
            {
                return $"Func: {Func}";
            }

            return base.ToString();
        }
    }
}
