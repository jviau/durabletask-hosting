using System;
using System.Linq;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Activities.Tests
{
    public class TaskHubWorkerBuilderActivityExtensionsTests
    {
        private const string Name = "TaskHubWorkerBuilderActivityExtensionsTests_Name";
        private const string Version = "TaskHubWorkerBuilderActivityExtensionsTests_Version";

        [Fact]
        public void AddActivityDescriptor_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    null, new TaskActivityDescriptor(typeof(TestActivity))));

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
        public void AddActivityDescriptor_ArgumentNullDescriptor()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), (TaskActivityDescriptor)null));

        [Fact]
        public void AddActivityType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(
                    Mock.Of<ITaskHubWorkerBuilder>(), (Type)null));

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
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Activities.Should().HaveCount(3);
                    original.Activities.Should().OnlyContain(x => x.Type == typeof(TestActivity));
                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 3)]
        public void AddActivityType_Added2(bool includeAliases, int count)
            => RunTest(
                builder => builder.AddActivity(typeof(TestActivity), includeAliases),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Activities.Should().HaveCount(count);
                    original.Activities.Should().OnlyContain(x => x.Type == typeof(TestActivity));
                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddActivityTypeNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddActivity(typeof(TestActivity), name, version),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Activities.Should().HaveCount(1);

                    TaskActivityDescriptor descriptor = original.Activities.Single();
                    descriptor.Type.Should().Be(typeof(TestActivity));
                    descriptor.Name.Should().Be(name);
                    descriptor.Version.Should().Be(version);

                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Fact]
        public void AddActivityGeneric_Added()
            => RunTest(
                builder => builder.AddActivity<TestActivity>(),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Activities.Should().HaveCount(3);
                    original.Activities.Should().OnlyContain(x => x.Type == typeof(TestActivity));
                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 3)]
        public void AddActivityGeneric_Added2(bool includeAliases, int count)
            => RunTest(
                builder => builder.AddActivity<TestActivity>(includeAliases),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Activities.Should().HaveCount(count);
                    original.Activities.Should().OnlyContain(x => x.Type == typeof(TestActivity));
                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddActivityGenericNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddActivity<TestActivity>(name, version),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Activities.Should().HaveCount(1);

                    TaskActivityDescriptor descriptor = original.Activities.Single();
                    descriptor.Type.Should().Be(typeof(TestActivity));
                    descriptor.Name.Should().Be(name);
                    descriptor.Version.Should().Be(version);

                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Fact]
        public void AddMiddlewareDescriptor_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    null, new TaskMiddlewareDescriptor(typeof(TestMiddleware))));

        [Fact]
        public void AddMiddlewareType_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    null, typeof(TestMiddleware)));

        [Fact]
        public void AddMiddlewareDescriptor_ArgumentNullDescriptor()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), (TaskMiddlewareDescriptor)null));

        [Fact]
        public void AddMiddlewareType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.UseActivityMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), (Type)null));

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
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.ActivityMiddleware.Should().HaveCount(2);
                    original.ActivityMiddleware.Last().Type.Should().Be(typeof(TestMiddleware));
                    original.Activities.Should().BeEmpty();
                    original.Orchestrations.Should().BeEmpty();
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Fact]
        public void AddMiddlewareGeneric_Added()
            => RunTest(
                builder => builder.UseActivityMiddleware<TestMiddleware>(),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.ActivityMiddleware.Should().HaveCount(2);
                    original.ActivityMiddleware.Last().Type.Should().Be(typeof(TestMiddleware));
                    original.Activities.Should().BeEmpty();
                    original.Orchestrations.Should().BeEmpty();
                    original.OrchestrationMiddleware.Should().HaveCount(1);
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
            Action<ITaskHubWorkerBuilder, TResult> verify)
        {
            var builder = new DefaultTaskHubWorkerBuilder(new ServiceCollection());
            TResult result = act(builder);
            verify?.Invoke(builder, result);
        }

        [TaskAlias(Name)]
        [TaskAlias(Name, Version)]
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
