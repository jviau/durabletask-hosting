// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.History;
using DurableTask.Core.Middleware;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.Extensions.Middleware;

/// <summary>
/// Middleware for setting common orchestration data.
/// </summary>
public sealed class SetActivityDataMiddleware : ITaskMiddleware
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly DataConverter _dataConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetActivityDataMiddleware"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory. Not null.</param>
    /// <param name="options">The data converter. Not null.</param>
    public SetActivityDataMiddleware(ILoggerFactory loggerFactory, IOptions<DurableExtensionsOptions> options)
    {
        _loggerFactory = Check.NotNull(loggerFactory);
        DurableExtensionsOptions opt = Check.NotNull(options).Value;
        _dataConverter = Check.NotNull(opt.DataConverter);
    }

    /// <inheritdoc />
    public async Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        Check.NotNull(context);
        Check.NotNull(next);

        TaskScheduledEvent scheduledEvent = context.GetProperty<TaskScheduledEvent>();
        string name = scheduledEvent.Name ?? string.Empty;
        string? version = scheduledEvent.Version;

        using IDisposable scope = _loggerFactory.CreateLogger(GetType()).RunActivityScope(name, version);
        TaskActivity activity = context.GetProperty<TaskActivity>();
        Type type = activity.GetType();
        if (activity is IActivityBase baseActivity)
        {
            ILogger logger = _loggerFactory.CreateLogger(baseActivity.GetType());
            baseActivity.Initialize(name, version, logger, _dataConverter);
        }

        if (activity is ReflectionBasedTaskActivity reflectionActivity)
        {
            reflectionActivity.DataConverter = _dataConverter;
        }

        await next();
    }
}
