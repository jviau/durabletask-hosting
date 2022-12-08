// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace DurableTask;

/// <summary>
/// Log messages.
/// </summary>
internal static class LogMessages
{
#pragma warning disable SA1201 // elements should be in correct order
#pragma warning disable SA1600 // elements should be documented

    private static readonly Func<ILogger, string, string?, IDisposable> s_runActivityScope = LoggerMessage
        .DefineScope<string, string?>("ActivityName = {activityName}, ActivityVersion = {activityVersion}");

    public static IDisposable RunActivityScope(this ILogger logger, string name, string? version)
        => s_runActivityScope.Invoke(logger, name, version);

    private static readonly Func<ILogger, string, string?, IDisposable> s_runOrchestrationScope = LoggerMessage
        .DefineScope<string, string?>("OrchestrationName = {orchestrationName}, OrchestrationVersion = {orchestrationVersion}");

    public static IDisposable RunOrchestrationScope(this ILogger logger, string name, string? version)
        => s_runOrchestrationScope.Invoke(logger, name, version);

#pragma warning restore SA1201 // elements should be in correct order
#pragma warning restore SA1600 // elements should be documented
}
