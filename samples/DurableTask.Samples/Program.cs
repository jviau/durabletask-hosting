// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using DurableTask.Samples;
using DurableTask.Samples.Generics;
using DurableTask.Samples.Greetings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<SampleMiddleware>();

builder.Services.AddTaskHubWorker()
    .Configure(o => o.CreateIfNotExists = true)
    .UseOrchestrationService(new LocalOrchestrationService())
    .UseOrchestrationMiddleware<SampleMiddleware>()
    .UseActivityMiddleware<SampleMiddleware>()
    .AddOrchestration<GreetingsOrchestration>()
    .AddOrchestration<GenericOrchestrationRunner>()
    .AddActivitiesFromAssembly<SampleMiddleware>()
    .AddClient();

builder.Services
    .AddSingleton<IConsole, ConsoleWrapper>()
    .AddHostedService<TaskEnqueuer>();

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
        OrchestrationInstance instance = await _client.CreateOrchestrationInstanceAsync(
            NameVersionHelper.GetDefaultName(typeof(GenericOrchestrationRunner)),
            NameVersionHelper.GetDefaultVersion(typeof(GenericOrchestrationRunner)),
            _instanceId,
            null,
            new Dictionary<string, string>()
            {
                ["CorrelationId"] = Guid.NewGuid().ToString(),
            });

        OrchestrationState result = await _client.WaitForOrchestrationAsync(
            instance, TimeSpan.FromSeconds(60), stoppingToken);

        _console.WriteLine();
        _console.WriteLine($"Orchestration finished.");
        _console.WriteLine($"Run stats: {result.Status}");
        _console.WriteLine("Press Ctrl+C to exit");
    }
}
