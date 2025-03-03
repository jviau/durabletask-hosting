// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection
{
    internal class DefaultTaskHubClientProvider : ITaskHubClientProvider
    {
        private readonly IEnumerable<ClientContainer> _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskHubClientProvider"/> class.
        /// </summary>
        /// <param name="clients">The set of clients.</param>
        public DefaultTaskHubClientProvider(IEnumerable<ClientContainer> clients)
        {
            _clients = clients;
        }

        /// <inheritdoc/>
        public BaseTaskHubClient GetClient(string? name = null)
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
        internal class ClientContainer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ClientContainer"/> class.
            /// </summary>
            /// <param name="client">The client.</param>
            public ClientContainer(BaseTaskHubClient client)
            {
                Client = Check.NotNull(client);
            }

            /// <summary>
            /// Gets the client name.
            /// </summary>
            public string Name => Client.Name;

            /// <summary>
            /// Gets the client.
            /// </summary>
            public BaseTaskHubClient Client { get; }
        }
    }
}
