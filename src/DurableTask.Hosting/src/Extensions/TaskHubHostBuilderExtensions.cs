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
    /// <returns>The original host builder with task hub worker configured.</returns>
    public static IHostApplicationBuilder ConfigureTaskHubWorker(
        this IHostApplicationBuilder builder, Action<ITaskHubWorkerBuilder> configure)
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
    /// <param name="configureOptions">The action to configure the task hub host options.</param>
    /// <returns>The original host builder with task hub worker configured.</returns>
    public static IHostApplicationBuilder ConfigureTaskHubWorker(
        this IHostApplicationBuilder builder,
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
    /// <returns>The original host builder with task hub worker configured.</returns>
    public static IHostApplicationBuilder ConfigureTaskHubWorker(
        this IHostApplicationBuilder builder, Action<IHostApplicationBuilder, ITaskHubWorkerBuilder> configure)
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
            services.AddOptions();
            services.AddLogging();

            services
                .AddOptions<TaskHubOptions>()
                .Bind(context.Configuration.GetSection("TaskHub"))
                .Configure(configureOptions);

            services.AddTaskHubWorker(taskHubBuilder => configure(context, taskHubBuilder));
            services.AddHostedService<TaskHubBackgroundService>();
        });

        return builder;
    }

    /// <summary>
    /// Configures the task hub worker background service.
    /// </summary>
    /// <param name="builder">The host builder, not null.</param>
    /// <param name="configure">The action to configure the worker, not null.</param>
    /// <param name="configureOptions">The action to configure the task hub host options.</param>
    /// <returns>The original host builder with task hub worker configured.</returns>
    public static IHostApplicationBuilder ConfigureTaskHubWorker(
        this IHostApplicationBuilder builder,
        Action<IHostApplicationBuilder, ITaskHubWorkerBuilder> configure,
        Action<TaskHubOptions> configureOptions)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);
        Check.NotNull(configureOptions);

        builder.Services.AddOptions();
        builder.Services.AddLogging();

        builder.Services
            .AddOptions<TaskHubOptions>()
            .Bind(builder.Configuration.GetSection("TaskHub"))
            .Configure(configureOptions);

        builder.Services.AddTaskHubWorker(taskHubBuilder => configure(builder, taskHubBuilder));
        builder.Services.AddHostedService<TaskHubBackgroundService>();

        return builder;
    }
}
