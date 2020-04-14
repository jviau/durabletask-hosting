using System;
using DurableTask.Core;
using FluentAssertions;
using Moq;
using Xunit;
using static DurableTask.DependencyInjection.Tests.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubWorkerBuilderExtensionsTests
    {
        [Fact]
        public void WithOrchestrationService_ArgumentNull()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderExtensions.WithOrchestrationService(null, Mock.Of<IOrchestrationService>()));

        [Fact]
        public void WithOrchestration_ServiceIsSet()
            => RunTest(
                builder =>
                {
                    IOrchestrationService service = Mock.Of<IOrchestrationService>();
                    var returned = builder.WithOrchestrationService(service);

                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(returned);
                    return service;
                },
                (mock, service) =>
                {
                    mock.VerifySet(m => m.OrchestrationService = service, Times.Once);
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
            TResult result = act(mock.Object);
            verify?.Invoke(mock, result);
        }
    }
}
