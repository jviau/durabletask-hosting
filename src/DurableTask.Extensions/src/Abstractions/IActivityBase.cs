// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using Microsoft.Extensions.Logging;

namespace DurableTask.Extensions.Abstractions;

/// <summary>
/// Interface for <see cref="TaskActivity"/>.
/// </summary>
public interface IActivityBase
{
    /// <summary>
    /// Gets or sets the name this activity was scheduled with.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the version this activity was scheduled with.
    /// </summary>
    string Version { get; set; }

    /// <summary>
    /// Gets or sets the logger for this activity.
    /// </summary>
    ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets the data converter for this activity.
    /// </summary>
    DataConverter DataConverter { get; set; }
}