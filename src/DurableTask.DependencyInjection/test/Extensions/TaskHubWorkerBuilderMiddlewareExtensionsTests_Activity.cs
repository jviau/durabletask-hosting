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
    public class TaskHubWorkerBuilderMiddlewareExtensionsTests_Activity
    {
        [Fact]
        public void UseActivityMiddlewareInstanceNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseActivityMiddleware(null, new TestMiddleware()));

        [Fact]
        public void UseActivityMiddlewareTypeNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseActivityMiddleware(null, typeof(TestMiddleware)));

        [Fact]
        public void UseActivityMiddlewareTypeNullInstance()
            => RunTestException<ArgumentNullException>(
                builder => builder.UseActivityMiddleware((ITaskMiddleware)null));

        [Fact]
        public void UseActivityMiddlewareTypeNullType()
            => RunTestException<ArgumentNullException>(
                builder => builder.UseActivityMiddleware((Type)null));

        [Fact]
        public void UseActivityMiddlewareGenericNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseActivityMiddleware<TestMiddleware>(null));

        [Fact]
        public void UseActivityMiddlewareFactoryNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderMiddlewareExtensions.UseActivityMiddleware(null, __ => new TestMiddleware()));

        [Fact]
        public void UseActivityMiddlewareInstance()
        {
            var instance = new TestMiddleware();
            RunTest(
                builder => builder.UseActivityMiddleware(instance),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseActivityMiddleware(IsDescriptor(instance)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });
        }

        [Fact]
        public void UseActivityMiddlewareType()
            => RunTest(
                builder => builder.UseActivityMiddleware(typeof(TestMiddleware)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseActivityMiddleware(IsDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void UseActivityMiddlewareGeneric()
            => RunTest(
                builder => builder.UseActivityMiddleware<TestMiddleware>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseActivityMiddleware(IsDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void UseActivityMiddlewareFactory()
            => RunTest(
                builder => builder.UseActivityMiddleware(_ => new TestMiddleware()),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseActivityMiddleware(IsDescriptor(typeof(TestMiddleware))), Times.Once);
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
            mock.Setup(m => m.UseActivityMiddleware(It.IsAny<TaskMiddlewareDescriptor>())).Returns(mock.Object);

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
