// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;

namespace DurableTask.Extensions.Abstractions;

/// <summary>
/// Represents a request to run a <see cref="TaskOrchestration" />.
/// </summary>
public interface IOrchestrationRequestBase
{
    /// <summary>
    /// Gets the descriptor for the handler of this request.
    /// </summary>
    /// <remarks>
    /// This should point to the current name and version of the
    /// <see cref="TaskOrchestration"/> we want to schedule for this request.
    /// This must not return <c>null</c>.
    /// </remarks>
    TaskOrchestrationDescriptor GetDescriptor();
}
