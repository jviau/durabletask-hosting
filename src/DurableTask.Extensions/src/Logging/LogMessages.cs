// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace DurableTask;

internal static class LogMessages
{
    private static readonly Func<ILogger, string, string?, IDisposable> s_runActivityScope = LoggerMessage
        .DefineScope<string, string?>("ActivityName = {activityName}, ActivityVersion = {activityVersion}");

    public static IDisposable RunActivityScope(this ILogger logger, string name, string? version)
        => s_runActivityScope.Invoke(logger, name, version);

    private static readonly Func<ILogger, string, string?, IDisposable> s_runOrchestrationScope = LoggerMessage
        .DefineScope<string, string?>("OrchestrationName = {orchestrationName}, OrchestrationVersion = {orchestrationVersion}");

    public static IDisposable RunOrchestrationScope(this ILogger logger, string name, string? version)
        => s_runOrchestrationScope.Invoke(logger, name, version);
}
