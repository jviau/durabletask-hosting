// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A provider for getting <see cref="BaseTaskHubClient" />.
    /// </summary>
    /// <remarks>
    /// The purpose of this abstraction is that there may be multiple clients registered, so they cannot be DI'd directly.
    /// </remarks>
    public interface ITaskHubClientProvider
    {
        /// <summary>
        /// Gets the <see cref="BaseTaskHubClient" /> by name. Throws if the client by the requested name is not found.
        /// </summary>
        /// <param name="name">The name of the client to get or <c>null</c> to get the default client.</param>
        /// <returns>The client.</returns>
        BaseTaskHubClient GetClient(string? name = null);
    }
}
