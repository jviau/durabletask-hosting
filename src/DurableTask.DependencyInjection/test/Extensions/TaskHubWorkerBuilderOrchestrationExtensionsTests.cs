using System;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.DependencyInjection.Tests.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubWorkerBuilderOrchestrationExtensionsTests
    {
        private const string Name = "TaskHubWorkerBuilderOrchestrationExtensionsTests_Name";
        private const string Version = "TaskHubWorkerBuilderOrchestrationExtensionsTests_Version";

        [Fact]
        public void AddOrchestrationTypeNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(null, typeof(TestOrchestration)));

        [Fact]
        public void AddOrchestrationTypeNullType()
            => RunTestException<ArgumentNullException>(
                builder => builder.AddOrchestration((Type)null));

        [Fact]
        public void AddOrchestrationGenericNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<TestOrchestration>(null));

        [Fact]
        public void AddOrchestrationFactoryNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(null, __ => new TestOrchestration()));

        [Fact]
        public void AddOrchestrationTypeNamedNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(null, typeof(TestOrchestration), Name, Version));

        [Fact]
        public void AddOrchestrationTypeNamedNullType()
            => RunTestException<ArgumentNullException>(
                builder => builder.AddOrchestration(null, Name, Version));

        [Fact]
        public void AddOrchestrationGenericNamedNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<TestOrchestration>(null, Name, Version));

        [Fact]
        public void AddOrchestrationFactoryNamedNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(null, __ => new TestOrchestration(), Name, Version));

        [Fact]
        public void AddOrchestrationType()
            => RunTest(
                builder => builder.AddOrchestration(typeof(TestOrchestration)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(IsDescriptor(typeof(TestOrchestration))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddOrchestrationGeneric()
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(IsDescriptor(typeof(TestOrchestration))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddOrchestrationFactory()
            => RunTest(
                builder => builder.AddOrchestration(_ => new TestOrchestration()),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(IsDescriptor(typeof(TestOrchestration))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddOrchestrationTypeNamed()
            => RunTest(
                builder => builder.AddOrchestration(typeof(TestOrchestration), Name, Version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(IsDescriptor(typeof(TestOrchestration), Name, Version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddOrchestrationGenericNamed()
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(Name, Version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(IsDescriptor(typeof(TestOrchestration), Name, Version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddOrchestrationFactoryNamed()
            => RunTest(
                builder => builder.AddOrchestration(_ => new TestOrchestration(), Name, Version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(IsDescriptor(typeof(TestOrchestration), Name, Version)), Times.Once);
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
            mock.Setup(m => m.AddOrchestration(It.IsAny<TaskOrchestrationDescriptor>())).Returns(mock.Object);

            TResult result = act(mock.Object);
            verify?.Invoke(mock, result);
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

        private TaskOrchestrationDescriptor IsDescriptor(Type type)
            => Match.Create<TaskOrchestrationDescriptor>(
                descriptor => descriptor.Type == type
                    && descriptor.Descriptor.Lifetime == ServiceLifetime.Transient);

        private TaskOrchestrationDescriptor IsDescriptor(Type type, string name, string version)
            => Match.Create<TaskOrchestrationDescriptor>(
                descriptor => descriptor.Name == name
                    && descriptor.Version == version
                    && descriptor.Type == type
                    && descriptor.Descriptor.Lifetime == ServiceLifetime.Transient);
    }
}
