// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Extension methods for adding task hub services to a service collection.
/// </summary>
public static class TaskHubServiceCollectionExtensions
{
    ///// <summary>
    ///// Adds a <see cref="TaskHubWorker"/> and related services to the service collection.
    ///// </summary>
    ///// <param name="services">The service collection to add to.</param>
    ///// <param name="configure">The action to configure the task hub builder with.</param>
    ///// <returns>The original service collection, with services added.</returns>
    //public static IServiceCollection AddTaskHubWorker(
    //    this IServiceCollection services, Action <ITaskHubWorkerBuilder> configure)
    //{
    //    Check.NotNull(services);
    //    Check.NotNull(configure);

    //    ITaskHubWorkerBuilder builder = services.AddTaskHubWorkerCore();
    //    configure(builder);

    //    return services;
    //}

    //private static ITaskHubWorkerBuilder AddTaskHubWorkerCore(this IServiceCollection services)
    //{
    //    services.AddLogging();

    //    // This is added as a singleton implementation instance as we will fetch this out of the service collection
    //    // during subsequent calls to AddTaskHubWorker.
    //    DefaultTaskHubWorkerBuilder builder = new(services);
    //    services.TryAddSingleton<ITaskHubWorkerBuilder>(builder);
    //    services.TryAddSingleton(sp => builder.Build(sp));

    //    return services.GetTaskHubBuilder();
    //}

    //private static ITaskHubWorkerBuilder GetTaskHubBuilder(this IServiceCollection services)
    //{
    //    return (ITaskHubWorkerBuilder)services.Single(sd => sd.ServiceType == typeof(ITaskHubWorkerBuilder))
    //        .ImplementationInstance;
    //}
}
