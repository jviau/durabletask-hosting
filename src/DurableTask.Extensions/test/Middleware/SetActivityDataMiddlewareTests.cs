// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Configuration;
using System.Reflection;
using DurableTask.Core;
using DurableTask.Core.History;
using DurableTask.Core.Middleware;
using DurableTask.Core.Serializing;
using DurableTask.Extensions.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DurableTask.Extensions.Middleware.Tests;

public class SetActivityDataMiddlewareTests
{
    private readonly Mock<ILoggerFactory> _loggerFactory = new();
    private readonly ILogger _logger = Mock.Of<ILogger>();
    private readonly DataConverter _converter = Mock.Of<JsonDataConverter>();
    private readonly IOptions<DurableExtensionsOptions> _options;

    public SetActivityDataMiddlewareTests()
    {
        _loggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(_logger);
        _options =  Options.Create(new DurableExtensionsOptions
        {
            DataConverter = _converter,
        });
    }

    [Fact]
    public void Ctor_ArgNull_LoggerFactory()
    {
        Action act = () => new SetActivityDataMiddleware(null, _options);
        act.Should().Throw<ArgumentNullException>().WithParameterName("loggerFactory");
    }

    [Fact]
    public void Ctor_ArgNull_DataConverter()
    {
        Action act = () => new SetActivityDataMiddleware(_loggerFactory.Object, null);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }
    
    [Fact]
    public async Task Invoke_NoBase_Skips()
    {
        // arrange
        Mock<TaskActivity> activity = CreateActivity(false);
        DispatchMiddlewareContext context = CreateContext(activity.Object);
        SetActivityDataMiddleware middleware = new(_loggerFactory.Object, _options);
        NextClosure next = new();

        // act
        await middleware.InvokeAsync(context, next.InvokeAsync);

        // assert
        next.CallCount.Should().Be(1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("v1")]
    public async Task Invoke_ActivityBase_Initializes(string version)
    {
        // arrange
        const string name = "TestActivity";
        TaskScheduledEvent started = CreateTaskScheduled(name, version);
        Mock<TaskActivity> activity = CreateActivity(true);
        DispatchMiddlewareContext context = CreateContext(activity.Object, started);
        SetActivityDataMiddleware middleware = new(_loggerFactory.Object, _options);
        NextClosure next = new();

        // act
        await middleware.InvokeAsync(context, next.InvokeAsync);

        // assert
        next.CallCount.Should().Be(1);
        activity.As<IActivityBase>()
            .Verify(m => m.Initialize(name, version, _logger, _converter), Times.Once);
    }

    [Fact]
    public async Task Invoke_ReflectionActivity_Initializes()
    {
        // arrange
        MethodInfo method = typeof(IMyService).GetMethod(nameof(IMyService.MyMethodAsync));
        ReflectionBasedTaskActivity activity = new(Mock.Of<IMyService>(), method);
        DispatchMiddlewareContext context = CreateContext(activity);
        SetActivityDataMiddleware middleware = new(_loggerFactory.Object, _options);
        NextClosure next = new();

        // act
        await middleware.InvokeAsync(context, next.InvokeAsync);

        // assert
        next.CallCount.Should().Be(1);
        activity.DataConverter.Should().Be(_converter);
    }

    private static DispatchMiddlewareContext CreateContext(
        TaskActivity activity, TaskScheduledEvent scheduledEvent = null)
    {
        var context = new DispatchMiddlewareContext();
        scheduledEvent ??= CreateTaskScheduled("TestActivity");

        context.SetProperty(activity);
        context.SetProperty(scheduledEvent);
        context.SetProperty(Mock.Of<OrchestrationInstance>());

        return context;
    }

    private static TaskScheduledEvent CreateTaskScheduled(string name, string version = null)
    {
        return new TaskScheduledEvent(-1)
        {
            Name = name,
            Version = version,
        };
    }

    private static Mock<TaskActivity> CreateActivity(bool @interface)
    {
        Mock<TaskActivity> activity = new(MockBehavior.Strict);
        if (@interface)
        {
            activity.As<IActivityBase>()
                .Setup(m => m.Initialize(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ILogger>(),
                    It.IsAny<DataConverter>()));
        }

        return activity;
    }

    public interface IMyService
    {
        Task<string> MyMethodAsync(string input);
    }

    private class NextClosure
    {
        public int CallCount { get; private set; }

        public Task InvokeAsync()
        {
            CallCount++;
            return Task.CompletedTask;
        }
    }
}
