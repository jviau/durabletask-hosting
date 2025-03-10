// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection.Internal;
using DurableTask.Extensions;
using DurableTask.Extensions.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        services.AddOptions();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        DefaultTaskHubWorkerBuilder builder = new(services);

        // act
        builder.AddDurableExtensions();
        IServiceProvider provider = services.BuildServiceProvider();

        // assert
        TaskMiddlewareDescriptor descriptor = builder.ActivityMiddleware.Last();
        descriptor.Factory.Should().NotBeNull();
        ITaskMiddleware middleware = descriptor.Factory.Invoke(provider);
        middleware.Should().BeOfType<SetActivityDataMiddleware>();

        descriptor = builder.OrchestrationMiddleware.Last();
        descriptor.Factory.Should().NotBeNull();
        middleware = descriptor.Factory.Invoke(provider);
        middleware.Should().BeOfType<SetOrchestrationDataMiddleware>();
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
        InternalTaskHubOptions clientOptions = provider
            .GetRequiredService<IOptions<InternalTaskHubOptions>>().Value;
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
        InternalTaskHubOptions clientOptions = provider
            .GetRequiredService<IOptions<InternalTaskHubOptions>>().Value;
        clientOptions.DataConverter.Should().Be(_converter);
        DurableExtensionsOptions extensionsOptions = provider
            .GetRequiredService<IOptions<DurableExtensionsOptions>>().Value;
        extensionsOptions.DataConverter.Should().Be(_converter);
    }
}
