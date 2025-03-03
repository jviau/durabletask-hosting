// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection" />.
    /// </summary>
    public static class TaskHubWorkerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures Durable Task worker services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="name">The name of the builder to add.</param>
        /// <returns>The builder used to configured the <see cref="ITaskHubWorkerBuilder"/>.</returns>
        public static ITaskHubWorkerBuilder AddTaskHubWorker(this IServiceCollection services, string? name = null)
        {
            Check.NotNull(services);
            ITaskHubWorkerBuilder builder = GetBuilder(services, name ?? Options.DefaultName, out bool added);
            ConditionalConfigureBuilder(services, builder, added);
            return builder;
        }

        /// <summary>
        /// Adds and configures Durable Task worker services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configure">The callback to configure the builder.</param>
        /// <returns>The service collection for call chaining.</returns>
        public static IServiceCollection AddTaskHubWorker(
            this IServiceCollection services, Action<ITaskHubWorkerBuilder> configure)
        {
            Check.NotNull(services);
            Check.NotNull(configure);
            return services.AddTaskHubWorker(Options.DefaultName, configure);
        }

        /// <summary>
        /// Adds and configures Durable Task worker services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="name">The name of the builder to add.</param>
        /// <param name="configure">The callback to configure the builder.</param>
        /// <returns>The service collection for call chaining.</returns>
        public static IServiceCollection AddTaskHubWorker(
            this IServiceCollection services, string name, Action<ITaskHubWorkerBuilder> configure)
        {
            Check.NotNull(services);
            Check.NotNull(name);
            Check.NotNull(configure);

            services.AddLogging();

            ITaskHubWorkerBuilder builder = GetBuilder(services, name, out bool added);
            configure.Invoke(builder);
            ConditionalConfigureBuilder(services, builder, added);
            return services;
        }

        /// <summary>
        /// Sets the build target for this builder. This is the hosted service which will ultimately be ran on host
        /// startup.
        /// </summary>
        /// <param name="builder">The builder to set the builder target for.</param>
        /// <param name="target">The type of target to set.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static ITaskHubWorkerBuilder UseBuildTarget(this ITaskHubWorkerBuilder builder, Type target)
        {
            Check.NotNull(builder);
            builder.BuildTarget = target;
            return builder;
        }

        /// <summary>
        /// Sets the build target for this builder. This is the hosted service which will ultimately be ran on host
        /// startup.
        /// </summary>
        /// <typeparam name="TTarget">The builder target type.</typeparam>
        /// <param name="builder">The builder to set the builder target for.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static ITaskHubWorkerBuilder UseBuildTarget<TTarget>(this ITaskHubWorkerBuilder builder)
            where TTarget : BaseTaskHubWorker
            => builder.UseBuildTarget(typeof(TTarget));

        private static void ConditionalConfigureBuilder(
            IServiceCollection services, ITaskHubWorkerBuilder builder, bool configure)
        {
            if (!configure)
            {
                return;
            }

            services.AddSingleton(sp => builder.Build(sp));
        }

        private static ITaskHubWorkerBuilder GetBuilder(IServiceCollection services, string name, out bool added)
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
            private readonly Dictionary<string, ITaskHubWorkerBuilder> _builders = [];
            private readonly IServiceCollection _services = services;

            public ITaskHubWorkerBuilder GetOrAdd(string name, out bool added)
            {
                added = false;
                if (!_builders.TryGetValue(name, out ITaskHubWorkerBuilder builder))
                {
                    builder = new DefaultTaskHubWorkerBuilder(name, _services);
                    _builders[name] = builder;
                    added = true;
                }

                return builder;
            }
        }
    }
}
