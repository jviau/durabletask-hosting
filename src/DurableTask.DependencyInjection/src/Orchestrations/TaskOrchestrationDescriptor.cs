// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for <see cref="TaskOrchestration"/>.
    /// </summary>
    public sealed class TaskOrchestrationDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskOrchestrationDescriptor"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="name">The name of the type.</param>
        /// <param name="version">The version of the type.</param>
        public TaskOrchestrationDescriptor(Type type, string name = null, string version = null)
        {
            Check.NotNull(type, nameof(type));
            Check.ConcreteType<TaskOrchestration>(type, nameof(type));

            Type = type;
            Name = name ?? GenericNameHelper.GetDefaultName(type);
            Version = version ?? NameVersionHelper.GetDefaultVersion(type);
        }

        /// <summary>
        /// Gets the task name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the task version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the type held by this descriptor.
        /// </summary>
        public Type Type { get; }
    }
}
