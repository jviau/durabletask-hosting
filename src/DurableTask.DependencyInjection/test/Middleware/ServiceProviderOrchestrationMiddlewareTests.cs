using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Middleware;
using DurableTask.DependencyInjection.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Middleware
{
    public class ServiceProviderOrchestrationMiddlewareTests
    {
        [Fact]
        public void Ctor_ArgumentNull()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => new ServiceProviderOrchestrationMiddleware(null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public async Task InvokeAsync_ArgumentNullContext()
        {
            // arrange
            var middleware = new ServiceProviderOrchestrationMiddleware(Mock.Of<IServiceProvider>());

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
            var middleware = new ServiceProviderOrchestrationMiddleware(Mock.Of<IServiceProvider>());

            // act
            ArgumentNullException ex = await Capture<ArgumentNullException>(
                () => middleware.InvokeAsync(CreateContext(null), null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public async Task InvokeAsync_Wrapped_SetsOrchestration()
        {
            // arrange
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(m => m.GetService(typeof(TestOrchestration))).Returns(new TestOrchestration());
            var wrapper = new WrapperOrchestration(new TaskOrchestrationDescriptor(typeof(TestOrchestration)));
            DispatchMiddlewareContext context = CreateContext(wrapper);
            var middleware = new ServiceProviderOrchestrationMiddleware(serviceProvider.Object);

            // act
            await middleware.InvokeAsync(context, () => Task.CompletedTask);

            // assert
            TaskOrchestration activity = context.GetProperty<TaskOrchestration>();
            activity.Should().NotBeNull();
            activity.Should().Be(wrapper.InnerOrchestration);
            serviceProvider.Verify(m => m.GetService(typeof(TestOrchestration)), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_NotWrapped_Continues()
        {
            // arrange
            var activity = new TestOrchestration();
            var serviceProvider = new Mock<IServiceProvider>();
            DispatchMiddlewareContext context = CreateContext(activity);
            var middleware = new ServiceProviderOrchestrationMiddleware(serviceProvider.Object);

            // act
            await middleware.InvokeAsync(context, () => Task.CompletedTask);

            // assert
            TaskOrchestration actual = context.GetProperty<TaskOrchestration>();
            actual.Should().NotBeNull();
            actual.Should().Be(activity);
            serviceProvider.Verify(m => m.GetService(It.IsAny<Type>()), Times.Never);
        }

        private static DispatchMiddlewareContext CreateContext(TaskOrchestration activity)
        {
            var context = (DispatchMiddlewareContext)Activator.CreateInstance(typeof(DispatchMiddlewareContext), true);
            context.SetProperty(activity);
            return context;
        }

        private class TestOrchestration : TaskOrchestration
        {
            public override Task<string> Execute(OrchestrationContext context, string input)
            {
                throw new NotImplementedException();
            }

            public override string GetStatus()
            {
                throw new NotImplementedException();
            }

            public override void RaiseEvent(OrchestrationContext context, string name, string input)
            {
                throw new NotImplementedException();
            }
        }
    }
}
