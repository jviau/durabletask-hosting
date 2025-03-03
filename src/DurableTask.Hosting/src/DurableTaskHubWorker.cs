// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Hosting.Options;
using DurableTask.Hosting.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.Hosting;

/// <summary>
/// A dotnet hosted service for <see cref="TaskHubWorker"/>.
/// </summary>
public class DurableTaskHubWorker : BaseTaskHubWorker
{
    private readonly TaskHubWorker _worker;
    private readonly ILogger _logger;
    private readonly IOptions<TaskHubOptions> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DurableTaskHubWorker"/> class.
    /// </summary>
    /// <param name="name">The name of the worker.</param>
    /// <param name="worker">The task hub worker. Not null.</param>
    /// <param name="loggerFactory">The logger factory. Not null.</param>
    /// <param name="options">The task hub options.</param>
    public DurableTaskHubWorker(
        string name,
        TaskHubWorker worker,
        ILoggerFactory loggerFactory,
        IOptions<TaskHubOptions> options) :
        base(name)
    {
        _worker = Check.NotNull(worker);
        _logger = loggerFactory.CreateLogger($"{typeof(DurableTaskHubWorker).Namespace}.{nameof(DurableTaskHubWorker)}");
        _options = Check.NotNull(options);
    }

    private TaskHubOptions Options => _options.Value;

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug(Strings.TaskHubWorkerStarting);

        if (Options.CreateIfNotExists)
        {
            await _worker.orchestrationService.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        await _worker.StartAsync().ConfigureAwait(false);
        _worker.TaskActivityDispatcher.IncludeDetails = Options.IncludeDetails.HasFlag(IncludeDetails.Activities);
        _worker.TaskOrchestrationDispatcher.IncludeDetails = Options.IncludeDetails.HasFlag(IncludeDetails.Orchestrations);
        _worker.ErrorPropagationMode = Options.ErrorPropagationMode;
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var cancel = Task.Delay(Timeout.Infinite, cancellationToken);
        Task task = await Task.WhenAny(_worker.StopAsync(), cancel).ConfigureAwait(false);

        if (cancel == task)
        {
            _logger.LogWarning(Strings.ForcedShutdown);
        }
    }
}
