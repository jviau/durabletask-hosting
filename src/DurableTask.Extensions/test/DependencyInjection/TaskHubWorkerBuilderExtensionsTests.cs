// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection.Internal;
using DurableTask.Extensions;
using DurableTask.Extensions.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DurableTask.DependencyInjection.Tests;

public class TaskHubWorkerBuilderExtensionsTests
{
    private readonly DataConverter _converter = Mock.Of<JsonDataConverter>();

    [Fact]
    public void AddDurableExtensions_AddsMiddleware()
    {
        // arrange
        ServiceCollection services = new();
        DefaultTaskHubWorkerBuilder builder = new(services);

        // act
        builder.AddDurableExtensions();

        // assert
        builder.ActivityMiddleware.Last().Type.Should().Be<SetActivityDataMiddleware>();
        builder.OrchestrationMiddleware.Last().Type.Should().Be<SetOrchestrationDataMiddleware>();
    }

    [Fact]
    public void AddDurableExtensions_Configure_DataConverter()
    {
        // arrange
        ServiceCollection services = new();
        services.AddLogging();
        services.AddOptions();
        services.AddSingleton(Mock.Of<IOrchestrationService>());
        services.AddSingleton(Mock.Of<IOrchestrationServiceClient>());

        DefaultTaskHubWorkerBuilder builder = new(services);

        // act
        builder.AddDurableExtensions(opt => opt.DataConverter = _converter);
        builder.AddClient();
        IServiceProvider provider = services.BuildServiceProvider();

        // assert
        TaskHubClientOptions clientOptions = provider
            .GetRequiredService<IOptions<TaskHubClientOptions>>().Value;
        clientOptions.DataConverter.Should().Be(_converter);
        DurableExtensionsOptions extensionsOptions = provider
            .GetRequiredService<IOptions<DurableExtensionsOptions>>().Value;
        extensionsOptions.DataConverter.Should().Be(_converter);
    }

    [Fact]
    public void AddDurableExtensions_Service_DataConverter()
    {
        // arrange
        ServiceCollection services = new();
        services.AddLogging();
        services.AddOptions();
        services.AddSingleton(_converter);
        services.AddSingleton(Mock.Of<IOrchestrationService>());
        services.AddSingleton(Mock.Of<IOrchestrationServiceClient>());

        DefaultTaskHubWorkerBuilder builder = new(services);

        // act
        builder.AddDurableExtensions();
        builder.AddClient();
        IServiceProvider provider = services.BuildServiceProvider();

        // assert
        TaskHubClientOptions clientOptions = provider
            .GetRequiredService<IOptions<TaskHubClientOptions>>().Value;
        clientOptions.DataConverter.Should().Be(_converter);
        DurableExtensionsOptions extensionsOptions = provider
            .GetRequiredService<IOptions<DurableExtensionsOptions>>().Value;
        extensionsOptions.DataConverter.Should().Be(_converter);
    }
}
