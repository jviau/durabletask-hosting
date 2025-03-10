// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using DurableTask.AzureStorage;
using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Extensions;
using DurableTask.Extensions.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTaskHubWorker()
    .Configure(o => o.CreateIfNotExists = true)
    .UseOrchestrationService(GetOrchestrationService())
    .AddOrchestrationsFromAssembly<TopOrchestration>(includePrivate: true)
    .AddActivitiesFromAssembly<TopOrchestration>(includePrivate: true)
    .AddDurableExtensions()
    .AddDurableInstrumentation()
    .AddClient();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("Sample"))
    .WithTracing(b =>
    {
        b.AddSource("DurableTask.Instrumentation.Samples")
            .AddDurableTaskInstrumentation()
            .AddConsoleExporter()
            .AddZipkinExporter();
    });

builder.Services
    .AddSingleton<IConsole, ConsoleWrapper>()
    .AddHostedService<TaskEnqueuer>();

IHost host = builder.Build();
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

    public TaskEnqueuer(TaskHubClient client, IConsole console)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
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
