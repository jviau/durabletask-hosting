using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Activities;
using DurableTask.DependencyInjection.Middleware;
using FluentAssertions;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Middleware
{
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
            var middleware = new ServiceProviderActivityMiddleware(Mock.Of<IServiceProvider>());

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
            var middleware = new ServiceProviderActivityMiddleware(Mock.Of<IServiceProvider>());

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
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(m => m.GetService(typeof(TestActivity))).Returns(new TestActivity());
            var wrapper = new WrapperActivity(typeof(TestActivity));
            DispatchMiddlewareContext context = CreateContext(wrapper);
            var middleware = new ServiceProviderActivityMiddleware(serviceProvider.Object);

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
            var activity = new TestActivity();
            var serviceProvider = new Mock<IServiceProvider>();
            DispatchMiddlewareContext context = CreateContext(activity);
            var middleware = new ServiceProviderActivityMiddleware(serviceProvider.Object);

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
}
