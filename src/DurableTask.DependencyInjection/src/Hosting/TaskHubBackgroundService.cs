// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;
using DurableTask.Hosting.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.Hosting;

/// <summary>
/// A dotnet hosted service for <see cref="TaskHubWorker"/>.
/// </summary>
public class TaskHubBackgroundService : IHostedService
{
    private readonly TaskHubWorker _worker;
    private readonly ILogger _logger;
    private readonly TaskHubOptions _options;

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
        : this(worker, logger, Check.NotNull(options).Value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskHubBackgroundService"/> class.
    /// </summary>
    /// <param name="worker">The task hub worker. Not null.</param>
    /// <param name="logger">The logger. Not null.</param>
    /// <param name="options">The task hub options.</param>
    public TaskHubBackgroundService(
        TaskHubWorker worker,
        ILogger<TaskHubBackgroundService> logger,
        TaskHubOptions options)
    {
        _worker = Check.NotNull(worker);
        _logger = Check.NotNull(logger);
        _options = Check.NotNull(options);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug(Strings.TaskHubWorkerStarting);

        if (_options.CreateIfNotExists)
        {
            await _worker.orchestrationService.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        await _worker.StartAsync().ConfigureAwait(false);
        _worker.TaskActivityDispatcher.IncludeDetails = _options.IncludeDetails.HasFlag(IncludeDetails.Activities);
        _worker.TaskOrchestrationDispatcher.IncludeDetails = _options.IncludeDetails.HasFlag(
            IncludeDetails.Orchestrations);
        _worker.ErrorPropagationMode = _options.ErrorPropagationMode;
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
