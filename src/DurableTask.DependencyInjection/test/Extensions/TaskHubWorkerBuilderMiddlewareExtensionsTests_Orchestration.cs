using System;
using System.Threading.Tasks;
using DurableTask.Core.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.DependencyInjection.Tests.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubWorkerBuilderMiddlewareExtensionsTests_Orchestration
    {
        [Fact]
        public void UseOrchestrationMiddlewareInstanceNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseOrchestrationMiddleware(null, new TestMiddleware()));

        [Fact]
        public void UseOrchestrationMiddlewareTypeNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseOrchestrationMiddleware(null, typeof(TestMiddleware)));

        [Fact]
        public void UseOrchestrationMiddlewareTypeNullInstance()
            => RunTestException<ArgumentNullException>(
                builder => builder.UseOrchestrationMiddleware((ITaskMiddleware)null));

        [Fact]
        public void UseOrchestrationMiddlewareTypeNullType()
            => RunTestException<ArgumentNullException>(
                builder => builder.UseOrchestrationMiddleware((Type)null));

        [Fact]
        public void UseOrchestrationMiddlewareGenericNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseOrchestrationMiddleware<TestMiddleware>(null));

        [Fact]
        public void UseOrchestrationMiddlewareFactoryNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseOrchestrationMiddleware(null, __ => new TestMiddleware()));

        [Fact]
        public void UseOrchestrationMiddlewareInstance()
        {
            var instance = new TestMiddleware();
            RunTest(
                builder => builder.UseOrchestrationMiddleware(instance),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseOrchestrationMiddleware(IsDescriptor(instance)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });
        }

        [Fact]
        public void UseOrchestrationMiddlewareType()
            => RunTest(
                builder => builder.UseOrchestrationMiddleware(typeof(TestMiddleware)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseOrchestrationMiddleware(IsDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void UseOrchestrationMiddlewareGeneric()
            => RunTest(
                builder => builder.UseOrchestrationMiddleware<TestMiddleware>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseOrchestrationMiddleware(IsDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void UseOrchestrationMiddlewareFactory()
            => RunTest(
                builder => builder.UseOrchestrationMiddleware(_ => new TestMiddleware()),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseOrchestrationMiddleware(IsDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
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
            var mock = new Mock<ITaskHubWorkerBuilder>();
            mock.Setup(m => m.UseOrchestrationMiddleware(It.IsAny<TaskMiddlewareDescriptor>())).Returns(mock.Object);

            TResult result = act(mock.Object);
            verify?.Invoke(mock, result);
        }

        private class TestMiddleware : ITaskMiddleware
        {
            public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
            {
                throw new NotImplementedException();
            }
        }

        private TaskMiddlewareDescriptor IsDescriptor(Type type)
            => Match.Create<TaskMiddlewareDescriptor>(
                descriptor => descriptor.Type == type
                    && descriptor.Descriptor.Lifetime == ServiceLifetime.Transient);

        private TaskMiddlewareDescriptor IsDescriptor(ITaskMiddleware instance)
            => Match.Create<TaskMiddlewareDescriptor>(
                descriptor => descriptor.Type == instance.GetType()
                    && descriptor.Descriptor.ImplementationInstance == instance
                    && descriptor.Descriptor.Lifetime == ServiceLifetime.Singleton);
    }
}
