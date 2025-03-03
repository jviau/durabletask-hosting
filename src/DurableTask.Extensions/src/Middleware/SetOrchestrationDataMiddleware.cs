// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.Extensions.Middleware;

/// <summary>
/// Middleware for setting common orchestration data.
/// </summary>
public sealed class SetOrchestrationDataMiddleware : ITaskMiddleware
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly DataConverter _dataConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetOrchestrationDataMiddleware"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory. Not null.</param>
    /// <param name="options">The data converter. Not null.</param>
    public SetOrchestrationDataMiddleware(ILoggerFactory loggerFactory, IOptions<DurableExtensionsOptions> options)
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

        OrchestrationRuntimeState runtimeState = context.GetProperty<OrchestrationRuntimeState>();
        string name = runtimeState.Name;
        string? version = runtimeState.Version;

        using IDisposable scope = _loggerFactory.CreateLogger(GetType()).RunOrchestrationScope(name, version);
        TaskOrchestration orchestration = context.GetProperty<TaskOrchestration>();
        if (orchestration is IOrchestrationBase baseOrchestration)
        {
            ILogger logger = _loggerFactory.CreateLogger(baseOrchestration.GetType());
            baseOrchestration.Initialize(name, version, logger, _dataConverter);
        }

        await next();
    }
}
