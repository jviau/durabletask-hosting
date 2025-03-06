// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Hosting;
using DurableTask.Hosting.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Extension methods for adding task hub services to a service collection.
/// </summary>
public static class TaskHubServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="TaskHubWorker"/> and related services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="name">The name of the task hub worker.</param>
    /// <returns>The original service collection, with services added.</returns>
    public static ITaskHubWorkerBuilder AddTaskHubWorker(this IServiceCollection services, string? name = null)
    {
        Check.NotNull(services);
        name ??= Options.DefaultName;

        DefaultTaskHubWorkerBuilder builder = GetBuilder(services, name, out bool added);

        if (added)
        {
            services.AddLogging();
            services.AddOptions();
            string section = string.IsNullOrEmpty(name) ? "TaskHub" : $"TaskHub:{name}";
            services
                .AddOptions<TaskHubOptions>(name)
                .Configure<IConfiguration>((options, config) => config.Bind(section, options));

            services.AddSingleton(sp =>
                ActivatorUtilities.CreateInstance<TaskHubBackgroundService>(sp, builder.Build(sp)));
        }

        return builder;
    }

    /// <summary>
    /// Adds a <see cref="TaskHubWorker"/> and related services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="configure">The action to configure the task hub builder with.</param>
    /// <returns>The original service collection, with services added.</returns>
    public static IServiceCollection AddTaskHubWorker(
        this IServiceCollection services, Action<ITaskHubWorkerBuilder> configure)
        => services.AddTaskHubWorker(Options.DefaultName, configure);

    /// <summary>
    /// Adds a <see cref="TaskHubWorker"/> and related services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="name">The name of the task hub worker.</param>
    /// <param name="configure">The action to configure the task hub builder with.</param>
    /// <returns>The original service collection, with services added.</returns>
    public static IServiceCollection AddTaskHubWorker(
        this IServiceCollection services, string name, Action<ITaskHubWorkerBuilder> configure)
    {
        Check.NotNull(services);
        Check.NotNull(configure);

        ITaskHubWorkerBuilder builder = services.AddTaskHubWorker(name);
        configure(builder);

        return services;
    }

    private static DefaultTaskHubWorkerBuilder GetBuilder(IServiceCollection services, string name, out bool added)
    {
        // To ensure the builders are tracked with this service collection, we use a singleton service descriptor as a
        // holder for all builders.
        ServiceDescriptor descriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(BuilderContainer));

        if (descriptor is null)
        {
            descriptor = ServiceDescriptor.Singleton(new BuilderContainer(services));
            services.Add(descriptor);
        }

        var container = (BuilderContainer)descriptor.ImplementationInstance!;
        return container.GetOrAdd(name, out added);
    }

    /// <summary>
    /// A container which is used to store and retrieve builders from within the <see cref="IServiceCollection" />.
    /// </summary>
    private class BuilderContainer(IServiceCollection services)
    {
        private readonly Dictionary<string, DefaultTaskHubWorkerBuilder> _builders = [];

        public DefaultTaskHubWorkerBuilder GetOrAdd(string name, out bool added)
        {
            if (string.IsNullOrEmpty(name) && _builders.Keys.Any(x => !string.IsNullOrEmpty(x)))
            {
                throw new InvalidOperationException(
                    "An unnamed TaskHubWOrker cannot be added when a named TaskHubWorker already exists.");
            }

            if (!string.IsNullOrEmpty(name) && _builders.ContainsKey(string.Empty))
            {
                throw new InvalidOperationException(
                    "A named TaskHubWorker cannot be added when an unnamed TaskHubWorker already exists.");
            }

            added = false;
            if (!_builders.TryGetValue(name, out DefaultTaskHubWorkerBuilder builder))
            {
                builder = new DefaultTaskHubWorkerBuilder(services);
                _builders[name] = builder;
                added = true;
            }

            return builder;
        }
    }
}
