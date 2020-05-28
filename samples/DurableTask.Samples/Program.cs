// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using DurableTask.Hosting;
using DurableTask.Samples.Greetings;
using Dynamitey.DynamicObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

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
            IHost host = CreateDefaultBuilder(args)
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

                    builder.AddOrchestration<GreetingsOrchestration>();
                    builder
                        .AddActivity<GetUserTask>()
                        .AddActivity<SendGreetingTask>();
                })
                .UseConsoleLifetime()
                .Build();

            using (DurableTaskEventListener listener
                = ActivatorUtilities.CreateInstance<DurableTaskEventListener>(host.Services))
            {
                return host.RunAsync();
            }
        }

        private static HostBuilder CreateDefaultBuilder(string[] args)
        {
            // Host.CreateDefaultBuilder() is not available before Microsoft.Extensions.Hosting 3.0.
            // So this is a copied from https://github.com/dotnet/extensions
            var builder = new HostBuilder();

            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "DOTNET_");
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                IHostingEnvironment env = hostingContext.HostingEnvironment;
                env.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            builder.ConfigureLogging((hostingContext, logging) =>
            {
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
                // the defaults be overridden by the configuration.
                if (isWindows)
                {
                    // Default the EventLogLoggerProvider to warning or above
                    logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                }

                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();

                if (isWindows)
                {
                    // Add the EventLogLoggerProvider on windows machines
                    logging.AddEventLog();
                }
            });

            var options = new ServiceProviderOptions();
            builder.UseServiceProviderFactory(new DefaultServiceProviderFactory(options));

            return builder;
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
                        NameVersionHelper.GetDefaultName(typeof(GreetingsOrchestration)),
                        NameVersionHelper.GetDefaultVersion(typeof(GreetingsOrchestration)),
                        _instanceId,
                        null,
                        new Dictionary<string, string>()
                        {
                            ["CorrelationId"] = Guid.NewGuid().ToString(),
                        });

                OrchestrationState result = await _client.WaitForOrchestrationAsync(
                    instance, TimeSpan.FromSeconds(60));

                _console.WriteLine();
                _console.WriteLine($"Orchestration finished.");
                _console.WriteLine($"Run stats: {result.Status}");
                _console.WriteLine("Press Ctrl+C to exit");
            }
        }
    }
}
