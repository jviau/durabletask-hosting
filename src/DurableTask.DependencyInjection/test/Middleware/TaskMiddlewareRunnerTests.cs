﻿// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DurableTask.DependencyInjection.Middleware.Tests;

public class TaskMiddlewareRunnerTests
{
    [Fact]
    public async Task Run_NullDescriptor_Throws()
    {
        Func<Task> act = () => TaskMiddlewareRunner.RunAsync(null, CreateContext(), () => Task.CompletedTask);
        await act.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Run_NullContext_Throws()
    {
        TaskMiddlewareDescriptor descriptor = new((c, n) => Task.CompletedTask);
        Func<Task> act = () => TaskMiddlewareRunner.RunAsync(descriptor, null, () => Task.CompletedTask);
        await act.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Run_NullNext_Throws()
    {
        TaskMiddlewareDescriptor descriptor = new((c, n) => Task.CompletedTask);
        Func<Task> act = () => TaskMiddlewareRunner.RunAsync(descriptor, CreateContext(), null);
        await act.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Run_Type_Executes(bool addMiddleware)
    {
        // arrange
        NextFunc next = new();
        var descriptor = TaskMiddlewareDescriptor.Create<TestMiddleware>();

        IServiceProvider serviceProvider = CreateServiceProvider(addMiddleware);
        var context = CreateContext(serviceProvider);

        // act
        await TaskMiddlewareRunner.RunAsync(descriptor, context, next.InvokeAsync);

        // assert
        var tracker = serviceProvider.GetRequiredService<ExecutionTracker>();
        tracker.ExecutionCount.Should().Be(1);
        next.ExecutionCount.Should().Be(1);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Run_Type_ExecutesTwice(bool addMiddleware)
    {
        // arrange
        NextFunc next1 = new();
        NextFunc next2 = new();
        var descriptor = TaskMiddlewareDescriptor.Create<TestMiddleware>();

        IServiceProvider serviceProvider = CreateServiceProvider(addMiddleware);
        var context = CreateContext(serviceProvider);

        // act
        await TaskMiddlewareRunner.RunAsync(descriptor, context, next1.InvokeAsync);
        await TaskMiddlewareRunner.RunAsync(descriptor, context, next2.InvokeAsync);

        // assert
        var tracker = serviceProvider.GetRequiredService<ExecutionTracker>();
        tracker.ExecutionCount.Should().Be(2);
        next1.ExecutionCount.Should().Be(1);
        next2.ExecutionCount.Should().Be(1);
    }

    [Fact]
    public async Task Run_Func_Executes()
    {
        // arrange
        NextFunc next = new();
        int executed = 0;
        Task Middleware(DispatchMiddlewareContext context, Func<Task> next)
        {
            executed++;
            return next();
        }

        TaskMiddlewareDescriptor descriptor = new(Middleware);
        var context = CreateContext();

        // act
        await TaskMiddlewareRunner.RunAsync(descriptor, context, next.InvokeAsync);

        // assert
        executed.Should().Be(1);
        next.ExecutionCount.Should().Be(1);
    }

    [Fact]
    public async Task Run_Func_ExecutesTwice()
    {
        // arrange
        NextFunc next1 = new();
        NextFunc next2 = new();
        int executed = 0;
        Task Middleware(DispatchMiddlewareContext context, Func<Task> next)
        {
            executed++;
            return next();
        }

        TaskMiddlewareDescriptor descriptor = new(Middleware);
        var context = CreateContext();

        // act
        await TaskMiddlewareRunner.RunAsync(descriptor, context, next1.InvokeAsync);
        await TaskMiddlewareRunner.RunAsync(descriptor, context, next2.InvokeAsync);

        // assert
        executed.Should().Be(2);
        next1.ExecutionCount.Should().Be(1);
        next2.ExecutionCount.Should().Be(1);
    }

    private static DispatchMiddlewareContext CreateContext(IServiceProvider serviceProvider = null)
    {
        serviceProvider ??= Mock.Of<IServiceProvider>();
        var context = (DispatchMiddlewareContext)Activator.CreateInstance(
            typeof(DispatchMiddlewareContext), nonPublic: true);
        context.SetProperty(serviceProvider);
        return context;
    }

    private static IServiceProvider CreateServiceProvider(bool addMiddleware = false)
    {
        ServiceCollection services = new();
        services.AddSingleton<ExecutionTracker>();
        if (addMiddleware)
        {
            services.AddTransient<TestMiddleware>();
        }

        return services.BuildServiceProvider();
    }

    public class ExecutionTracker
    {
        public int ExecutionCount { get; set; }
    }

    public class TestMiddleware : ITaskMiddleware
    {
        private readonly ExecutionTracker _tracker;

        public TestMiddleware(ExecutionTracker tracker)
        {
            _tracker = tracker;
        }

        public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
        {
            _tracker.ExecutionCount++;
            return next();
        }
    }

    public class NextFunc
    {
        public int ExecutionCount { get; private set; }

        public Task InvokeAsync()
        {
            ExecutionCount++;
            return Task.CompletedTask;
        }
    }
}
