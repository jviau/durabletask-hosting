// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Extensions.Tests;

public class TaskHubWorkerBuilderExtensionsTests
{
    [Fact]
    public void WithOrchestrationService_ArgumentNull()
        => RunTestException<ArgumentNullException>(
            _ => TaskHubWorkerBuilderExtensions.WithOrchestrationService(null, Mock.Of<IOrchestrationService>()));

    [Fact]
    public void WithOrchestration_ServiceIsSet()
        => RunTest(
            builder =>
            {
                IOrchestrationService service = Mock.Of<IOrchestrationService>();
                ITaskHubWorkerBuilder returned = builder.WithOrchestrationService(service);

                builder.Should().NotBeNull();
                builder.Should().BeSameAs(returned);
                return service;
            },
            (mock, service) =>
            {
                mock.Object.Services.Should().Contain(x => x.ServiceType == typeof(IOrchestrationService));
            });

    [Fact]
    public void AddClient_ClientAdded()
        => RunTest(
            builder => builder.AddClient(),
            (mock, builder) =>
            {
                builder.Should().Be(mock.Object);
                builder.Services.Should().HaveCount(2);
                var client = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(TaskHubClient));
                client.Should().NotBeNull();
                client.Lifetime.Should().Be(ServiceLifetime.Singleton);
            });

    [Fact]
    public void ClientFactory_NoOrchestration()
        => RunTestException<InvalidOperationException>(
            builder =>
            {
                builder.AddClient();
                IServiceProvider provider = builder.Services.BuildServiceProvider();
                provider.GetService<TaskHubClient>();
            });

    [Fact]
    public void ClientFactory_NotOrchestrationClient()
        => RunTestException<InvalidOperationException>(
            builder =>
            {
                Mock<IOrchestrationService> mockOrchestrationService = new();
                builder.WithOrchestrationService(mockOrchestrationService.Object);
                builder.AddClient();
                IServiceProvider provider = builder.Services.BuildServiceProvider();
                provider.GetService<TaskHubClient>();
            });

    [Fact]
    public void ClientFactory_ClientReturned_Casted()
        => RunTest(
            builder =>
            {
                Mock<IOrchestrationService> mockOrchestrationService = new();
                mockOrchestrationService.As<IOrchestrationServiceClient>();
                builder.WithOrchestrationService(mockOrchestrationService.Object);
                builder.AddClient();
                IServiceProvider provider = builder.Services.BuildServiceProvider();
                return provider.GetService<TaskHubClient>();
            },
            (mock, client) =>
            {
                client.Should().NotBeNull();
            });

    [Fact]
    public void ClientFactory_ClientReturned_Casted2()
        => RunTest(
            builder =>
            {
                Mock<IOrchestrationService> mockOrchestrationService = new();
                mockOrchestrationService.As<IOrchestrationServiceClient>();

#pragma warning disable CS0618 // Type or member is obsolete
                Mock.Get(builder)
                    .Setup(m => m.OrchestrationService)
                    .Returns(mockOrchestrationService.Object);
#pragma warning restore CS0618 // Type or member is obsolete

                builder.AddClient();
                IServiceProvider provider = builder.Services.BuildServiceProvider();
                return provider.GetService<TaskHubClient>();
            },
            (mock, client) =>
            {
                client.Should().NotBeNull();
            });

    [Fact]
    public void ClientFactory_ClientReturned_Direct()
        => RunTest(
            builder =>
            {
                var mockClient = Mock.Of<IOrchestrationServiceClient>();
                builder.Services.AddSingleton(mockClient);
                builder.AddClient();
                IServiceProvider provider = builder.Services.BuildServiceProvider();
                return provider.GetService<TaskHubClient>();
            },
            (mock, client) =>
            {
                client.Should().NotBeNull();
            });

    private static void RunTestException<TException>(Action<ITaskHubWorkerBuilder> act)
        where TException : Exception
    {
        bool Act(ITaskHubWorkerBuilder builder)
        {
            act(builder);
            return true;
        }

        TException exception = Capture<TException>(() => RunTest(Act, null));
        exception.Should().NotBeNull();
    }

    private static void RunTest<TResult>(
        Func<ITaskHubWorkerBuilder, TResult> act,
        Action<Mock<ITaskHubWorkerBuilder>, TResult> verify)
    {
        Mock<ITaskHubWorkerBuilder> mock = new();

        ServiceCollection services = new();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        mock.Setup(x => x.Services).Returns(services);

        TResult result = act(mock.Object);
        verify?.Invoke(mock, result);
    }
}
