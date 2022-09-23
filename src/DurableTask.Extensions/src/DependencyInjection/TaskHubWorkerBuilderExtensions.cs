// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Extensions.Middleware;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Extensions for <see cref="ITaskHubWorkerBuilder" />.
/// </summary>
public static class TaskHubWorkerBuilderExtensions
{
    /// <summary>
    /// Adds durable extensions to the worker builder.
    /// </summary>
    /// <param name="builder">The builder to add to.</param>
    /// <returns>The original builder.</returns>
    public static ITaskHubWorkerBuilder AddDurableExtensions(this ITaskHubWorkerBuilder builder)
    {
        Check.NotNull(builder);
        builder.UseActivityMiddleware<SetActivityDataMiddleware>();
        builder.UseOrchestrationMiddleware<SetOrchestrationDataMiddleware>();
        return builder;
    }
}
