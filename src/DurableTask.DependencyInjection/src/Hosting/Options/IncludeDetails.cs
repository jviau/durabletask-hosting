// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Hosting.Options;

/// <summary>
/// The flags for when to include error details.
/// </summary>
[Flags]
public enum IncludeDetails
{
    /// <summary>
    /// Do not include error details.
    /// </summary>
    None = 0,

    /// <summary>
    /// Enabled for activities.
    /// <see cref="TaskActivityDispatcher.IncludeDetails"/>.
    /// </summary>
    Activities = 1 << 0,

    /// <summary>
    /// Enabled for orchestrations.
    /// <see cref="TaskOrchestrationDispatcher.IncludeDetails"/>.
    /// </summary>
    Orchestrations = 1 << 1,

    /// <summary>
    /// Enabled for all.
    /// </summary>
    All = Activities | Orchestrations,
}
