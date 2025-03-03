// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DurableTask.DependencyInjection;

/// <summary>
/// The default builder for task hub client.
/// </summary>
public class DefaultTaskHubClientBuilder : ITaskHubClientBuilder
{
    private Type? _buildTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTaskHubClientBuilder"/> class.
    /// </summary>
    /// <param name="name">The name for this builder.</param>
    /// <param name="services">The current service collection, not null.</param>
    public DefaultTaskHubClientBuilder(string name, IServiceCollection services)
    {
        Name = name ?? Options.DefaultName;
        Services = Check.NotNull(services);

    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc/>
    public Type? BuildTarget
    {
        get => _buildTarget;
        set
        {
            if (value is not null)
            {
                Check.ConcreteType<BaseTaskHubClient>(value);
            }

            _buildTarget = value;
        }
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    public Func<IServiceProvider, IOrchestrationServiceClient>? OrchestrationServiceFactory { get; set; }

    /// <summary>
    /// Builds and returns a <see cref="TaskHubWorker"/> using the configurations from this instance.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A new <see cref="TaskHubWorker"/>.</returns>
    public BaseTaskHubClient Build(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider);

        const string error = "No valid DurableTask client target was registered. Ensure a valid client has been"
            + " configured via 'UseBuildTarget(Type target)'.";
        Check.NotNull(_buildTarget, error);

        IOrchestrationServiceClient orchestrationService = serviceProvider.GetService<IOrchestrationServiceClient>();
        if (orchestrationService is null && OrchestrationServiceFactory is not null)
        {
            orchestrationService = OrchestrationServiceFactory(serviceProvider);
        }
        const string error2 = "No valid OrchestrationServiceClient was registered. Ensure a valid OrchestrationServiceClient has been"
            + " configured via 'WithOrchestrationService(IOrchestrationServiceClient orchestrationService)'.";
        Check.NotNull(orchestrationService, error2);
        
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Options does not have to be present.
        IOptions<TaskHubClientOptions> options = serviceProvider.GetService<IOptions<TaskHubClientOptions>>();
        DataConverter converter = options?.Value?.DataConverter ?? JsonDataConverter.Default;

        var client = new TaskHubClient(orchestrationService, converter, loggerFactory);

        return (BaseTaskHubClient)ActivatorUtilities.CreateInstance(serviceProvider, _buildTarget, Name, client);
    }
}
