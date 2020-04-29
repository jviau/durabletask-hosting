// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for <see cref="ITaskMiddleware"/>.
    /// </summary>
    public sealed class TaskMiddlewareDescriptor : TypeDescriptor<ITaskMiddleware>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMiddlewareDescriptor"/> class.
        /// </summary>
        /// <param name="type">The type to describe.</param>
        public TaskMiddlewareDescriptor(Type type)
            : base(type)
        {
        }
    }
}
