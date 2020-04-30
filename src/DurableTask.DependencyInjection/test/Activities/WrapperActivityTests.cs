using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.DependencyInjection.Activities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Activities
{
    public class WrapperActivityTests
    {
        private const string InstanceId = "WrapperActivityTests_InstanceId";
        private static readonly string s_input = "Input";

        private static readonly TaskContext s_taskContext
            = new TaskContext(
                new OrchestrationInstance
                {
                    InstanceId = InstanceId,
                });

        private TaskContext InvokedContext { get; set; }
        private string InvokedInput { get; set; }

        [Fact]
        public void Ctor_ArgumentNull()
            => RunTestException<ArgumentNullException>(
                _ => new WrapperActivity(null));

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(TaskActivity))]
        public void Ctor_ArgumentInvalidType(Type type)
            => RunTestException<ArgumentException>(
                _ => new WrapperActivity(type));

        [Fact]
        public void Ctor_InnerActivityTypeSet()
            => RunTest(
                typeof(TestActivity),
                _ => new WrapperActivity(typeof(TestActivity)),
                (_, wrapper) =>
                {
                    wrapper.InnerActivityType.Should().Be(typeof(TestActivity));
                    wrapper.InnerActivity.Should().BeNull();
                });

        [Fact]
        public void Run_InvalidOperation()
            => RunTestException<InvalidOperationException>(
                wrapper => wrapper.Run(s_taskContext, s_input));

        [Fact]
        public void Run_InvokesInner()
            => RunTest(
                typeof(TestActivity),
                wrapper =>
                {
                    CreateScope();
                    wrapper.InnerActivity = (TaskActivity)Activator.CreateInstance(wrapper.InnerActivityType, this);
                    return wrapper.Run(s_taskContext, s_input);
                },
                (wrapper, result) =>
                {
                    result.Should().Be(s_input);
                    InvokedContext.Should().Be(s_taskContext);
                    InvokedInput.Should().Be(s_input);
                });

        [Fact]
        public Task RunAsync_InvalidOperation()
            => RunTestExceptionAsync<InvalidOperationException>(
                wrapper => wrapper.RunAsync(s_taskContext, s_input));

        [Theory]
        [InlineData(typeof(TestActivityOfT))]
        [InlineData(typeof(AsyncTestActivity))]
        public Task RunAsync_InvokesInner(Type type)
            => RunTestAsync(
                type,
                wrapper =>
                {
                    CreateScope();
                    wrapper.InnerActivity = (TaskActivity)Activator.CreateInstance(wrapper.InnerActivityType, this);
                    return wrapper.RunAsync(s_taskContext, $"[ \"{s_input}\" ]");
                },
                (wrapper, result) =>
                {
                    result.Should().Be($"\"{s_input}\"");
                    InvokedContext.Should().Be(s_taskContext);
                    InvokedInput.Should().Be(s_input);
                });

        private static IOrchestrationScope CreateScope()
        {
            IServiceScopeFactory factory = Mock.Of<IServiceScopeFactory>(
                m => m.CreateScope() == Mock.Of<IServiceScope>());
            IServiceProvider serviceProvider = Mock.Of<IServiceProvider>(
                m => m.GetService(typeof(IServiceScopeFactory)) == factory);

            return OrchestrationScope.CreateScope(InstanceId, serviceProvider);
        }

        private static void RunTestException<TException>(Action<WrapperActivity> act)
            where TException : Exception
        {
            bool Act(WrapperActivity wrapper)
            {
                act(wrapper);
                return true;
            }

            TException exception = Capture<TException>(
                () => RunTest(typeof(TestActivity), Act, null));
            exception.Should().NotBeNull();
        }

        private static async Task RunTestExceptionAsync<TException>(Func<WrapperActivity, Task> act)
            where TException : Exception
        {
            async Task<bool> Act(WrapperActivity wrapper)
            {
                await act(wrapper);
                return true;
            }

            TException exception = await Capture<TException>(
                () => RunTestAsync(typeof(TestActivity), Act, null));
            exception.Should().NotBeNull();
        }

        private static void RunTest<TResult>(
            Type innerType,
            Func<WrapperActivity, TResult> act,
            Action<WrapperActivity, TResult> verify)
        {
            var services = new WrapperActivity(innerType);
            TResult result = act(services);
            verify?.Invoke(services, result);
        }

        private static async Task RunTestAsync<TResult>(
            Type innerType,
            Func<WrapperActivity, Task<TResult>> act,
            Action<WrapperActivity, TResult> verify)
        {
            var services = new WrapperActivity(innerType);
            TResult result = await act(services);
            verify?.Invoke(services, result);
        }

        private class TestActivity : TaskActivity
        {
            private readonly WrapperActivityTests _test;

            public TestActivity(WrapperActivityTests test)
            {
                _test = test;
            }

            public override string Run(TaskContext context, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                return input;
            }
        }

        private class TestActivityOfT : TaskActivity<string, string>
        {
            private readonly WrapperActivityTests _test;

            public TestActivityOfT(WrapperActivityTests test)
            {
                _test = test;
            }

            protected override string Execute(TaskContext context, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                return input;
            }
        }

        private class AsyncTestActivity : AsyncTaskActivity<string, string>
        {
            private readonly WrapperActivityTests _test;

            public AsyncTestActivity(WrapperActivityTests test)
            {
                _test = test;
            }

            protected override Task<string> ExecuteAsync(TaskContext context, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                return Task.FromResult(input);
            }
        }
    }
}
