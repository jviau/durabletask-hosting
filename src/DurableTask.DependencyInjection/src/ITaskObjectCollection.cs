// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// The collection of task types.
    /// </summary>
    internal interface ITaskObjectCollection : IReadOnlyCollection<NamedServiceDescriptorWrapper>
    {
        /// <summary>
        /// Gets the task object identified by <paramref name="taskName"/> and <paramref name="taskVersion"/>.
        /// </summary>
        /// <param name="taskName">The name of the task to get.</param>
        /// <param name="taskVersion">The version of the task to get.</param>
        Type this[string taskName, string taskVersion] { get; }
    }
}
