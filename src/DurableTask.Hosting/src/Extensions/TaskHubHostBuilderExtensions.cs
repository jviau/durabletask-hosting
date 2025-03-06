// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;
using DurableTask.Hosting.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DurableTask.Hosting;

/// <summary>
/// Extensions for configuring a task hub worker service on <see cref="IHostBuilder"/>.
/// </summary>
public static class TaskHubHostBuilderExtensions
{
    /// <summary>
    /// Configures the task hub worker background service.
    /// </summary>
    /// <param name="builder">The host builder, not null.</param>
    /// <param name="configure">The action to configure the worker, not null.</param>
    /// <returns>The original host builder with task hub worker configured.</returns>
    [Obsolete("Use IServiceCollection.AddTaskHubWorker instead. This method will be removed in a future version.")]
    public static IHostBuilder ConfigureTaskHubWorker(
        this IHostBuilder builder, Action<ITaskHubWorkerBuilder> configure)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);

        return builder.ConfigureTaskHubWorker(configure, _ => { });
    }

    /// <summary>
    /// Configures the task hub worker background service.
    /// </summary>
    /// <param name="builder">The host builder, not null.</param>
    /// <param name="configure">The action to configure the worker, not null.</param>
    /// <param name="configureOptions">The action to configure the task hub host options.</param>
    /// <returns>The original host builder with task hub worker configured.</returns>
    [Obsolete("Use IServiceCollection.AddTaskHubWorker instead. This method will be removed in a future version.")]
    public static IHostBuilder ConfigureTaskHubWorker(
        this IHostBuilder builder,
        Action<ITaskHubWorkerBuilder> configure,
        Action<TaskHubOptions> configureOptions)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);
        Check.NotNull(configureOptions);

        return builder.ConfigureTaskHubWorker((_, b) => configure(b), configureOptions);
    }

    /// <summary>
    /// Configures the task hub worker background service.
    /// </summary>
    /// <param name="builder">The host builder, not null.</param>
    /// <param name="configure">The action to configure the worker, not null.</param>
    /// <returns>The original host builder with task hub worker configured.</returns>
    [Obsolete("Use IServiceCollection.AddTaskHubWorker instead. This method will be removed in a future version.")]
    public static IHostBuilder ConfigureTaskHubWorker(
        this IHostBuilder builder, Action<HostBuilderContext, ITaskHubWorkerBuilder> configure)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);
        return builder.ConfigureTaskHubWorker(configure, _ => { });
    }

    /// <summary>
    /// Configures the task hub worker background service.
    /// </summary>
    /// <param name="builder">The host builder, not null.</param>
    /// <param name="configure">The action to configure the worker, not null.</param>
    /// <param name="configureOptions">The action to configure the task hub host options.</param>
    /// <returns>The original host builder with task hub worker configured.</returns>
    [Obsolete("Use IServiceCollection.AddTaskHubWorker instead. This method will be removed in a future version.")]
    public static IHostBuilder ConfigureTaskHubWorker(
        this IHostBuilder builder,
        Action<HostBuilderContext, ITaskHubWorkerBuilder> configure,
        Action<TaskHubOptions> configureOptions)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);
        Check.NotNull(configureOptions);

        builder.ConfigureServices((context, services) =>
        {
            ITaskHubWorkerBuilder b = services.AddTaskHubWorker();
            configure(context, b);
            b.Configure(configureOptions);
        });

        return builder;
    }
}
