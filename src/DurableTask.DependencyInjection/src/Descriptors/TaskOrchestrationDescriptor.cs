// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for <see cref="TaskOrchestration"/>.
    /// </summary>
    public sealed class TaskOrchestrationDescriptor : NamedTypeDescriptor<TaskOrchestration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskOrchestrationDescriptor"/> class.
        /// </summary>
        /// <param name="type">The type of orchestration to describe.</param>
        public TaskOrchestrationDescriptor(Type type)
            : base(type)
        {
        }
    }
}
