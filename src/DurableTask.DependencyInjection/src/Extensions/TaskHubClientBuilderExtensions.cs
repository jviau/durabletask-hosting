// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection.Extensions
{
    public static class TaskHubClientBuilderExtensions
    {
        /// <summary>
        /// Sets the provided <paramref name="orchestrationService"/> to the <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The task hub builder.</param>
        /// <param name="orchestrationService">The orchestration service to use.</param>
        /// <returns>The original builder, with orchestration service set.</returns>
        public static ITaskHubClientBuilder WithOrchestrationService(
            this ITaskHubClientBuilder builder, IOrchestrationServiceClient orchestrationService)
        {
            Check.NotNull(builder);
            Check.NotNull(orchestrationService);
            builder.OrchestrationServiceFactory = (sp) => orchestrationService;
            return builder;
        }

        /// <summary>
        /// Sets the provided <paramref name="orchestrationServiceFactory"/> to the <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The task hub builder.</param>
        /// <param name="orchestrationServiceFactory">The orchestration service factory to use.</param>
        /// <returns>The original builder, with orchestration service set.</returns>
        public static ITaskHubClientBuilder WithOrchestrationService(
            this ITaskHubClientBuilder builder, Func<IServiceProvider, IOrchestrationServiceClient> orchestrationServiceFactory)
        {
            Check.NotNull(builder);
            Check.NotNull(orchestrationServiceFactory);
            builder.OrchestrationServiceFactory = orchestrationServiceFactory;
            return builder;
        }

        /// <summary>
        /// Registers this builders <see cref="BaseTaskHubClient" /> directly to the service container. This will allow for
        /// directly importing <see cref="BaseTaskHubClient" />. This can <b>only</b> be used for a single builder. Only
        /// the first call will register.
        /// </summary>
        /// <param name="builder">The builder to register the client directly of.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static ITaskHubClientBuilder RegisterDirectly(this ITaskHubClientBuilder builder)
        {
            BaseTaskHubClient GetClient(IServiceProvider services)
            {
                ITaskHubClientProvider provider = services.GetRequiredService<ITaskHubClientProvider>();
                return provider.GetClient(builder.Name);
            }

            builder.Services.TryAddSingleton(GetClient);
            return builder;
        }

        /// <summary>
        /// Sets the build target for this builder.
        /// startup.
        /// </summary>
        /// <param name="builder">The builder to set the builder target for.</param>
        /// <param name="target">The type of target to set.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static ITaskHubClientBuilder UseBuildTarget(this ITaskHubClientBuilder builder, Type target)
        {
            builder.BuildTarget = target;
            return builder;
        }

        /// <summary>
        /// Sets the build target for this builder.
        /// startup.
        /// </summary>
        /// <typeparam name="TTarget">The builder target type.</typeparam>
        /// <param name="builder">The builder to set the builder target for.</param>
        /// <returns>The original builder, for call chaining.</returns>
        public static ITaskHubClientBuilder UseBuildTarget<TTarget>(this ITaskHubClientBuilder builder)
            where TTarget : BaseTaskHubClient
            => builder.UseBuildTarget(typeof(TTarget));
    }
}
