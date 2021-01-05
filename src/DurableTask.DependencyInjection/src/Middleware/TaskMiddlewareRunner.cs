// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection.Middleware
{
    /// <summary>
    /// Helper for executing task middleware.
    /// </summary>
    internal static class TaskMiddlewareRunner
    {
        private static readonly ConcurrentDictionary<
            TaskMiddlewareDescriptor, Func<IServiceProvider, ITaskMiddleware>> s_factories
            = new ConcurrentDictionary<
                TaskMiddlewareDescriptor, Func<IServiceProvider, ITaskMiddleware>>();

        /// <summary>
        /// Runs the middleware described by <see cref="TaskMiddlewareDescriptor"/>.
        /// </summary>
        /// <param name="descriptor">The task middleware to find and run.</param>
        /// <param name="context">The dispatch middleware context.</param>
        /// <param name="next">The next middleware callback.</param>
        /// <returns>A task that completes when the middleware has finished executing.</returns>
        public static Task RunAsync(
            TaskMiddlewareDescriptor descriptor, DispatchMiddlewareContext context, Func<Task> next)
        {
            Check.NotNull(descriptor, nameof(descriptor));
            Check.NotNull(context, nameof(context));
            Check.NotNull(next, nameof(next));

            IServiceScope scope = OrchestrationScope.GetScope(context.GetProperty<OrchestrationInstance>().InstanceId);
            IServiceProvider serviceProvider = scope.ServiceProvider;

            ITaskMiddleware middleware = null;
            if (!s_factories.TryGetValue(descriptor, out Func<IServiceProvider, ITaskMiddleware> factory))
            {
                if (descriptor.Func is object)
                {
                    factory = s_factories.GetOrAdd(descriptor, _ => new FuncMiddleware(descriptor.Func));
                    middleware = factory.Invoke(serviceProvider);
                }

                if (serviceProvider.GetService(descriptor.Type) is ITaskMiddleware fetchedMiddleware)
                {
                    s_factories[descriptor] = sp => (ITaskMiddleware)sp.GetRequiredService(descriptor.Type);
                    middleware = fetchedMiddleware;
                }
                else
                {
                    ObjectFactory objectFactory = ActivatorUtilities.CreateFactory(
                        descriptor.Type, Array.Empty<Type>());
                    factory = s_factories.GetOrAdd(
                        descriptor, sp => (ITaskMiddleware)objectFactory.Invoke(sp, Array.Empty<object>()));
                    middleware = factory.Invoke(serviceProvider);
                }
            }

            return middleware.InvokeAsync(context, next);
        }

        private class FuncMiddleware : ITaskMiddleware
        {
            private readonly Func<DispatchMiddlewareContext, Func<Task>, Task> _func;

            public FuncMiddleware(Func<DispatchMiddlewareContext, Func<Task>, Task> func)
            {
                _func = Check.NotNull(func, nameof(func));
            }

            public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
                => _func.Invoke(context, next);
        }
    }
}
