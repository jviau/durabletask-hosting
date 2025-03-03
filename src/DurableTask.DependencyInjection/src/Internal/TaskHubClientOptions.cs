// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Serializing;

namespace DurableTask.DependencyInjection.Internal;

/// <summary>
/// This is an internal API that supports the DurableTask infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new DurableTask release.
/// </summary>
/// <remarks>
/// This is internal because it is only a piece of the puzzle for replacing the <see cref="DataConverter" />
/// throughout all of DTFx. Please use Vio.DurableTask.Extensions package to properly replace the entire
/// data converter.
/// </remarks>
public sealed class TaskHubClientOptions
{
    /// <summary>
    /// Gets or sets the data converter.
    /// This is an internal API that supports the DurableTask infrastructure and not subject to
    /// the same compatibility standards as public APIs. It may be changed or removed without notice in
    /// any release. You should only use it directly in your code with extreme caution and knowing that
    /// doing so can result in application failures when updating to a new DurableTask release.
    /// </summary>
    public DataConverter DataConverter { get; set; } = JsonDataConverter.Default;
}
