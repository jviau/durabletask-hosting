// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using DurableTask.AzureStorage;
using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.DependencyInjection.Extensions;
using DurableTask.Extensions;
using DurableTask.Extensions.Samples;
using DurableTask.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Sample"))
    .AddDurableTaskInstrumentation()
    .AddSource("DurableTask.Instrumentation.Samples")
    .AddConsoleExporter()
    .AddZipkinExporter()
    .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        IOrchestrationService orchestrationService = GetOrchestrationService();

        services.AddTaskHubWorker((builder) =>
        {
            builder
                .UseBuildTarget<DurableTaskHubWorker>()
                .WithOrchestrationService(orchestrationService)

                .AddDurableExtensions()
                .AddDurableInstrumentation()

                .AddOrchestrationsFromAssembly<TopOrchestration>(includePrivate: true)
                .AddActivitiesFromAssembly<TopOrchestration>(includePrivate: true);
        });
        services.AddTaskHubClient((builder) =>
        {
            builder
                .UseBuildTarget<DurableTaskHubClient>()
                .WithOrchestrationService((IOrchestrationServiceClient)orchestrationService);
        });

        services.AddSingleton<IConsole, ConsoleWrapper>();
        services.AddHostedService<TaskEnqueuer>();

    })
    //.ConfigureTaskHubWorker((context, builder) =>
    //{
    //    builder.WithOrchestrationService(GetOrchestrationService());
    //    builder.AddDurableExtensions();
    //    builder.AddDurableInstrumentation();
    //    //builder.AddClient();
    //    builder.AddOrchestrationsFromAssembly<TopOrchestration>(includePrivate: true);
    //    builder.AddActivitiesFromAssembly<TopOrchestration>(includePrivate: true);
    //})
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();

static IOrchestrationService GetOrchestrationService()
{
    var settings = new AzureStorageOrchestrationServiceSettings
    {
        TaskHubName = "InstrumentationSampleHub",
        StorageConnectionString = "UseDevelopmentStorage=true",
    };

    return new AzureStorageOrchestrationService(settings);
}

internal class TaskEnqueuer : BackgroundService
{
    private readonly ActivitySource _source = new("DurableTask.Instrumentation.Samples", "0.1");
    private readonly TaskHubClient _client;
    private readonly IConsole _console;
    private readonly string _instanceId = Guid.NewGuid().ToString();

    public TaskEnqueuer(ITaskHubClientProvider clientProvider, IConsole console)
    {
        _client = ((DurableTaskHubClient)clientProvider.GetClient()).Client;
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using Activity activity = _source.StartActivity("run_sample", ActivityKind.Internal);
        OrchestrationInstance instance = await _client.StartOrchestrationAsync(
            _instanceId, new TopOrchestration.Request());

        await _client.WaitForOrchestrationAsync(instance, TimeSpan.FromSeconds(60), stoppingToken);

        _console.WriteLine("Orchestration finished.");
        _console.WriteLine("Press Ctrl+C to exit");
    }
}
