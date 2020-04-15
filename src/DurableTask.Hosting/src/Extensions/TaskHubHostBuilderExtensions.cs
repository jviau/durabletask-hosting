// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DurableTask.Hosting
{
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
        public static IHostBuilder ConfigureTaskHubWorker(this IHostBuilder builder, Action<ITaskHubWorkerBuilder> configure)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(configure, nameof(configure));

            return builder.ConfigureTaskHubWorker((_, b) => configure(b));
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
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(configure, nameof(configure));

            builder.ConfigureServices((context, services) =>
            {
                services.AddTaskHubWorker(taskHubBuilder => configure(context, taskHubBuilder));
                services.AddHostedService<TaskHubBackgroundService>();
            });

            return builder;
        }
    }
}
