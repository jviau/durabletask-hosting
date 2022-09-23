// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.Extensions.Logging;
using DurableTask.Extensions.Properties;
using Microsoft.Extensions.Logging;

namespace DurableTask.Extensions.Abstractions;

/// <summary>
/// A base <see cref="TaskOrchestration" /> with additional semantics..
/// </summary>
/// <typeparam name="TInput">The input for the orchestration.</typeparam>
/// <typeparam name="TResult">The result of the orchestration.</typeparam>
public abstract class OrchestrationBase<TInput, TResult> : TaskOrchestration<TResult, TInput>, IOrchestrationBase
    where TInput : IOrchestrationRequest<TResult>
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
    DataConverter IOrchestrationBase.DataConverter
    {
        get => DataConverter;
        set => DataConverter = value;
    }

    /// <summary>
    /// Gets the orchestration context.
    /// </summary>
    protected OrchestrationContext Context { get; private set; }

    /// <inheritdoc />
    public override Task<TResult> RunTask(OrchestrationContext context, TInput input)
    {
        Check.NotNull(context, nameof(context));

        if (DataConverter is JsonDataConverter converter)
        {
            // Set more data converters. The middleware will have set this to a JsonDataConverter.
            context.MessageDataConverter = converter;
            context.ErrorDataConverter = converter;
        }
        else
        {
            throw new InvalidOperationException(
                Strings.InvalidDataConverterType(typeof(JsonDataConverter), DataConverter?.GetType()));
        }

        // Wrap the logger the middleware gave us with an orchestration specific logger.
        // This logger will not log when the OrchestrationContext is replaying.
        Logger = Logger is OrchestrationLogger wrapped
            ? new OrchestrationLogger(context, wrapped.InnerLogger)
            : new OrchestrationLogger(context, Logger);

        // Set the context property so it doesn't need to be passed around in every method.
        Context = context;

        return RunAsync(input);
    }

    /// <summary>
    /// Executes this orchestration instance.
    /// </summary>
    /// <param name="input">The input for this orchestration.</param>
    /// <returns>The result of this orchestration.</returns>
    protected abstract Task<TResult> RunAsync(TInput input);
}
