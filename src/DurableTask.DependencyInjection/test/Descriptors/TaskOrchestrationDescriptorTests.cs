using System;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DurableTask.DependencyInjection.Tests.Descriptors
{
    public class TaskOrchestrationDescriptorTests
    {
        private const string Name = "TaskOrchestrationDescriptorTests_Name";
        private const string Version = "TaskOrchestrationDescriptorTests_Version";

        [Fact]
        public void SingletonByType()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton(typeof(TestOrchestration)),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByTypeNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton(Name, Version, typeof(TestOrchestration)),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByTypeNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskOrchestrationDescriptor.Singleton((Type)null));

        [Fact]
        public void SingleByTypeAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskOrchestrationDescriptor.Singleton(typeof(TaskOrchestration)));

        [Fact]
        public void SingletonByGeneric()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton<TestOrchestration>(),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByGenericNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton<TestOrchestration>(Name, Version),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByGenericAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskOrchestrationDescriptor.Singleton<TaskOrchestration>());

        [Fact]
        public void SingletonByInstance()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton(new TestOrchestration()),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByInstanceNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton(Name, Version, new TestOrchestration()),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByInstanceNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskOrchestrationDescriptor.Singleton((TaskOrchestration)null));

        [Fact]
        public void SingletonByFactory()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton(_ => new TestOrchestration()),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByFactoryNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Singleton(Name, Version, _ => new TestOrchestration()),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByFactoryNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskOrchestrationDescriptor.Singleton((Func<IServiceProvider, TaskOrchestration>)null));

        [Fact]
        public void SingleByFactoryAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskOrchestrationDescriptor.Singleton(_ => new TestOrchestration() as TaskOrchestration));

        [Fact]
        public void TransientByType()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Transient(typeof(TestOrchestration)),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByTypeNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Transient(Name, Version, typeof(TestOrchestration)),
                Name,
                Version,
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByTypeNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskOrchestrationDescriptor.Transient((Type)null));

        [Fact]
        public void TransientByTypeAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskOrchestrationDescriptor.Transient(typeof(TaskOrchestration)));

        [Fact]
        public void TransientByGeneric()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Transient<TestOrchestration>(),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByGenericNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Transient<TestOrchestration>(Name, Version),
                Name,
                Version,
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByGenericAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskOrchestrationDescriptor.Transient<TaskOrchestration>());

        [Fact]
        public void TransientByFactory()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Transient(_ => new TestOrchestration()),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByFactoryNamed()
            => RunTest<TestOrchestration>(
                () => TaskOrchestrationDescriptor.Transient(Name, Version, _ => new TestOrchestration()),
                Name,
                Version,
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByFactoryNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskOrchestrationDescriptor.Transient((Func<IServiceProvider, TaskOrchestration>)null));

        [Fact]
        public void TransientByFactoryAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskOrchestrationDescriptor.Transient(_ => new TestOrchestration() as TaskOrchestration));

        private void RunTest<TMiddleware>(Func<TaskOrchestrationDescriptor> test, ServiceLifetime serviceLifetime)
            => RunTest<TMiddleware>(test, typeof(TMiddleware).FullName, string.Empty, serviceLifetime);

        private void RunTest<TMiddleware>(
            Func<TaskOrchestrationDescriptor> test, string name, string version, ServiceLifetime serviceLifetime)
        {
            TaskOrchestrationDescriptor descriptor = test();

            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(typeof(TMiddleware));
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
            descriptor.Descriptor.Should().NotBeNull();
            descriptor.Descriptor.Lifetime.Should().Be(serviceLifetime);
        }

        private void RunExceptionTest<TException>(Func<TaskOrchestrationDescriptor> test)
            where TException : Exception
        {
            TException exception = TestHelpers.Capture<TException>(() => test());
            exception.Should().NotBeNull();
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
