// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for <see cref="TaskActivity"/>.
    /// </summary>
    public sealed class TaskActivityDescriptor : NamedTypeDescriptor<TaskActivity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskActivityDescriptor"/> class.
        /// </summary>
        /// <param name="type">The type of activity to describe.</param>
        public TaskActivityDescriptor(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskActivityDescriptor"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="name">The name of the type.</param>
        /// <param name="version">The version of the type.</param>
        public TaskActivityDescriptor(Type type, string name, string version)
            : base(type, name, version)
        {
        }
    }
}
