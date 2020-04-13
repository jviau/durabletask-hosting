using System;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DurableTask.DependencyInjection.Tests.Descriptors
{
    public class TaskActivityDescriptorTests
    {
        private const string Name = "TaskActivityDescriptorTests_Name";
        private const string Version = "TaskActivityDescriptorTests_Version";

        [Fact]
        public void SingletonByType()
        {
            RunTest<TestActivity>(
                           () => TaskActivityDescriptor.Singleton(typeof(TestActivity)),
                           ServiceLifetime.Singleton);
        }

        [Fact]
        public void SingletonByTypeNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton(typeof(TestActivity), Name, Version),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByTypeNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskActivityDescriptor.Singleton((Type)null));

        [Fact]
        public void SingleByTypeAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskActivityDescriptor.Singleton(typeof(TaskActivity)));

        [Fact]
        public void SingletonByGeneric()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton<TestActivity>(),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByGenericNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton<TestActivity>(Name, Version),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByGenericAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskActivityDescriptor.Singleton<TaskActivity>());

        [Fact]
        public void SingletonByInstance()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton(new TestActivity()),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByInstanceNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton(new TestActivity(), Name, Version),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByInstanceNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskActivityDescriptor.Singleton((TaskActivity)null));

        [Fact]
        public void SingletonByFactory()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton(_ => new TestActivity()),
                ServiceLifetime.Singleton);

        [Fact]
        public void SingletonByFactoryNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Singleton(_ => new TestActivity(), Name, Version),
                Name,
                Version,
                ServiceLifetime.Singleton);

        [Fact]
        public void SingleByFactoryNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskActivityDescriptor.Singleton((Func<IServiceProvider, TaskActivity>)null));

        [Fact]
        public void SingleByFactoryAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskActivityDescriptor.Singleton(_ => new TestActivity() as TaskActivity));

        [Fact]
        public void TransientByType()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Transient(typeof(TestActivity)),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByTypeNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Transient(typeof(TestActivity), Name, Version),
                Name,
                Version,
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByTypeNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskActivityDescriptor.Transient((Type)null));

        [Fact]
        public void TransientByTypeAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskActivityDescriptor.Transient(typeof(TaskActivity)));

        [Fact]
        public void TransientByGeneric()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Transient<TestActivity>(),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByGenericNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Transient<TestActivity>(Name, Version),
                Name,
                Version,
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByGenericAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskActivityDescriptor.Transient<TaskActivity>());

        [Fact]
        public void TransientByFactory()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Transient(_ => new TestActivity()),
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByFactoryNamed()
            => RunTest<TestActivity>(
                () => TaskActivityDescriptor.Transient(_ => new TestActivity(), Name, Version),
                Name,
                Version,
                ServiceLifetime.Transient);

        [Fact]
        public void TransientByFactoryNull()
            => RunExceptionTest<ArgumentNullException>(
                () => TaskActivityDescriptor.Transient((Func<IServiceProvider, TaskActivity>)null));

        [Fact]
        public void TransientByFactoryAbstract()
            => RunExceptionTest<ArgumentException>(
                () => TaskActivityDescriptor.Transient(_ => new TestActivity() as TaskActivity));

        private void RunTest<TMiddleware>(Func<TaskActivityDescriptor> test, ServiceLifetime serviceLifetime)
            => RunTest<TMiddleware>(test, typeof(TMiddleware).FullName, string.Empty, serviceLifetime);

        private void RunTest<TMiddleware>(
            Func<TaskActivityDescriptor> test, string name, string version, ServiceLifetime serviceLifetime)
        {
            TaskActivityDescriptor descriptor = test();

            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(typeof(TMiddleware));
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
            descriptor.Descriptor.Should().NotBeNull();
            descriptor.Descriptor.Lifetime.Should().Be(serviceLifetime);
        }

        private void RunExceptionTest<TException>(Func<TaskActivityDescriptor> test)
            where TException : Exception
        {
            TException exception = TestHelpers.Capture<TException>(() => test());
            exception.Should().NotBeNull();
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
