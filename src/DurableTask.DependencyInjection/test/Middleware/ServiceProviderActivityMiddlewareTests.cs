// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;
using FluentAssertions;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Middleware.Tests;

public class ServiceProviderActivityMiddlewareTests
{
    [Fact]
    public void Ctor_ArgumentNull()
    {
        // arrange, act
        ArgumentNullException ex = Capture<ArgumentNullException>(
            () => new ServiceProviderActivityMiddleware(null));

        // assert
        ex.Should().NotBeNull();
    }

    [Fact]
    public async Task InvokeAsync_ArgumentNullContext()
    {
        // arrange
        ServiceProviderActivityMiddleware middleware = new(Mock.Of<IServiceProvider>());

        // act
        ArgumentNullException ex = await Capture<ArgumentNullException>(
            () => middleware.InvokeAsync(null, () => Task.CompletedTask));

        // assert
        ex.Should().NotBeNull();
    }

    [Fact]
    public async Task InvokeAsync_ArgumentNullNext()
    {
        // arrange
        ServiceProviderActivityMiddleware middleware = new(Mock.Of<IServiceProvider>());

        // act
        ArgumentNullException ex = await Capture<ArgumentNullException>(
            () => middleware.InvokeAsync(CreateContext(null), null));

        // assert
        ex.Should().NotBeNull();
    }

    [Fact]
    public async Task InvokeAsync_Wrapped_SetsActivity()
    {
        // arrange
        Mock<IServiceProvider> serviceProvider = new();
        serviceProvider.Setup(m => m.GetService(typeof(TestActivity))).Returns(new TestActivity());
        WrapperActivity wrapper = new(new TaskActivityDescriptor(typeof(TestActivity)));
        DispatchMiddlewareContext context = CreateContext(wrapper);
        ServiceProviderActivityMiddleware middleware = new(serviceProvider.Object);

        // act
        await middleware.InvokeAsync(context, () => Task.CompletedTask);

        // assert
        TaskActivity activity = context.GetProperty<TaskActivity>();
        activity.Should().NotBeNull();
        activity.Should().Be(wrapper.InnerActivity);
        serviceProvider.Verify(m => m.GetService(typeof(TestActivity)), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_NotWrapped_Continues()
    {
        // arrange
        TestActivity activity = new();
        Mock<IServiceProvider> serviceProvider = new();
        DispatchMiddlewareContext context = CreateContext(activity);
        ServiceProviderActivityMiddleware middleware = new(serviceProvider.Object);

        // act
        await middleware.InvokeAsync(context, () => Task.CompletedTask);

        // assert
        TaskActivity actual = context.GetProperty<TaskActivity>();
        actual.Should().NotBeNull();
        actual.Should().Be(activity);
        serviceProvider.Verify(m => m.GetService(It.IsAny<Type>()), Times.Never);
    }

    private static DispatchMiddlewareContext CreateContext(TaskActivity activity)
    {
        var context = (DispatchMiddlewareContext)Activator.CreateInstance(typeof(DispatchMiddlewareContext), true);
        context.SetProperty(activity);
        return context;
    }

    private class TestActivity : TaskActivity
    {
        public override string Run(TaskContext context, string input)
        {
            throw new NotImplementedException();
        }
    }
}
