// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

namespace DurableTask.Extensions.Abstractions;

/// <summary>
/// Contract for providing input to an orchestration or activity.
/// </summary>
internal interface IProvidesInput
{
    /// <summary>
    /// Gets the input for the orchestration or activity.
    /// </summary>
    /// <returns>The input value.</returns>
    /// <remarks>
    /// This is a method and not a property to ensure it is not included in serialization.
    /// </remarks>
    object? GetInput();
}
