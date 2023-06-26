// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using Microsoft.Extensions.Logging;

namespace DurableTask.Extensions;

/// <summary>
/// Interface for <see cref="TaskActivity"/>.
/// </summary>
public interface IActivityBase
{
    /// <summary>
    /// Gets the name this activity was scheduled with.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the version this activity was scheduled with.
    /// </summary>
    string? Version { get; }

    /// <summary>
    /// Initialize the <see cref="IActivityBase" /> before running.
    /// </summary>
    /// <param name="name">The activity name.</param>
    /// <param name="version">The activity version.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="converter">The data converter.</param>
    void Initialize(string name, string? version, ILogger logger, DataConverter converter);
}
