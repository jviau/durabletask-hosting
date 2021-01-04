// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Hosting.Options;
using DurableTask.Hosting.Properties;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.Hosting
{
    /// <summary>
    /// A dotnet hosted service for <see cref="TaskHubWorker"/>.
    /// </summary>
    public class TaskHubBackgroundService : IHostedService
    {
        private readonly TaskHubWorker _worker;
        private readonly ILogger _logger;
        private readonly IOptions<TaskHubOptions> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskHubBackgroundService"/> class.
        /// </summary>
        /// <param name="worker">The task hub worker. Not null.</param>
        /// <param name="logger">The logger. Not null.</param>
        /// <param name="options">The task hub options.</param>
        public TaskHubBackgroundService(
            TaskHubWorker worker,
            ILogger<TaskHubBackgroundService> logger,
            IOptions<TaskHubOptions> options)
        {
            _worker = Check.NotNull(worker, nameof(worker));
            _logger = Check.NotNull(logger, nameof(logger));
            _options = Check.NotNull(options, nameof(options));
        }

        private TaskHubOptions Options => _options.Value;

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(Strings.TaskHubWorkerStarting);

            if (Options.CreateIfNotExists)
            {
                await _worker.orchestrationService.CreateIfNotExistsAsync().ConfigureAwait(false);
            }

            await _worker.StartAsync().ConfigureAwait(false);
            _worker.TaskActivityDispatcher.IncludeDetails = Options.IncludeDetails.HasFlag(IncludeDetails.Activities);
            _worker.TaskOrchestrationDispatcher.IncludeDetails = Options.IncludeDetails.HasFlag(
                IncludeDetails.Orchestrations);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var cancel = Task.Delay(Timeout.Infinite, cancellationToken);
            Task task = await Task.WhenAny(_worker.StopAsync(), cancel).ConfigureAwait(false);

            if (cancel == task)
            {
                _logger.LogWarning(Strings.ForcedShutdown);
            }
        }
    }
}
