using System;
using System.Threading.Tasks;
using DurableTask.Core.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DurableTask.DependencyInjection.Tests.Descriptors
{
    public class TaskMiddlewareDescriptorTests
    {
        [Fact]
        public void SingletonByType()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Singleton(typeof(TestMiddleware)),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByTypeNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskMiddlewareDescriptor.Singleton((Type)null));

        [Fact]
        public void SingleByTypeAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskMiddlewareDescriptor.Singleton(typeof(AbstractMiddleware)));

        [Fact]
        public void SingletonByGeneric()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Singleton<TestMiddleware>(),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByGenericAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskMiddlewareDescriptor.Singleton<AbstractMiddleware>());

        [Fact]
        public void SingletonByInstance()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Singleton(new TestMiddleware()),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByInstanceNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskMiddlewareDescriptor.Singleton((ITaskMiddleware)null));

        [Fact]
        public void SingletonByFactory()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Singleton(_ => new TestMiddleware()),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByFactoryNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskMiddlewareDescriptor.Singleton((Func<IServiceProvider, ITaskMiddleware>)null));

        [Fact]
        public void SingleByFactoryAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskMiddlewareDescriptor.Singleton(_ => new TestMiddleware() as AbstractMiddleware));

        [Fact]
        public void TransientByType()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Transient(typeof(TestMiddleware)),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByTypeNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskMiddlewareDescriptor.Transient((Type)null));

        [Fact]
        public void TransientByTypeAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskMiddlewareDescriptor.Transient(typeof(AbstractMiddleware)));

        [Fact]
        public void TransientByGeneric()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Transient<TestMiddleware>(),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByGenericAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskMiddlewareDescriptor.Transient<AbstractMiddleware>());

        [Fact]
        public void TransientByFactory()
            => RunTest<TestMiddleware>(
                () => TaskMiddlewareDescriptor.Transient(_ => new TestMiddleware()),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByFactoryNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskMiddlewareDescriptor.Transient((Func<IServiceProvider, ITaskMiddleware>)null));

        [Fact]
        public void TransientByFactoryAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskMiddlewareDescriptor.Transient(_ => new TestMiddleware() as AbstractMiddleware));

        private void RunTest<TMiddleware>(Func<TaskMiddlewareDescriptor> test, ServiceLifetime serviceLifetime)
        {
            TaskMiddlewareDescriptor descriptor = test();

            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(typeof(TMiddleware));
            descriptor.Descriptor.Should().NotBeNull();
            descriptor.Descriptor.Lifetime.Should().Be(serviceLifetime);
        }

        private void RunExceptionTest<TException>(Func<TaskMiddlewareDescriptor> test)
            where TException : Exception
        {
            TException exception = TestHelpers.Capture<TException>(() => test());
            exception.Should().NotBeNull();
        }

        private class TestMiddleware : AbstractMiddleware
        {
        }

        private abstract class AbstractMiddleware : ITaskMiddleware
        {
            public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
            {
                throw new NotImplementedException();
            }
        }
    }
}
