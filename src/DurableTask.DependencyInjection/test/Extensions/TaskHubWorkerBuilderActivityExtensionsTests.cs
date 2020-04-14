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
    public class TaskHubWorkerBuilderActivityExtensionsTests
    {
        private const string Name = "TaskHubWorkerBuilderActivityExtensionsTests_Name";
        private const string Version = "TaskHubWorkerBuilderActivityExtensionsTests_Version";

        [Fact]
        public void AddActivityType_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(null, typeof(TestActivity)));

        [Fact]
        public void AddActivityType_ArgumentNullBuilder2()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    null, typeof(TestActivity), Name, Version));

        [Fact]
        public void AddActivityType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), null));

        [Theory]
        [InlineData(null, Name, Version)]
        [InlineData(typeof(TestActivity), null, Version)]
        [InlineData(typeof(TestActivity), Name, null)]
        public void AddActivityType_ArgumentNull(Type type, string name, string version)
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), type, name, version));

        [Fact]
        public void AddActivityType_ArgumentEmptyName()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), typeof(TestActivity), string.Empty, Version));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(AbstractActivity))]
        public void AddActivityType_ArgumentInvalidType(Type type)
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), type));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(AbstractActivity))]
        public void AddActivityType_ArgumentInvalidType2(Type type)
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), type, Name, Version));

        [Fact]
        public void AddActivityGeneric_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<TestActivity>(null));


        [Fact]
        public void AddActivityGeneric_ArgumentNullBuilder2()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<TestActivity>(
                    null, Name, Version));

        [Theory]
        [InlineData(Name, null)]
        [InlineData(null, Version)]
        public void AddActivityGeneric_ArgumentNull(string name, string version)
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<TestActivity>(
                    Mock.Of<ITaskHubWorkerBuilder>(), name, version));

        [Fact]
        public void AddActivityGeneric_ArgumentEmptyName()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<TestActivity>(
                    Mock.Of<ITaskHubWorkerBuilder>(), string.Empty, Version));

        [Fact]
        public void AddActivityGeneric_ArgumentInvalidType()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<AbstractActivity>(
                    Mock.Of<ITaskHubWorkerBuilder>()));

        [Fact]
        public void AddActivityGeneric_ArgumentInvalidType2()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<AbstractActivity>(
                    Mock.Of<ITaskHubWorkerBuilder>(), Name, Version));

        [Fact]
        public void AddActivityType_Added()
            => RunTest(
                builder => builder.AddActivity(typeof(TestActivity)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(
                        IsActivityDescriptor(typeof(TestActivity))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddActivityTypeNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddActivity(typeof(TestActivity), name, version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(
                        IsActivityDescriptor(typeof(TestActivity), name, version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddActivityGeneric_Added()
            => RunTest(
                builder => builder.AddActivity<TestActivity>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(
                        IsActivityDescriptor(typeof(TestActivity))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddActivityGenericNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddActivity<TestActivity>(name, version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(
                        IsActivityDescriptor(typeof(TestActivity), name, version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddMiddlewareType_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    null, typeof(TestMiddleware)));

        [Fact]
        public void AddMiddlewareType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), null));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(AbstractMiddleware))]
        public void AddMiddlewareType_ArgumentInvalidType(Type type)
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), type));

        [Fact]
        public void AddMiddlewareGeneric_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware<TestMiddleware>(null));

        [Fact]
        public void AddMiddlewareGeneric_ArgumentInvalidType()
            => RunTestException<ArgumentException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware<AbstractMiddleware>(
                    Mock.Of<ITaskHubWorkerBuilder>()));

        [Fact]
        public void AddMiddlewareType_Added()
            => RunTest(
                builder => builder.UseActivityMiddleware(typeof(TestMiddleware)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseActivityMiddleware(
                        IsMiddlewareDescriptor(typeof(TestMiddleware))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddMiddlewareGeneric_Added()
            => RunTest(
                builder => builder.UseActivityMiddleware<TestMiddleware>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.UseActivityMiddleware(
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
                .Setup(m => m.AddActivity(It.IsAny<TaskActivityDescriptor>()))
                .Returns(mock.Object);

            mock
                .Setup(m => m.UseActivityMiddleware(It.IsAny<TaskMiddlewareDescriptor>()))
                .Returns(mock.Object);

            TResult result = act(mock.Object);
            verify?.Invoke(mock, result);
        }

        private TaskActivityDescriptor IsActivityDescriptor(Type type)
            => Match.Create<TaskActivityDescriptor>(
                descriptor => descriptor.Type == type);

        private TaskActivityDescriptor IsActivityDescriptor(Type type, string name, string version)
            => Match.Create<TaskActivityDescriptor>(
                descriptor => descriptor.Name == name
                    && descriptor.Version == version
                    && descriptor.Type == type);

        private TaskMiddlewareDescriptor IsMiddlewareDescriptor(Type type)
            => Match.Create<TaskMiddlewareDescriptor>(
                descriptor => descriptor.Type == type);

        private class TestActivity : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }

        private abstract class AbstractActivity : TaskActivity
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
