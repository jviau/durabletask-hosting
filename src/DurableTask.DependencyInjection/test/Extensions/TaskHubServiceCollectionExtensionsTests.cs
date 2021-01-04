using System;
using System.Linq;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddTaskHubWorker_ArgumentNullServices()
            => RunTestException<ArgumentNullException>(
                services => TaskHubServiceCollectionExtensions.AddTaskHubWorker(null, c => { }));

        [Fact]
        public void AddTaskHubWorker_ArgumentNullConfigure()
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
                    returned.Should().ContainSingle(sd => sd.ServiceType == typeof(ITaskHubWorkerBuilder)
                        && sd.ImplementationInstance.GetType() == typeof(DefaultTaskHubWorkerBuilder)
                        && sd.Lifetime == ServiceLifetime.Singleton);

                    returned.Should().ContainSingle(sd => sd.ServiceType == typeof(TaskHubWorker)
                        && sd.ImplementationFactory != null && sd.Lifetime == ServiceLifetime.Singleton);
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
