using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.DependencyInjection.Tests.TestHelpers;

namespace DurableTask.DependencyInjection.Tests
{
    public class DefaultTaskHubWorkerBuilderTests
    {
        [Fact]
        public void CtorArgumentNull()
            => RunTestException<ArgumentNullException>(_ => new DefaultTaskHubWorkerBuilder(null));

        [Fact]
        public void AddActivityNull()
            => RunTestException<ArgumentNullException>(b => b.AddActivity(null));

        [Fact]
        public void AddActivityMiddlewareNull()
            => RunTestException<ArgumentNullException>(b => b.UseActivityMiddleware(null));

        [Fact]
        public void AddOrchestrationNull()
            => RunTestException<ArgumentNullException>(b => b.AddOrchestration(null));

        [Fact]
        public void AddOrchestrationMiddlewareNull()
            => RunTestException<ArgumentNullException>(b => b.UseOrchestrationMiddleware(null));

        [Fact]
        public void BuildNullServiceProvider()
            => RunTestException<ArgumentNullException>(b => b.Build(null));

        [Fact]
        public void BuildOrchestrationNotSet()
            => RunTestException<InvalidOperationException>(b => b.Build(Mock.Of<IServiceProvider>()));

        [Fact]
        public void AddActivity()
        {
            TaskActivityDescriptor descriptor = TaskActivityDescriptor.Singleton<TestActivity>();
            RunTest(
                null,
                b => b.AddActivity(descriptor),
                (_, services) => services.Should().Contain(descriptor.Descriptor));
        }

        [Fact]
        public void AddActivityMiddleware()
        {
            TaskMiddlewareDescriptor descriptor = TaskMiddlewareDescriptor.Singleton<TestMiddleware>();
            RunTest(
                null,
                b => b.UseActivityMiddleware(descriptor),
                (_, services) => services.Should().Contain(descriptor.Descriptor));
        }

        [Fact]
        public void AddOrchestration()
        {
            TaskOrchestrationDescriptor descriptor = TaskOrchestrationDescriptor.Singleton<TestOrchestration>();
            RunTest(
                null,
                b => b.AddOrchestration(descriptor),
                (_, services) => services.Should().Contain(descriptor.Descriptor));
        }

        [Fact]
        public void AddOrchestrationMiddleware()
        {
            TaskMiddlewareDescriptor descriptor = TaskMiddlewareDescriptor.Singleton<TestMiddleware>();
            RunTest(
                null,
                b => b.UseOrchestrationMiddleware(descriptor),
                (_, services) => services.Should().Contain(descriptor.Descriptor));
        }

        [Fact]
        public void BuildTaskHubWorker()
        {
            RunTest(
                null,
                b =>
                {
                    b.OrchestrationService = Mock.Of<IOrchestrationService>();
                    TaskHubWorker worker = b.Build(Mock.Of<IServiceProvider>());
                    worker.Should().NotBeNull();
                    worker.orchestrationService.Should().Be(b.OrchestrationService);
                },
                null);
        }

        private static void RunTestException<TException>(Action<DefaultTaskHubWorkerBuilder> act)
            where TException : Exception
        {
            TException exception = Capture<TException>(() => RunTest(null, act, null));
            exception.Should().NotBeNull();
        }

        private static void RunTest(
            Action<IServiceCollection> arrange,
            Action<DefaultTaskHubWorkerBuilder> act,
            Action<DefaultTaskHubWorkerBuilder, IServiceCollection> verify)
        {
            var services = new ServiceCollection();
            arrange?.Invoke(services);
            var builder = new DefaultTaskHubWorkerBuilder(services);

            act(builder);

            verify?.Invoke(builder, services);
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

        private class TestActivity : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }

        private class TestMiddleware : ITaskMiddleware
        {
            public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
            {
                throw new NotImplementedException();
            }
        }
    }
}
