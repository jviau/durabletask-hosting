// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;
using DurableTask.DependencyInjection.Middleware;
using DurableTask.DependencyInjection.Orchestrations;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DurableTask.DependencyInjection;

/// <summary>
/// The default builder for task hub worker.
/// </summary>
public class DefaultTaskHubWorkerBuilder : ITaskHubWorkerBuilder
{
    private Type? _buildTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTaskHubWorkerBuilder"/> class.
    /// </summary>
    /// <param name="name">The name for this builder.</param>
    /// <param name="services">The current service collection, not null.</param>
    public DefaultTaskHubWorkerBuilder(IServiceCollection services) :
        this(Options.DefaultName, services)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTaskHubWorkerBuilder"/> class.
    /// </summary>
    /// <param name="name">The name for this builder.</param>
    /// <param name="services">The current service collection, not null.</param>
    public DefaultTaskHubWorkerBuilder(string name, IServiceCollection services)
    {
        Name = name ?? Options.DefaultName;
        Services = Check.NotNull(services);
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public IOrchestrationService? OrchestrationService { get; set; }

    public Func<IServiceProvider, IOrchestrationService>? OrchestrationServiceFactory { get; set; }

    /// <inheritdoc/>
    public Type? BuildTarget
    {
        get => _buildTarget;
        set
        {
            if (value is not null)
            {
                Check.ConcreteType<BaseTaskHubWorker>(value);
            }

            _buildTarget = value;
        }
    }

    /// <inheritdoc />
    public IList<TaskMiddlewareDescriptor> ActivityMiddleware { get; } =
    [
        new(typeof(ServiceProviderActivityMiddleware)),
    ];

    /// <inheritdoc />
    public IList<TaskMiddlewareDescriptor> OrchestrationMiddleware { get; } =
    [
        new(typeof(ServiceProviderOrchestrationMiddleware)),
    ];

    /// <inheritdoc/>
    public IList<TaskActivityDescriptor> Activities { get; } = [];

    /// <inheritdoc/>
    public IList<TaskOrchestrationDescriptor> Orchestrations { get; } = [];

    /// <summary>
    /// Builds and returns a <see cref="TaskHubWorker"/> using the configurations from this instance.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A new <see cref="TaskHubWorker"/>.</returns>
    public IHostedService Build(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider);

        const string error = "No valid DurableTask worker target was registered. Ensure a valid worker has been"
            + " configured via 'UseBuildTarget(Type target)'.";
        //Check.NotNull(_buildTarget, error);

        IOrchestrationService orchestrationService = serviceProvider.GetService<IOrchestrationService>();
        if (orchestrationService is null && OrchestrationServiceFactory is not null)
        {
            orchestrationService = OrchestrationServiceFactory(serviceProvider);
        }
        const string error2 = "No valid OrchestrationService was registered. Ensure a valid OrchestrationService has been"
            + " configured via 'WithOrchestrationService(IOrchestrationService orchestrationService)'.";
        Check.NotNull(orchestrationService, error2);

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
            orchestrationService,
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

        return (IHostedService)ActivatorUtilities.CreateInstance(serviceProvider, _buildTarget, Name, worker, loggerFactory);
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
