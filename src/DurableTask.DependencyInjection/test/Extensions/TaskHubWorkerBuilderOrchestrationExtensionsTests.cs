using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using FluentAssertions;
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
        public void AddOrchestrationType_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(null, typeof(TestOrchestration)));

        [Fact]
        public void AddOrchestrationType_ArgumentNullBuilder2()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    null, typeof(TestOrchestration), Name, Version));

        [Fact]
        public void AddOrchestrationType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), null));

        [Theory]
        [InlineData(null, Name, Version)]
        [InlineData(typeof(TestOrchestration), null, Version)]
        [InlineData(typeof(TestOrchestration), Name, null)]
        public void AddOrchestrationType_ArgumentNull(Type type, string name, string version)
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), type, name, version));

        [Fact]
        public void AddOrchestrationType_ArgumentEmptyName()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), typeof(TestOrchestration), string.Empty, Version));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(AbstractOrchestration))]
        public void AddOrchestrationType_ArgumentInvalidType(Type type)
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), type));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(AbstractOrchestration))]
        public void AddOrchestrationType_ArgumentInvalidType2(Type type)
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), type, Name, Version));

        [Fact]
        public void AddOrchestrationGeneric_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<TestOrchestration>(null));


        [Fact]
        public void AddOrchestrationGeneric_ArgumentNullBuilder2()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<TestOrchestration>(
                    null, Name, Version));

        [Theory]
        [InlineData(Name, null)]
        [InlineData(null, Version)]
        public void AddOrchestrationGeneric_ArgumentNull(string name, string version)
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<TestOrchestration>(
                    Mock.Of<ITaskHubWorkerBuilder>(), name, version));

        [Fact]
        public void AddOrchestrationGeneric_ArgumentEmptyName()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<TestOrchestration>(
                    Mock.Of<ITaskHubWorkerBuilder>(), string.Empty, Version));

        [Fact]
        public void AddOrchestrationGeneric_ArgumentInvalidType()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<AbstractOrchestration>(
                    Mock.Of<ITaskHubWorkerBuilder>()));

        [Fact]
        public void AddOrchestrationGeneric_ArgumentInvalidType2()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration<AbstractOrchestration>(
                    Mock.Of<ITaskHubWorkerBuilder>(), Name, Version));

        [Fact]
        public void AddOrchestrationType_Added()
            => RunTest(
                builder => builder.AddOrchestration(typeof(TestOrchestration)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(
                        IsOrchestrationDescriptor(typeof(TestOrchestration))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddOrchestrationTypeNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddOrchestration(typeof(TestOrchestration), name, version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(
                        IsOrchestrationDescriptor(typeof(TestOrchestration), name, version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddOrchestrationGeneric_Added()
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(
                        IsOrchestrationDescriptor(typeof(TestOrchestration))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddOrchestrationGenericNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(name, version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddOrchestration(
                        IsOrchestrationDescriptor(typeof(TestOrchestration), name, version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddMiddlewareType_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    null, typeof(TestMiddleware)));

        [Fact]
        public void AddMiddlewareType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), null));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(AbstractMiddleware))]
        public void AddMiddlewareType_ArgumentInvalidType(Type type)
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), type));

        [Fact]
        public void AddMiddlewareGeneric_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware<TestMiddleware>(null));

        [Fact]
        public void AddMiddlewareGeneric_ArgumentInvalidType()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware<AbstractMiddleware>(
                    Mock.Of<ITaskHubWorkerBuilder>()));

        [Fact]
        public void AddMiddlewareType_Added()
            => RunTest(
                builder => builder.UseOrchestrationMiddleware(typeof(TestMiddleware)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseOrchestrationMiddleware(
                        IsMiddlewareDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddMiddlewareGeneric_Added()
            => RunTest(
                builder => builder.UseOrchestrationMiddleware<TestMiddleware>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseOrchestrationMiddleware(
                        IsMiddlewareDescriptor(typeof(TestMiddleware))), Times.Once);
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
            mock
                .Setup(m => m.AddOrchestration(It.IsAny<TaskOrchestrationDescriptor>()))
                .Returns(mock.Object);

            mock
                .Setup(m => m.UseOrchestrationMiddleware(It.IsAny<TaskMiddlewareDescriptor>()))
                .Returns(mock.Object);

            TResult result = act(mock.Object);
            verify?.Invoke(mock, result);
        }

        private TaskOrchestrationDescriptor IsOrchestrationDescriptor(Type type)
            => Match.Create<TaskOrchestrationDescriptor>(
                descriptor => descriptor.Type == type);

        private TaskOrchestrationDescriptor IsOrchestrationDescriptor(Type type, string name, string version)
            => Match.Create<TaskOrchestrationDescriptor>(
                descriptor => descriptor.Name == name
                    && descriptor.Version == version
                    && descriptor.Type == type);

        private TaskMiddlewareDescriptor IsMiddlewareDescriptor(Type type)
            => Match.Create<TaskMiddlewareDescriptor>(
                descriptor => descriptor.Type == type);

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

        private abstract class AbstractOrchestration : TaskOrchestration
        {
        }

        private class TestMiddleware : ITaskMiddleware
        {
            public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
            {
                throw new NotImplementedException();
            }
        }

        private abstract class AbstractMiddleware : ITaskMiddleware
        {
            public abstract Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next);
        }
    }
}
