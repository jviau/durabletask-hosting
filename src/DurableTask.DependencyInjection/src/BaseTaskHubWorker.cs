// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection
{
    public abstract class BaseTaskHubWorker : IHostedService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTaskHubWorker" /> class.
        /// </summary>
        /// <param name="name">The name of the worker.</param>
        protected BaseTaskHubWorker(string? name)
        {
            Name = name ?? Options.DefaultName;
        }

        /// <summary>
        /// Gets the name of this worker.
        /// </summary>
        protected virtual string Name { get; }

        /// <inheritdoc />
        public abstract Task StartAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract Task StopAsync(CancellationToken cancellationToken);
    }
}
