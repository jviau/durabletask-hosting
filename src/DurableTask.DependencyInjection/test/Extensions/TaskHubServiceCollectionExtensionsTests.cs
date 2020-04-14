using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static DurableTask.DependencyInjection.Tests.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddTaskHubWorker_ArgumentNull()
            => RunTestException<ArgumentNullException>(
                services => TaskHubServiceCollectionExtensions.AddTaskHubWorker(services, null));

        [Fact]
        public void AddTaskHubWorker_Func()
            => RunTest(
                null,
                services => services.AddTaskHubWorker(
                    builder =>
                    {
                        builder.Should().NotBeNull();
                    }),
                (original, returned) =>
                {
                    returned.Should().NotBeNull();
                    returned.Should().BeSameAs(original);
                    returned.Should().HaveCount(3);

                    ServiceDescriptor descriptor = original.Last();
                    descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
                    descriptor.ImplementationFactory.Should().NotBeNull();
                });

        private static void RunTestException<TException>(Action<IServiceCollection> act)
            where TException : Exception
        {
            bool Act(IServiceCollection services)
            {
                act(services);
                return true;
            }

            TException exception = Capture<TException>(() => RunTest(null, Act, null));
            exception.Should().NotBeNull();
        }

        private static void RunTest<TResult>(
            Action<IServiceCollection> arrange,
            Func<IServiceCollection, TResult> act,
            Action<IServiceCollection, TResult> verify)
        {
            var services = new ServiceCollection();
            arrange?.Invoke(services);
            TResult result = act(services);
            verify?.Invoke(services, result);
        }
    }
}
