// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;
using DurableTask.DependencyInjection.Extensions;
using DurableTask.DependencyInjection.Middleware;
using DurableTask.DependencyInjection.Orchestrations;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// The default builder for task hub worker.
    /// </summary>
    public class DefaultTaskHubWorkerBuilder : ITaskHubWorkerBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskHubWorkerBuilder"/> class.
        /// </summary>
        /// <param name="services">The current service collection, not null.</param>
        public DefaultTaskHubWorkerBuilder(IServiceCollection services)
        {
            Services = Check.NotNull(services, nameof(services));
            Services.TryAddScoped<ServiceProviderActivityMiddleware>();
            Services.TryAddScoped<ServiceProviderOrchestrationMiddleware>();
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public IOrchestrationService OrchestrationService { get; set; }

        /// <inheritdoc />
        public IList<TaskMiddlewareDescriptor> ActivityMiddleware { get; } = new List<TaskMiddlewareDescriptor>
        {
            new TaskMiddlewareDescriptor(typeof(ServiceProviderActivityMiddleware)),
        };

        /// <inheritdoc />
        public IList<TaskMiddlewareDescriptor> OrchestrationMiddleware { get; } = new List<TaskMiddlewareDescriptor>
        {
            new TaskMiddlewareDescriptor(typeof(ServiceProviderOrchestrationMiddleware)),
        };

        /// <inheritdoc/>
        public IList<TaskActivityDescriptor> Activities { get; } = new List<TaskActivityDescriptor>();

        /// <inheritdoc/>
        public IList<TaskOrchestrationDescriptor> Orchestrations { get; } = new List<TaskOrchestrationDescriptor>();

        /// <summary>
        /// Builds and returns a <see cref="TaskHubWorker"/> using the configurations from this instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>A new <see cref="TaskHubWorker"/>.</returns>
        public TaskHubWorker Build(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            if (OrchestrationService == null)
            {
                throw new InvalidOperationException(Strings.OrchestrationInstanceNull);
            }

            // Verify we still have our ServiceProvider middleware
            if (OrchestrationMiddleware.FirstOrDefault(x => x.Type == typeof(ServiceProviderOrchestrationMiddleware)) is null)
            {
                throw new InvalidOperationException(Strings.ExpectedMiddlewareMissing(
                    typeof(ServiceProviderOrchestrationMiddleware), nameof(OrchestrationMiddleware)));
            }

            if (ActivityMiddleware.FirstOrDefault(x => x.Type == typeof(ServiceProviderActivityMiddleware)) is null)
            {
                throw new InvalidOperationException(Strings.ExpectedMiddlewareMissing(
                    typeof(ServiceProviderActivityMiddleware), nameof(ActivityMiddleware)));
            }

            var worker = new TaskHubWorker(
                OrchestrationService,
                new WrapperObjectManager<TaskOrchestration>(
                    new TaskHubCollection<TaskOrchestration>(Orchestrations), type => new WrapperOrchestration(type)),
                new WrapperObjectManager<TaskActivity>(
                    new TaskHubCollection<TaskActivity>(Activities), type => new WrapperActivity(type)));

            // The first middleware added begins the service scope for all further middleware, the orchestration, and activities.
            worker.AddOrchestrationDispatcherMiddleware(BeginMiddlewareScope(serviceProvider));
            foreach (Type middlewareType in OrchestrationMiddleware.Select(x => x.Type))
            {
                worker.AddOrchestrationDispatcherMiddleware(WrapMiddleware(middlewareType));
            }

            worker.AddActivityDispatcherMiddleware(BeginMiddlewareScope(serviceProvider));
            foreach (Type middlewareType in ActivityMiddleware.Select(x => x.Type))
            {
                worker.AddActivityDispatcherMiddleware(WrapMiddleware(middlewareType));
            }

            return worker;
        }

        private static Func<DispatchMiddlewareContext, Func<Task>, Task> WrapMiddleware(Type middlewareType)
        {
            return (context, next) =>
            {
                IServiceScope scope = OrchestrationScope.GetScope(
                    context.GetProperty<OrchestrationInstance>().InstanceId);
                var middleware = (ITaskMiddleware)scope.ServiceProvider.GetServiceOrCreateInstance(middlewareType);
                return middleware.InvokeAsync(context, next);
            };
        }

        private static Func<DispatchMiddlewareContext, Func<Task>, Task> BeginMiddlewareScope(IServiceProvider serviceProvider)
        {
            return async (context, next) =>
            {
                IOrchestrationScope scope = OrchestrationScope.GetOrCreateScope(
                    context.GetProperty<OrchestrationInstance>().InstanceId, serviceProvider);

                using (scope.Enter())
                {
                    await next().ConfigureAwait(false);
                }
            };
        }
    }
}
