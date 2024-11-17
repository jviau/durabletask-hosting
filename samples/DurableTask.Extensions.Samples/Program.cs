// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using DurableTask.Extensions;
using DurableTask.Extensions.Samples;
using DurableTask.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Can register DataConvert in service container, or in options below.
// builder.Services.AddSingleton<DataConverter>(new StjDataConverter());
builder.Services.AddSingleton<IConsole, ConsoleWrapper>();
builder.Services.AddHostedService<TaskEnqueuer>();

builder.ConfigureTaskHubWorker((context, builder) =>
{
    builder.WithOrchestrationService(new LocalOrchestrationService());
    builder.AddDurableExtensions(opt => opt.DataConverter = new StjDataConverter());
    builder.AddClient();
    builder.AddOrchestrationsFromAssembly<GreetingsOrchestration>(includePrivate: true);
    builder.AddActivitiesFromAssembly<GreetingsOrchestration>(includePrivate: true);
});

IHost host = builder.Build();

await host.RunAsync();

internal class TaskEnqueuer : BackgroundService
{
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
        OrchestrationInstance instance = await _client.StartOrchestrationAsync(
            _instanceId, new GreetingsOrchestration());

        await _client.WaitForOrchestrationAsync(instance, TimeSpan.FromSeconds(60), stoppingToken);

        _console.WriteLine("Orchestration finished.");
        _console.WriteLine("Press Ctrl+C to exit");
    }
}
