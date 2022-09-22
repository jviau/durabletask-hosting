﻿// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.DependencyInjection;
using DurableTask.Emulator;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace DurableTask.Hosting.Functional.Tests;

public class EndToEndTests
{
    private readonly string _instanceId = Guid.NewGuid().ToString();

    [Fact]
    public void ConfigureTaskHubWorker_ServicesAdded()
    {
        IHost host = CreateHost(
            _ => { },
            b => b.WithOrchestrationService(new LocalOrchestrationService()));

        TaskHubWorker worker = host.Services.GetService<TaskHubWorker>();
        IEnumerable<IHostedService> hostedServices = host.Services.GetServices<IHostedService>();

        worker.Should().NotBeNull();
        hostedServices.Should().HaveCount(1);
        hostedServices.Single().Should().BeOfType(typeof(TaskHubBackgroundService));
    }

    [Fact]
    public void ConfigureTaskHubWorker_ServicesAdded2()
    {
        IHost host = CreateHost(
            s => s.AddSingleton<IOrchestrationService>(new LocalOrchestrationService()),
            _ => { });

        TaskHubWorker worker = host.Services.GetService<TaskHubWorker>();
        IEnumerable<IHostedService> hostedServices = host.Services.GetServices<IHostedService>();

        worker.Should().NotBeNull();
        hostedServices.Should().HaveCount(1);
        hostedServices.Single().Should().BeOfType(typeof(TaskHubBackgroundService));
    }

    [Fact]
    public void ConfigureTaskHubWorker_ServicesAdded3()
    {
        IHost host = CreateHost(
            _ => { },
#pragma warning disable CS0618 // Type or member is obsolete
            b => b.OrchestrationService = new LocalOrchestrationService());
#pragma warning restore CS0618 // Type or member is obsolete

        TaskHubWorker worker = host.Services.GetService<TaskHubWorker>();
        IEnumerable<IHostedService> hostedServices = host.Services.GetServices<IHostedService>();

        worker.Should().NotBeNull();
        hostedServices.Should().HaveCount(1);
        hostedServices.Single().Should().BeOfType(typeof(TaskHubBackgroundService));
    }

    [Theory]
    [InlineData((ServiceLifetime)(-1))]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public async Task RunOrchestration_Activated(ServiceLifetime serviceLifetime)
    {
        IHost host = CreateHost(
            s =>
            {
                s.AddSingleton<IOrchestrationService>(new LocalOrchestrationService());
                s.AddSingleton<ExecutionTracker>();

                if ((int)serviceLifetime != -1)
                {
                    s.Add(new ServiceDescriptor(
                        typeof(TestOrchestration), typeof(TestOrchestration), serviceLifetime));
                }
            },
            b =>
            {
                b.AddOrchestration<TestOrchestration>();
                b.AddClient();
            });

        await host.StartAsync();
        TaskHubClient client = host.Services.GetRequiredService<TaskHubClient>();
        ExecutionTracker executionTracker = host.Services.GetRequiredService<ExecutionTracker>();

        string input = "input";
        OrchestrationInstance instance = await client
            .CreateOrchestrationInstanceAsync(typeof(TestOrchestration), _instanceId, input);
        OrchestrationState result = await client
            .WaitForOrchestrationAsync(instance, TimeSpan.FromSeconds(60));

        TestPayload payload = JsonConvert.DeserializeObject<TestPayload>(result.Output);

        payload.Should().NotBeNull();
        payload.Input.Should().Be(input);
        executionTracker.Executions.Should().HaveCount(1);
        executionTracker.Executions.Single().ExecutedType.Should().Be(typeof(TestOrchestration));
        await TestHelpers.Capture<OperationCanceledException>(() => host.StopAsync());
    }

    private static IHost CreateHost(
        Action<IServiceCollection> configureServices,
        Action<ITaskHubWorkerBuilder> configureTaskHubWorker)
    {
        IHostBuilder builder = CreateHostBuilder();
        builder.ConfigureServices(configureServices);
        builder.ConfigureTaskHubWorker(configureTaskHubWorker);

        return builder.Build();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        var builder = new HostBuilder();
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        builder.ConfigureLogging(_ => { });
        return builder;
    }

    private class TestPayload
    {
        public string Input { get; set; }
    }

    private class TestOrchestration : TaskOrchestration<TestPayload, string>
    {
        private readonly ExecutionTracker _tracker;

        public TestOrchestration(ExecutionTracker tracker)
        {
            _tracker = tracker;
        }

        public override Task<TestPayload> RunTask(OrchestrationContext context, string input)
        {
            _tracker.Add(typeof(TestOrchestration), context, input);
            return Task.FromResult(new TestPayload
            {
                Input = input,
            });
        }
    }

    private class ExecutionTracker
    {
        public IList<ExecutionInstance> Executions { get; }
            = new List<ExecutionInstance>();

        public void Add(Type type, OrchestrationContext context, string input)
        {
            Executions.Add(new ExecutionInstance
            {
                ExecutedType = type,
                Context = context,
                Input = input,
            });
        }
    }

    private class ExecutionInstance
    {
        public Type ExecutedType { get; set;  }

        public OrchestrationContext Context { get; set; }

        public string Input { get; set; }
    }
}
