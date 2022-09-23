// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using Microsoft.Extensions.Logging;

namespace DurableTask.Extensions.Abstractions;

/// <summary>
/// A base <see cref="TaskActivity" /> with additional semantics.
/// </summary>
/// <typeparam name="TInput">The input for the activity.</typeparam>
/// <typeparam name="TResult">The output for the activity.</typeparam>
public abstract class ActivityBase<TInput, TResult> : AsyncTaskActivity<TInput, TResult>, IActivityBase
    where TInput : IActivityRequest<TResult>
{
    /// <inheritdoc />
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    public string Name { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    public string Version { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    public ILogger Logger { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// This will be set by middleware.
    /// </remarks>
    DataConverter IActivityBase.DataConverter
    {
        get => DataConverter;
        set => DataConverter = value;
    }

    /// <summary>
    /// Gets the task context for this activity.
    /// </summary>
    protected TaskContext Context { get; private set; }

    /// <inheritdoc />
    protected override async Task<TResult> ExecuteAsync(TaskContext context, TInput input)
    {
        Check.NotNull(context, nameof(context));
        Context = context;
        return await RunAsync(input);
    }

    /// <summary>
    /// Abstract method for executing a task activity asynchronously.
    /// </summary>
    /// <param name="input">The typed input.</param>
    /// <returns>The typed output from the execution.</returns>
    protected abstract Task<TResult> RunAsync(TInput input);
}
