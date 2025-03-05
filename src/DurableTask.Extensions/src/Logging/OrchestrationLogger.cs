// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using Microsoft.Extensions.Logging;

namespace DurableTask.Extensions.Logging;

/// <summary>
/// A logger for use in orchestrations, to avoid duplicate logging during replays.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OrchestrationLogger"/> class.
/// </remarks>
/// <param name="context">The orchestration context.</param>
/// <param name="logger">The underlying logger.</param>
internal sealed class OrchestrationLogger(OrchestrationContext context, ILogger logger) : ILogger
{
    private readonly OrchestrationContext _context = Check.NotNull(context, nameof(context));

    /// <summary>
    /// Gets the internal logger.
    /// </summary>
    internal ILogger InnerLogger { get; } = logger switch
    {
        OrchestrationLogger ol => ol.InnerLogger,
        _ => Check.NotNull(logger),
    };

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
        => InnerLogger.BeginScope(state);

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
        => !_context.IsReplaying && InnerLogger.IsEnabled(logLevel);

    /// <inheritdoc />
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            InnerLogger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
