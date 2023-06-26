// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.History;
using DurableTask.Core.Middleware;
using DurableTask.Core.Serializing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DurableTask.Extensions.Middleware.Tests;

public class SetOrchestrationDataMiddlewareTests
{
    private readonly Mock<ILoggerFactory> _loggerFactory = new();
    private readonly ILogger _logger = Mock.Of<ILogger>();
    private readonly DataConverter _converter = Mock.Of<JsonDataConverter>();
    private readonly IOptions<DurableExtensionsOptions> _options;

    public SetOrchestrationDataMiddlewareTests()
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
        Action act = () => new SetOrchestrationDataMiddleware(null, _options);
        act.Should().Throw<ArgumentNullException>().WithParameterName("loggerFactory");
    }

    [Fact]
    public void Ctor_ArgNull_DataConverter()
    {
        Action act = () => new SetOrchestrationDataMiddleware(_loggerFactory.Object, null);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }
    
    [Fact]
    public async Task Invoke_NoBase_Skips()
    {
        // arrange
        Mock<TaskOrchestration> orchestration = CreateOrchestration(false);
        DispatchMiddlewareContext context = CreateContext(orchestration.Object);
        SetOrchestrationDataMiddleware middleware = new(_loggerFactory.Object, _options);
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
    public async Task Invoke_OrchestrationBase_Initializes(string version)
    {
        // arrange
        const string name = "TestOrchestration";
        ExecutionStartedEvent started = CreateExecutionStarted(name, version);
        Mock<TaskOrchestration> orchestration = CreateOrchestration(true);
        DispatchMiddlewareContext context = CreateContext(orchestration.Object, started);
        SetOrchestrationDataMiddleware middleware = new(_loggerFactory.Object, _options);
        NextClosure next = new();

        // act
        await middleware.InvokeAsync(context, next.InvokeAsync);

        // assert
        next.CallCount.Should().Be(1);
        orchestration.As<IOrchestrationBase>()
            .Verify(m => m.Initialize(name, version, _logger, _converter), Times.Once);
    }

    private static DispatchMiddlewareContext CreateContext(
        TaskOrchestration orchestration, ExecutionStartedEvent executionStarted = null)
    {
        var context = new DispatchMiddlewareContext();
        executionStarted ??= CreateExecutionStarted("TestOrchestration");

        context.SetProperty(orchestration);
        context.SetProperty(new OrchestrationRuntimeState(new[] { executionStarted }));

        return context;
    }

    private static ExecutionStartedEvent CreateExecutionStarted(string name, string version = null)
    {
        return new ExecutionStartedEvent(-1, null)
        {
            Name = name,
            Version = version,
        };
    }

    private static Mock<TaskOrchestration> CreateOrchestration(bool @interface)
    {
        Mock<TaskOrchestration> orchestration = new(MockBehavior.Strict);
        if (@interface)
        {
            orchestration.As<IOrchestrationBase>()
                .Setup(m => m.Initialize(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ILogger>(),
                    It.IsAny<DataConverter>()));
        }

        return orchestration;
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
