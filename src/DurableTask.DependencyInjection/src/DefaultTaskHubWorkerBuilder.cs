﻿// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;
using DurableTask.DependencyInjection.Middleware;
using DurableTask.DependencyInjection.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// The default builder for task hub worker.
    /// </summary>
    public class DefaultTaskHubWorkerBuilder : ITaskHubWorkerBuilder
    {
        private readonly TaskHubCollection _activities = new TaskHubCollection();
        private readonly TaskHubCollection _orchestrations = new TaskHubCollection();
        private readonly List<Type> _activitiesMiddleware = new List<Type>();
        private readonly List<Type> _orchestrationsMiddleware = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskHubWorkerBuilder"/> class.
        /// </summary>
        /// <param name="services">The current service collection, not null.</param>
        public DefaultTaskHubWorkerBuilder(IServiceCollection services)
        {
            Services = Check.NotNull(services, nameof(services));
            Services.AddScoped<ServiceProviderOrchestrationMiddleware>();
            Services.AddScoped<ServiceProviderActivityMiddleware>();
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <summary>
        /// Gets or sets the current orchestration service.
        /// </summary>
        public IOrchestrationService OrchestrationService { get; set; }

        /// <inheritdoc />
        public ITaskHubWorkerBuilder AddActivity(TaskActivityDescriptor descriptor)
        {
            Check.NotNull(descriptor, nameof(descriptor));

            Services.TryAdd(descriptor.Descriptor);
            _activities.Add(descriptor);
            return this;
        }

        /// <inheritdoc />
        public ITaskHubWorkerBuilder UseActivityMiddleware(TaskMiddlewareDescriptor descriptor)
        {
            Check.NotNull(descriptor, nameof(descriptor));

            Services.TryAdd(descriptor.Descriptor);
            _activitiesMiddleware.Add(descriptor.Type);
            return this;
        }

        /// <inheritdoc />
        public ITaskHubWorkerBuilder AddOrchestration(TaskOrchestrationDescriptor descriptor)
        {
            Check.NotNull(descriptor, nameof(descriptor));

            Services.TryAdd(descriptor.Descriptor);
            _orchestrations.Add(descriptor);
            return this;
        }

        /// <inheritdoc />
        public ITaskHubWorkerBuilder UseOrchestrationMiddleware(TaskMiddlewareDescriptor descriptor)
        {
            Check.NotNull(descriptor, nameof(descriptor));

            Services.TryAdd(descriptor.Descriptor);
            _orchestrationsMiddleware.Add(descriptor.Type);
            return this;
        }

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
                throw new InvalidOperationException("Builder is not fully configured yet. OrchestrationService is null.");
            }

            var worker = new TaskHubWorker(
                OrchestrationService,
                new WrapperObjectManager<TaskOrchestration>(_orchestrations, type => new WrapperOrchestration(type)),
                new WrapperObjectManager<TaskActivity>(_activities, type => new WrapperActivity(type)));

            // The first middleware added begins the service scope for all further middleware, the orchestration, and activities.
            worker.AddOrchestrationDispatcherMiddleware(BeginMiddlewareScope(serviceProvider));
            worker.AddOrchestrationDispatcherMiddleware(WrapMiddleware(typeof(ServiceProviderOrchestrationMiddleware)));
            foreach (Type middlewareType in _orchestrationsMiddleware)
            {
                worker.AddOrchestrationDispatcherMiddleware(WrapMiddleware(middlewareType));
            }

            worker.AddActivityDispatcherMiddleware(WrapMiddleware(typeof(ServiceProviderActivityMiddleware)));
            foreach (Type middlewareType in _activitiesMiddleware)
            {
                worker.AddActivityDispatcherMiddleware(WrapMiddleware(middlewareType));
            }

            return worker;
        }

        private static Func<DispatchMiddlewareContext, Func<Task>, Task> WrapMiddleware(Type middlewareType)
        {
            return (context, next) =>
            {
                IServiceScope scope = OrchestrationScope.GetScope(context.GetProperty<OrchestrationInstance>());
                var middleware = (ITaskMiddleware)scope.ServiceProvider.GetRequiredService(middlewareType);
                return middleware.InvokeAsync(context, next);
            };
        }

        private static Func<DispatchMiddlewareContext, Func<Task>, Task> BeginMiddlewareScope(IServiceProvider serviceProvider)
        {
            return async (context, next) =>
            {
                IOrchestrationScope scope = null;
                try
                {
                    scope = OrchestrationScope.CreateScope(context.GetProperty<OrchestrationInstance>(), serviceProvider);
                    await next().ConfigureAwait(false);
                }
                finally
                {
                    scope?.SignalMiddlewareCompletion();
                }
            };
        }
    }
}
