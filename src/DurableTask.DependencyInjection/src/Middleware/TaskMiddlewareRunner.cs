// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Extensions;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DurableTask.DependencyInjection.Middleware;

/// <summary>
/// Helper for executing task middleware.
/// </summary>
internal static class TaskMiddlewareRunner
{
    private static readonly ConcurrentDictionary<
        TaskMiddlewareDescriptor, Func<IServiceProvider, ITaskMiddleware>> s_factories = new();

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

        IServiceProvider serviceProvider = context.GetProperty<IServiceProvider>();
        ITaskMiddleware middleware = GetMiddleware(descriptor, serviceProvider);
        if (middleware is null)
        {
            // This _should not_ be possible, as TaskMiddlewareDescriptor is designed to only be constructable with
            // valid values. But a good sanity check here.
            ILogger logger = serviceProvider.CreateLogger(typeof(TaskMiddlewareRunner));
            string message = Strings.MiddlewareCreationFailed(descriptor);
            logger.LogError(message);
            throw new InvalidOperationException(message); // TODO: maybe a better exception type.
        }

        return middleware.InvokeAsync(context, next);
    }

    private static ITaskMiddleware GetMiddleware(
        TaskMiddlewareDescriptor descriptor, IServiceProvider serviceProvider)
    {
        if (!s_factories.TryGetValue(descriptor, out Func<IServiceProvider, ITaskMiddleware> factory))
        {
            if (descriptor.Func is not null)
            {
                FuncMiddleware middleware = new(descriptor.Func);
                factory = s_factories.GetOrAdd(descriptor, _ => middleware);
                return middleware;
            }
            else if (serviceProvider.GetService(descriptor.Type) is ITaskMiddleware fetchedMiddleware)
            {
                s_factories[descriptor] = sp => (ITaskMiddleware)sp.GetRequiredService(descriptor.Type);
                return fetchedMiddleware;
            }
            else
            {
                ObjectFactory objectFactory = ActivatorUtilities.CreateFactory(
                    descriptor.Type, Array.Empty<Type>());
                factory = s_factories.GetOrAdd(
                    descriptor, sp => (ITaskMiddleware)objectFactory.Invoke(sp, Array.Empty<object>()));
                return factory.Invoke(serviceProvider);
            }
        }

        return factory.Invoke(serviceProvider);
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
