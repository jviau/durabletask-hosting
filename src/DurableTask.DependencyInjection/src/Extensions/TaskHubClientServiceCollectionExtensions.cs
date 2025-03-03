// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using DurableTask.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection
{

    /// <summary>
    /// Extensions for <see cref="IServiceCollection" />.
    /// </summary>
    public static class TaskHubClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures Durable Task worker services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="name">The name of the builder to add.</param>
        /// <returns>The builder used to configured the <see cref="BaseTaskHubClient"/>.</returns>
        public static ITaskHubClientBuilder AddTaskHubClient(this IServiceCollection services, string? name = null)
        {
            Check.NotNull(services);
            ITaskHubClientBuilder builder = GetBuilder(services, name ?? Options.DefaultName, out bool added);
            ConditionalConfigureBuilder(services, builder, added);
            return builder;
        }

        /// <summary>
        /// Configures and adds a <see cref="BaseTaskHubClient" /> to the service collection.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="configure">The callback to configure the client.</param>
        /// <returns>The original service collection, for call chaining.</returns>
        public static IServiceCollection AddTaskHubClient(this IServiceCollection services, Action<ITaskHubClientBuilder> configure)
        {
            return services.AddTaskHubClient(Options.DefaultName, configure);
        }

        /// <summary>
        /// Configures and adds a <see cref="BaseTaskHubClient" /> to the service collection.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="name">Gets the name of the client to add.</param>
        /// <param name="configure">The callback to configure the client.</param>
        /// <returns>The original service collection, for call chaining.</returns>
        public static IServiceCollection AddTaskHubClient(this IServiceCollection services, string name, Action<ITaskHubClientBuilder> configure)
        {
            services.TryAddSingleton<ITaskHubClientProvider, DefaultTaskHubClientProvider>();
            ITaskHubClientBuilder builder = GetBuilder(services, name, out bool added);
            configure.Invoke(builder);
            ConditionalConfigureBuilder(services, builder, added);
            return services;
        }

        private static void ConditionalConfigureBuilder(IServiceCollection services, ITaskHubClientBuilder builder, bool configure)
        {
            if (!configure)
            {
                return;
            }

            // We do not want to register DurableTaskClient type directly so we can keep a max of 1 DurableTaskClients
            // registered, allowing for direct-DI of the default client.
            services.AddSingleton(sp => new DefaultTaskHubClientProvider.ClientContainer(builder.Build(sp)));

            if (builder.Name == Options.DefaultName)
            {
                // If we have the default options name here, we will inject this client directly.
                builder.RegisterDirectly();
            }
        }

        private static ITaskHubClientBuilder GetBuilder(IServiceCollection services, string name, out bool added)
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
        private class BuilderContainer
        {
            private readonly Dictionary<string, ITaskHubClientBuilder> _builders = [];
            private readonly IServiceCollection _services;

            public BuilderContainer(IServiceCollection services)
            {
                _services = services;
            }

            public ITaskHubClientBuilder GetOrAdd(string name, out bool added)
            {
                added = false;
                if (!_builders.TryGetValue(name, out ITaskHubClientBuilder builder))
                {
                    builder = new DefaultTaskHubClientBuilder(name, _services);
                    _builders[name] = builder;
                    added = true;
                }

                return builder;
            }
        }
    }
}
