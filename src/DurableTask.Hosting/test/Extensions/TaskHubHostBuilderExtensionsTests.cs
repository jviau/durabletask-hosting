using System;
using DurableTask.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.Hosting.Tests.Extensions
{
    public class TaskHubHostBuilderExtensionsTests
    {
        [Fact]
        public void ConfigureTaskHubWorker_ArgumentNullBuilder()
            => RunTestException<ArgumentNullException>(
                builder => TaskHubHostBuilderExtensions
                    .ConfigureTaskHubWorker(null, b => { }));
        [Fact]
        public void ConfigureTaskHubWorker_ArgumentNullConfigure()
            => RunTestException<ArgumentNullException>(
                builder => TaskHubHostBuilderExtensions
                    .ConfigureTaskHubWorker(builder, (Action<ITaskHubWorkerBuilder>)null));

        [Fact]
        public void ConfigureTaskHubWorker_ArgumentNullBuilder2()
            => RunTestException<ArgumentNullException>(
                builder => TaskHubHostBuilderExtensions
                    .ConfigureTaskHubWorker(null, (c, b)  => { }));
        [Fact]
        public void ConfigureTaskHubWorker_ArgumentNullConfigure2()
            => RunTestException<ArgumentNullException>(
                builder => TaskHubHostBuilderExtensions
                    .ConfigureTaskHubWorker(builder, (Action<HostBuilderContext, ITaskHubWorkerBuilder>)null));

        [Fact]
        public void ConfigureTaskHubWorker_Callback()
        {
            ITaskHubWorkerBuilder capturedBuilder = null;
            RunTest(
                builder => builder.ConfigureTaskHubWorker(b => capturedBuilder = b),
                (builder, returned) =>
                {
                    returned.Should().BeSameAs(builder);
                    capturedBuilder.Should().NotBeNull();
                });
        }

        [Fact]
        public void ConfigureTaskHubWorker_Callback2()
        {
            HostBuilderContext capturedContext = null;
            ITaskHubWorkerBuilder capturedBuilder = null;
            RunTest(
                builder => builder.ConfigureTaskHubWorker((c, b) =>
                {
                    capturedContext = c;
                    capturedBuilder = b;
                }),
                (builder, returned) =>
                {
                    returned.Should().BeSameAs(builder);
                    capturedContext.Should().NotBeNull();
                    capturedBuilder.Should().NotBeNull();
                });
        }

        private static void RunTestException<TException>(Action<IHostBuilder> act)
            where TException : Exception
        {
            bool Act(IHostBuilder builder)
            {
                act(builder);
                return true;
            }

            TException exception = Capture<TException>(() => RunTest(Act, null));
            exception.Should().NotBeNull();
        }

        private static void RunTest<TResult>(
            Func<IHostBuilder, TResult> act,
            Action<IHostBuilder, TResult> verify)
        {
            var builder = new HostBuilder();

            TResult result = act(builder);
            builder.Build();
            verify?.Invoke(builder, result);
        }
    }
}
