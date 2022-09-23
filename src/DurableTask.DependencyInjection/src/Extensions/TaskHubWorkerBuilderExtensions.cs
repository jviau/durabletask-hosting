// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
/// </summary>
public static class TaskHubWorkerBuilderExtensions
{
    /// <summary>
    /// Sets the provided <paramref name="orchestrationService"/> to the <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The task hub builder.</param>
    /// <param name="orchestrationService">The orchestration service to use.</param>
    /// <returns>The original builder, with orchestration service set.</returns>
    public static ITaskHubWorkerBuilder WithOrchestrationService(
        this ITaskHubWorkerBuilder builder, IOrchestrationService orchestrationService)
    {
        Check.NotNull(builder);
        builder.Services.TryAddSingleton(orchestrationService);
        return builder;
    }

    /// <summary>
    /// Adds <see cref="TaskHubClient"/> to the service collection.
    /// </summary>
    /// <param name="builder">The builder to add the client from.</param>
    /// <returns>The original builder, with <see cref="TaskHubClient"/> added to the service collection.</returns>
    public static ITaskHubWorkerBuilder AddClient(this ITaskHubWorkerBuilder builder)
    {
        Check.NotNull(builder);
        builder.Services.TryAddSingleton(sp => ClientFactory(builder, sp));
        return builder;
    }

    private static TaskHubClient ClientFactory(ITaskHubWorkerBuilder builder, IServiceProvider serviceProvider)
    {
        IOrchestrationServiceClient? client = serviceProvider.GetService<IOrchestrationServiceClient>();

        if (client is null)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            IOrchestrationService service = builder.OrchestrationService
                ?? serviceProvider.GetRequiredService<IOrchestrationService>();
#pragma warning restore CS0618 // Type or member is obsolete

            client = service as IOrchestrationServiceClient;
            if (client is null)
            {
                throw new InvalidOperationException(
                    Strings.NotOrchestrationServiceClient(service.GetType()));
            }
        }

        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return new TaskHubClient(client, loggerFactory: loggerFactory);
    }
}
