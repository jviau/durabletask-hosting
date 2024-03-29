// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DurableTask.Extensions;

/// <summary>
/// A base <see cref="TaskActivity" /> with additional semantics.
/// </summary>
/// <typeparam name="TInput">The input for the activity.</typeparam>
public abstract class ActivityBase<TInput> : AsyncTaskActivity<TInput, Unit>, IActivityBase
{
    private TaskContext? _context;

    /// <inheritdoc />
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    public virtual string Name { get; private set; } = string.Empty;

    /// <inheritdoc />
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    public virtual string? Version { get; private set; }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    public ILogger Logger { get; private set; } = NullLogger.Instance;

    /// <summary>
    /// Gets the task context for this activity.
    /// </summary>
    protected TaskContext Context => _context!;

    /// <inheritdoc />
    void IActivityBase.Initialize(string name, string? version, ILogger logger, DataConverter converter)
    {
        Name = Check.NotNull(name);
        Version = version;
        Logger = Check.NotNull(logger);
        DataConverter = Check.NotNull(converter);
    }

    /// <inheritdoc />
    protected override async Task<Unit> ExecuteAsync(TaskContext context, TInput input)
    {
        Check.NotNull(context, nameof(context));
        _context = context;
        await RunAsync(input);
        return Unit.Value;
    }

    /// <summary>
    /// Abstract method for executing a task activity asynchronously.
    /// </summary>
    /// <param name="input">The typed input.</param>
    /// <returns>A task that completes when the activity is finished.</returns>
    protected abstract Task RunAsync(TInput input);
}
