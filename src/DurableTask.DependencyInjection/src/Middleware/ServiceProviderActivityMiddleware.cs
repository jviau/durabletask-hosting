// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;

namespace DurableTask.DependencyInjection.Middleware;

/// <summary>
/// Middleware that constructs and injects the real orchestration or activity at the necessary time.
/// </summary>
public class ServiceProviderActivityMiddleware : ITaskMiddleware
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceProviderActivityMiddleware"/> class.
    /// A middleware that lazily sets the inner orchestration to be ran.
    /// </summary>
    /// <param name="serviceProvider">The service provider. Not null.</param>
    public ServiceProviderActivityMiddleware(IServiceProvider serviceProvider)
    {
        _serviceProvider = Check.NotNull(serviceProvider);
    }

    /// <inheritdoc />
    public async Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        Check.NotNull(context);
        Check.NotNull(next);

        TaskActivity taskActivity = context.GetProperty<TaskActivity>();

        if (taskActivity is WrapperActivity wrapper)
        {
            wrapper.Initialize(_serviceProvider);

            // update the context task activity with the real one.
            context.SetProperty(wrapper.InnerActivity);
            await next().ConfigureAwait(false);
            return;
        }

        await next().ConfigureAwait(false);
    }
}
