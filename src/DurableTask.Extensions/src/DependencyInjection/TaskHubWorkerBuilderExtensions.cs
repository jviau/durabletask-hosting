// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Serializing;
using DurableTask.Extensions;
using DurableTask.Extensions.Middleware;
using Microsoft.Extensions.DependencyInjection;

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
        => builder.AddDurableExtensions(_ => { });

    /// <summary>
    /// Adds durable extensions to the worker builder.
    /// </summary>
    /// <param name="builder">The builder to add to.</param>
    /// <param name="configure">The options configure action.</param>
    /// <returns>The original builder.</returns>
    public static ITaskHubWorkerBuilder AddDurableExtensions(
        this ITaskHubWorkerBuilder builder, Action<DurableExtensionsOptions> configure)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);
        builder.UseActivityMiddleware<SetActivityDataMiddleware>();
        builder.UseOrchestrationMiddleware<SetOrchestrationDataMiddleware>();

        builder.Services.AddOptions<DurableExtensionsOptions>()
            .Configure<IServiceProvider>((opt, sp) =>
            {
                DataConverter converter = sp.GetService<DataConverter>();
                if (converter is not null)
                {
                    opt.DataConverter = converter;
                }
            })
            .Configure(configure);

        return builder;
    }
}
