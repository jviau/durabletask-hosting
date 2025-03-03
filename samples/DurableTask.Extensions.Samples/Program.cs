// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.DependencyInjection.Extensions;
using DurableTask.Emulator;
using DurableTask.Extensions;
using DurableTask.Extensions.Samples;
using DurableTask.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Can register DataConvert in service container, or in options below.
        //services.AddSingleton<DataConverter>(new StjDataConverter());
        services.AddSingleton<IConsole, ConsoleWrapper>();
        services.AddHostedService<TaskEnqueuer>();

        IOrchestrationService orchestrationService = new LocalOrchestrationService();

        services.AddTaskHubWorker((builder) =>
        {
            builder
                .UseBuildTarget<DurableTaskHubWorker>()
                .WithOrchestrationService(orchestrationService)

                .AddDurableExtensions(opt => opt.DataConverter = new StjDataConverter())

                .AddOrchestrationsFromAssembly<GreetingsOrchestration>(includePrivate: true)
                .AddActivitiesFromAssembly<GreetingsOrchestration>(includePrivate: true);
        });
        services.AddTaskHubClient((builder) =>
        {
            builder
                .UseBuildTarget<DurableTaskHubClient>()
                .WithOrchestrationService((IOrchestrationServiceClient)orchestrationService);
        });

    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();

internal class TaskEnqueuer : BackgroundService
{
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
        OrchestrationInstance instance = await _client.StartOrchestrationAsync(
            _instanceId, new GreetingsOrchestration());

        await _client.WaitForOrchestrationAsync(instance, TimeSpan.FromSeconds(60), stoppingToken);

        _console.WriteLine("Orchestration finished.");
        _console.WriteLine("Press Ctrl+C to exit");
    }
}
