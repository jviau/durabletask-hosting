// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using DurableTask.Core;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="ITaskHubClientProvider" />.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultTaskHubClientProvider"/> class.
/// </remarks>
/// <param name="clients">The set of clients.</param>
internal class DefaultTaskHubClientProvider(IEnumerable<DefaultTaskHubClientProvider.ClientContainer> clients)
    : ITaskHubClientProvider
{
    private readonly IEnumerable<ClientContainer> _clients = Check.NotNull(clients);

    /// <inheritdoc/>
    public TaskHubClient GetClient(string? name = null)
    {
        name ??= Options.DefaultName;
        ClientContainer? client = _clients.FirstOrDefault(
            x => string.Equals(name, x.Name, StringComparison.Ordinal)); // options are case sensitive.

        if (client is null)
        {
            string names = string.Join(", ", _clients.Select(x => $"\"{x.Name}\""));
            throw new ArgumentOutOfRangeException(
                nameof(name), name, $"The value of this argument must be in the set of available clients: [{names}].");
        }

        return client.Client;
    }

    /// <summary>
    /// Container for holding a client in memory.
    /// </summary>
    /// <param name="name">The name of the client.</param>
    /// <param name="client">The client.</param>
    internal class ClientContainer(string name, TaskHubClient client)
    {
        /// <summary>
        /// Gets the client name.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the client.
        /// </summary>
        public TaskHubClient Client { get; } = client;
    }
}
