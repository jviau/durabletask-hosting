// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using DurableTask.Hosting;
using DurableTask.Samples.Generics;
using DurableTask.Samples.Greetings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DurableTask.Samples
{
    /// <summary>
    /// The samples program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        /// <param name="args">The supplied arguments, if any.</param>
        /// <returns>A task that completes when this program is finished running.</returns>
        public static Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConsole, ConsoleWrapper>();
                    services.AddHostedService<TaskEnqueuer>();
                })
                .ConfigureTaskHubWorker((context, builder) =>
                {
                    // IOrchestrationService orchestrationService = UseServiceBus(context.Configuration);
                    IOrchestrationService orchestrationService = UseLocalEmulator();
                    builder.WithOrchestrationService(orchestrationService);

                    builder.AddClient();

                    builder.UseOrchestrationMiddleware<OrchestrationInstanceExMiddleware>();
                    builder.UseOrchestrationMiddleware<SampleMiddleware>();

                    builder.UseActivityMiddleware<ActivityInstanceExMiddleware>();
                    builder.UseActivityMiddleware<SampleMiddleware>();

                    builder
                        .AddOrchestration<GreetingsOrchestration>()
                        .AddOrchestration<GenericOrchestrationRunner>();

                    //builder
                    //    .AddActivity<PrintTask>()
                    //    .AddActivity<GetUserTask>()
                    //    .AddActivity<SendGreetingTask>()
                    //    .AddActivity(typeof(GenericActivity<>));

                    builder.AddActivitiesFromAssembly<Program>();
                })
                .UseConsoleLifetime()
                .Build();

            return host.RunAsync();
        }

        //private static IOrchestrationService UseServiceBus(IConfiguration config)
        //{
        //    string taskHubName = config.GetValue<string>("DurableTask:TaskHubName");
        //    string azureStorageConnectionString = config.GetValue<string>("DurableTask:AzureStorage:ConnectionString");
        //    string serviceBusConnectionString = config.GetValue<string>("DurableTask:ServiceBus:ConnectionString");

        //    IOrchestrationServiceInstanceStore instanceStore =
        //        new AzureTableInstanceStore(taskHubName, azureStorageConnectionString);

        //    var orchestrationService =
        //        new ServiceBusOrchestrationService(
        //            serviceBusConnectionString,
        //            taskHubName,
        //            instanceStore,
        //            null,
        //            null);

        //    // TODO: do by default via config
        //    orchestrationService.CreateIfNotExistsAsync().GetAwaiter().GetResult();

        //    return orchestrationService;
        //}

        private static IOrchestrationService UseLocalEmulator()
            => new LocalOrchestrationService();

        private class TaskEnqueuer : BackgroundService
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
    }
}
