// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection.Internal;
using DurableTask.DependencyInjection.Properties;
using DurableTask.Hosting.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Extensions for <see cref="ITaskHubWorkerBuilder"/>.
/// </summary>
public static class TaskHubWorkerBuilderExtensions
{
    /// <summary>
    /// Gets the name of the builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder name.</returns>
    /// <exception cref="InvalidOperationException">When the name of the builder cannot be determined.</exception>
    /// <remarks>
    /// This is an extension method fo avoid adding a member to the interface, making for a breaking change.
    /// </remarks>
    public static string GetName(this ITaskHubWorkerBuilder builder)
    {
        Check.NotNull(builder);

        if (builder is DefaultTaskHubWorkerBuilder defaultBuilder)
        {
            return defaultBuilder.Name;
        }

        throw new InvalidOperationException($"Unable to find the name of the builder for builder type {builder.GetType()}.");
    }

    /// <summary>
    /// Configures the task hub worker with the provided <paramref name="configure"/> action.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The original builder, with a configure action added.</returns>
    public static ITaskHubWorkerBuilder Configure(this ITaskHubWorkerBuilder builder, Action<TaskHubOptions> configure)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);

        builder.Services.Configure(builder.GetName(), configure);
        return builder;
    }

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
        Check.NotNull(orchestrationService);
        builder.Configure(o => o.OrchestrationService = orchestrationService);

        if (string.IsNullOrEmpty(builder.GetName()))
        {
            // retain legacy behavior of adding this to the service collection directly.
            builder.Services.TryAddSingleton(orchestrationService);
        }

        return builder;
    }

    /// <summary>
    /// Sets the provided <paramref name="orchestrationServiceFactory"/> to the <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The task hub builder.</param>
    /// <param name="orchestrationServiceFactory">The orchestration service factory to use.</param>
    /// <returns>The original builder, with orchestration service set.</returns>
    public static ITaskHubWorkerBuilder WithOrchestrationService(
        this ITaskHubWorkerBuilder builder, Func<IServiceProvider, IOrchestrationService> orchestrationServiceFactory)
    {
        Check.NotNull(builder);
        Check.NotNull(orchestrationServiceFactory);
        builder.Services.AddOptions<TaskHubOptions>(builder.GetName())
            .Configure<IServiceProvider>((o, s) => o.OrchestrationService = orchestrationServiceFactory(s));

        if (string.IsNullOrEmpty(builder.GetName()))
        {
            // retain legacy behavior of adding this to the service collection directly.
            builder.Services.TryAddSingleton(orchestrationServiceFactory);
        }

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

        // TODO: Add ITaskHubClientProvider, register named clients. Need to ensure each one
        // can have its own IOrchestrationServiceClient.
        if (string.IsNullOrEmpty(builder.GetName()))
        {
            // retain legacy behavior of adding this to the service collection directly.
            builder.Services.TryAddSingleton(sp => ClientFactory(builder, sp));
        }

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

        // Options does not have to be present.
        IOptions<TaskHubClientOptions> options = serviceProvider.GetService<IOptions<TaskHubClientOptions>>();
        DataConverter converter = options?.Value?.DataConverter ?? JsonDataConverter.Default;
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return new TaskHubClient(client, converter, loggerFactory);
    }
}
