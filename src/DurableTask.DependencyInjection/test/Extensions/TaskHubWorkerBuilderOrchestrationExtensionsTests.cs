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

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubWorkerBuilderOrchestrationExtensionsTests
    {
        private const string Name = "TaskHubWorkerBuilderOrchestrationExtensionsTests_Name";
        private const string Version = "TaskHubWorkerBuilderOrchestrationExtensionsTests_Version";

        [Fact]
        public void AddOrchestrationDescriptor_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    null, new TaskOrchestrationDescriptor(typeof(TestOrchestration))));

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
        public void AddOrchestrationDescriptor_ArgumentNullDescriptor()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), (TaskOrchestrationDescriptor)null));

        [Fact]
        public void AddOrchestrationType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.AddOrchestration(
                    Mock.Of<ITaskHubWorkerBuilder>(), (Type)null));

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
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Orchestrations.Should().HaveCount(3);
                    original.Orchestrations.Should().OnlyContain(x => x.Type == typeof(TestOrchestration));
                    original.Activities.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 3)]
        public void AddOrchestrationType_Added2(bool includeAliases, int count)
            => RunTest(
                builder => builder.AddOrchestration(typeof(TestOrchestration), includeAliases),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Orchestrations.Should().HaveCount(count);
                    original.Orchestrations.Should().OnlyContain(x => x.Type == typeof(TestOrchestration));
                    original.Activities.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddOrchestrationTypeNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddOrchestration(typeof(TestOrchestration), name, version),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Orchestrations.Should().HaveCount(1);

                    TaskOrchestrationDescriptor descriptor = original.Orchestrations.Single();
                    descriptor.Type.Should().Be(typeof(TestOrchestration));
                    descriptor.Name.Should().Be(name);
                    descriptor.Version.Should().Be(version);

                    original.Activities.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Fact]
        public void AddOrchestrationGeneric_Added()
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Orchestrations.Should().HaveCount(3);
                    original.Orchestrations.Should().OnlyContain(x => x.Type == typeof(TestOrchestration));
                    original.Activities.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 3)]
        public void AddOrchestrationGeneric_Added2(bool includeAliases, int count)
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(includeAliases),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Orchestrations.Should().HaveCount(count);
                    original.Orchestrations.Should().OnlyContain(x => x.Type == typeof(TestOrchestration));
                    original.Activities.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Theory]
        [InlineData(Name, Version)]
        [InlineData(Name, "")]
        public void AddOrchestrationGenericNamed_Added(string name, string version)
            => RunTest(
                builder => builder.AddOrchestration<TestOrchestration>(name, version),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.Orchestrations.Should().HaveCount(1);

                    TaskOrchestrationDescriptor descriptor = original.Orchestrations.Single();
                    descriptor.Type.Should().Be(typeof(TestOrchestration));
                    descriptor.Name.Should().Be(name);
                    descriptor.Version.Should().Be(version);

                    original.Activities.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                    original.OrchestrationMiddleware.Should().HaveCount(1);
                });

        [Fact]
        public void AddMiddlewareType_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    null, typeof(TestMiddleware)));

        [Fact]
        public void AddMiddlewareDescriptor_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    null, typeof(TestMiddleware)));

        [Fact]
        public void AddMiddlewareDescriptor_ArgumentNullDescriptor()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), (TaskMiddlewareDescriptor)null));

        [Fact]
        public void AddMiddlewareType_ArgumentNullType()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderOrchestrationExtensions.UseOrchestrationMiddleware(
                    Mock.Of<ITaskHubWorkerBuilder>(), (Type)null));

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
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.OrchestrationMiddleware.Should().HaveCount(2);
                    original.OrchestrationMiddleware.Last().Type.Should().Be(typeof(TestMiddleware));
                    original.Activities.Should().BeEmpty();
                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
                });

        [Fact]
        public void AddMiddlewareGeneric_Added()
            => RunTest(
                builder => builder.UseOrchestrationMiddleware<TestMiddleware>(),
                (original, result) =>
                {
                    result.Should().NotBeNull();
                    result.Should().BeSameAs(original);
                    original.OrchestrationMiddleware.Should().HaveCount(2);
                    original.OrchestrationMiddleware.Last().Type.Should().Be(typeof(TestMiddleware));
                    original.Activities.Should().BeEmpty();
                    original.Orchestrations.Should().BeEmpty();
                    original.ActivityMiddleware.Should().HaveCount(1);
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
