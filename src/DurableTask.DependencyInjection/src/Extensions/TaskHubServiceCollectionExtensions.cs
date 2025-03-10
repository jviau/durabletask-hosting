// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection.Extensions;
using DurableTask.Hosting;
using DurableTask.Hosting.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TaskHubOptions>, ConfigureTaskHubOptions>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TaskHubOptions>, PostConfigureTaskHubOptions>());

            services.AddSingleton(sp =>
            {
                TaskHubOptions options = sp.GetOptions<TaskHubOptions>(name);
                ILogger<TaskHubBackgroundService> logger = sp.CreateLogger<TaskHubBackgroundService>();

                // Options.Create for back-compat / avoid adding a new ctor.
                return new TaskHubBackgroundService(builder.Build(sp), logger, Options.Create(options));
            });
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

        public IReadOnlyCollection<string> Names => _builders.Keys;

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
                builder = new DefaultTaskHubWorkerBuilder(services) { Name = name };
                _builders[name] = builder;
                added = true;
            }

            return builder;
        }
    }

    private class ConfigureTaskHubOptions(IConfiguration configuration, BuilderContainer container)
        : IConfigureNamedOptions<TaskHubOptions>
    {
        public void Configure(string name, TaskHubOptions options)
        {
            // [legacy behavior] When only one default TaskHub is configured, we will use just "TaskHub".
            // Otherwise we will use "Default" for the default TaskHub.
            if (string.IsNullOrEmpty(name) && container.Names.Count == 1)
            {
                configuration.Bind("TaskHub", options);
            }

            string section = string.IsNullOrEmpty(name) ? "TaskHub:Default" : $"TaskHub:{name}";
            configuration.Bind(section, options);
        }

        public void Configure(TaskHubOptions options) => Configure(Options.DefaultName, options);
    }

    private class PostConfigureTaskHubOptions(IServiceProvider serviceProvider) : IPostConfigureOptions<TaskHubOptions>
    {
        public void PostConfigure(string name, TaskHubOptions options)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return;
            }

            options.OrchestrationService ??= serviceProvider.GetService<IOrchestrationService>();
            options.OrchestrationServiceClient ??= serviceProvider.GetService<IOrchestrationServiceClient>();
        }
    }
}
