// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.DependencyInjection;

/// <summary>
/// A provider for getting <see cref="TaskHubClient" />.
/// </summary>
/// <remarks>
/// The purpose of this abstraction is that there may be multiple clients registered, so they cannot be DI'd directly.
/// </remarks>
public interface ITaskHubClientProvider
{
    /// <summary>
    /// Gets the task hub client. Throws if the client by the requested name is not found.
    /// </summary>
    /// <param name="name">The name of the client to get or <c>null</c> to get the default client.</param>
    /// <returns>The task hub client.</returns>
    TaskHubClient GetClient(string? name = null);
}
