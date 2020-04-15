// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DurableTask.Hosting
{
    /// <summary>
    /// A dotnet hosted service for <see cref="TaskHubWorker"/>.
    /// </summary>
    public class TaskHubBackgroundService : IHostedService
    {
        private readonly TaskHubWorker _worker;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskHubBackgroundService"/> class.
        /// </summary>
        /// <param name="worker">The task hub worker. Not null.</param>
        /// <param name="logger">The logger. Not null.</param>
        public TaskHubBackgroundService(TaskHubWorker worker, ILogger<TaskHubBackgroundService> logger)
        {
            _worker = Check.NotNull(worker, nameof(worker));
            _logger = Check.NotNull(logger, nameof(logger));
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting task hub worker.");
            return _worker.StartAsync();
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var cancel = Task.Delay(Timeout.Infinite, cancellationToken);
            Task task = await Task.WhenAny(_worker.StopAsync(), cancel);

            if (cancel == task)
            {
                _logger.LogWarning("Unable to gracefully shutdown task hub worker in time.");
            }
        }
    }
}
