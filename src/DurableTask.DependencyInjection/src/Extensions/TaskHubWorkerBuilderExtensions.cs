// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection.Extensions;
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

        return string.Empty;
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
    public static ITaskHubWorkerBuilder UseOrchestrationService(
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
    public static ITaskHubWorkerBuilder UseOrchestrationService(
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
    /// Sets the provided <paramref name="orchestrationService"/> to the <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The task hub builder.</param>
    /// <param name="orchestrationService">The orchestration service to use.</param>
    /// <returns>The original builder, with orchestration service set.</returns>
    [Obsolete("Use UseOrchestrationService instead. This method will be removed in a future version.")]
    public static ITaskHubWorkerBuilder WithOrchestrationService(
        this ITaskHubWorkerBuilder builder, IOrchestrationService orchestrationService)
        => builder.UseOrchestrationService(orchestrationService);

    /// <summary>
    /// Sets the provided <paramref name="orchestrationServiceFactory"/> to the <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The task hub builder.</param>
    /// <param name="orchestrationServiceFactory">The orchestration service factory to use.</param>
    /// <returns>The original builder, with orchestration service set.</returns>
    [Obsolete("Use UseOrchestrationService instead. This method will be removed in a future version.")]
    public static ITaskHubWorkerBuilder WithOrchestrationService(
        this ITaskHubWorkerBuilder builder, Func<IServiceProvider, IOrchestrationService> orchestrationServiceFactory)
        => UseOrchestrationService(builder, orchestrationServiceFactory);

    /// <summary>
    /// Adds <see cref="TaskHubClient"/> to the service collection with the specified <paramref name="serviceClient"/>.
    /// </summary>
    /// <param name="builder">The builder to add the client from.</param>
    /// <param name="serviceClient">The orchestration service client to use.</param>
    /// <returns>The original builder, with <see cref="TaskHubClient"/> added to the service collection.</returns>
    public static ITaskHubWorkerBuilder AddClient(
        this ITaskHubWorkerBuilder builder, IOrchestrationServiceClient serviceClient)
    {
        Check.NotNull(builder);
        Check.NotNull(serviceClient);

        builder.Configure(o => o.OrchestrationServiceClient = serviceClient);
        builder.AddClient();
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

        if (builder is DefaultTaskHubWorkerBuilder { ClientAdded: true })
        {
            return builder;
        }

        string name = builder.GetName();
        builder.Services.TryAddSingleton<ITaskHubClientProvider, DefaultTaskHubClientProvider>();
        builder.Services.AddSingleton(sp => new DefaultTaskHubClientProvider.ClientContainer(
                name, ClientFactory(name, builder, sp)));
        if (builder is DefaultTaskHubWorkerBuilder b)
        {
            b.ClientAdded = true;
        }

        // retain legacy behavior of adding this to the service collection directly.
        if (string.IsNullOrEmpty(name))
        {
            builder.Services.TryAddSingleton(sp => ClientFactory(name, builder, sp));
        }

        return builder;
    }

    private static TaskHubClient ClientFactory(
        string name, ITaskHubWorkerBuilder builder, IServiceProvider serviceProvider)
    {
        TaskHubOptions options = serviceProvider.GetOptions<TaskHubOptions>(name);
        IOrchestrationServiceClient? client = options.GetOrchestrationServiceClient();

        // retain legacy behavior of getting from service provider for default client only.
        if (client is null && string.IsNullOrEmpty(name))
        {
            client = serviceProvider.GetService<IOrchestrationServiceClient>();
        }

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
        InternalTaskHubOptions internalOptions = serviceProvider.GetOptions<InternalTaskHubOptions>(name);
        DataConverter converter = internalOptions?.DataConverter ?? JsonDataConverter.Default;
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return new TaskHubClient(client, converter, loggerFactory);
    }
}
