// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;
using DurableTask.DependencyInjection.Extensions;
using DurableTask.DependencyInjection.Middleware;
using DurableTask.DependencyInjection.Orchestrations;
using DurableTask.DependencyInjection.Properties;
using DurableTask.Hosting.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection;

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
        Services = Check.NotNull(services);
    }

    /// <summary>
    /// Gets the name of this builder.
    /// </summary>
    public string Name { get; init; } = Options.DefaultName;

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public IOrchestrationService? OrchestrationService { get; set; }

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
    /// Gets or sets if a client as been added for this worker.
    /// </summary>
    internal bool ClientAdded { get; set; }

    /// <summary>
    /// Builds and returns a <see cref="TaskHubWorker"/> using the configurations from this instance.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A new <see cref="TaskHubWorker"/>.</returns>
    public TaskHubWorker Build(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider);

        if (OrchestrationService is null)
        {
            TaskHubOptions options = serviceProvider.GetOptions<TaskHubOptions>(Name);
            OrchestrationService = options.OrchestrationService;

            // retain legacy behavior of getting from service provider for default worker only.
            if (OrchestrationService is null && string.IsNullOrEmpty(Name))
            {
                OrchestrationService = serviceProvider.GetService<IOrchestrationService>();
            }
        }

        if (OrchestrationService is null)
        {
            throw new InvalidOperationException($"OrchestrationService is not configured for TaskHubWorker '{Name}'.");
        }

        // Verify we still have our ServiceProvider middleware
        if (OrchestrationMiddleware.FirstOrDefault(x => x.Type == typeof(ServiceProviderOrchestrationMiddleware))
            is null)
        {
            throw new InvalidOperationException(Strings.ExpectedMiddlewareMissing(
                typeof(ServiceProviderOrchestrationMiddleware), nameof(OrchestrationMiddleware)));
        }

        if (ActivityMiddleware.FirstOrDefault(x => x.Type == typeof(ServiceProviderActivityMiddleware)) is null)
        {
            throw new InvalidOperationException(Strings.ExpectedMiddlewareMissing(
                typeof(ServiceProviderActivityMiddleware), nameof(ActivityMiddleware)));
        }

        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        TaskHubWorker worker = new(
            OrchestrationService,
            new GenericObjectManager<TaskOrchestration>(),
            new GenericObjectManager<TaskActivity>(),
            loggerFactory);

        worker.AddTaskOrchestrations(Orchestrations.Select(x => new OrchestrationObjectCreator(x)).ToArray());
        worker.AddTaskActivities(Activities.Select(x => new ActivityObjectCreator(x)).ToArray());

        // The first middleware added begins the service scope for all further middleware, the orchestration, and
        // activities.
        worker.AddOrchestrationDispatcherMiddleware(BeginMiddlewareScope(serviceProvider));
        foreach (TaskMiddlewareDescriptor middlewareDescriptor in OrchestrationMiddleware)
        {
            worker.AddOrchestrationDispatcherMiddleware(WrapMiddleware(middlewareDescriptor));
        }

        worker.AddActivityDispatcherMiddleware(BeginMiddlewareScope(serviceProvider));
        foreach (TaskMiddlewareDescriptor middlewareDescriptor in ActivityMiddleware)
        {
            worker.AddActivityDispatcherMiddleware(WrapMiddleware(middlewareDescriptor));
        }

        return worker;
    }

    private static Func<DispatchMiddlewareContext, Func<Task>, Task> WrapMiddleware(
        TaskMiddlewareDescriptor descriptor)
    {
        return (context, next) => TaskMiddlewareRunner.RunAsync(descriptor, context, next);
    }

    private static Func<DispatchMiddlewareContext, Func<Task>, Task> BeginMiddlewareScope(
        IServiceProvider serviceProvider)
    {
        return async (context, next) =>
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            context.SetProperty(scope.ServiceProvider);
            await next().ConfigureAwait(false);
        };
    }
}
