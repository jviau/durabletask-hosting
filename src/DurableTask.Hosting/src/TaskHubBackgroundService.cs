// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.Extensions.Hosting;

namespace DurableTask.Hosting
{
    /// <summary>
    /// A dotnet hosted service for <see cref="TaskHubWorker"/>.
    /// </summary>
    public class TaskHubBackgroundService : BackgroundService
    {
        private readonly TaskHubWorker _worker;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskHubBackgroundService"/> class.
        /// </summary>
        /// <param name="worker">The task hub worker.</param>
        public TaskHubBackgroundService(TaskHubWorker worker)
        {
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
        }

        /// <inheritdoc />
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _worker.StopAsync();
            await base.StopAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _worker.StartAsync();
        }
    }
}
