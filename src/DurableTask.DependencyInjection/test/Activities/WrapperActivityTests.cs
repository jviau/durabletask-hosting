// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Activities.Tests
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

        [Fact]
        public void Ctor_InnerActivityTypeSet()
            => RunTest(
                typeof(TestActivity),
                _ => new WrapperActivity(new TaskActivityDescriptor(typeof(TestActivity))),
                (_, wrapper) =>
                {
                    wrapper.Descriptor.Should().NotBeNull();
                    wrapper.Descriptor.Type.Should().Be(typeof(TestActivity));
                    wrapper.InnerActivity.Should().BeNull();
                });

        [Fact]
        public void Run_InvalidOperation()
            => RunTestException<InvalidOperationException>(
                wrapper => wrapper.Run(s_taskContext, s_input));

        [Fact]
        public void Run_Type_InvokesInner()
            => RunTest(
                typeof(TestActivity),
                wrapper =>
                {
                    CreateScope();
                    wrapper.Initialize(CreateServiceProvider());
                    return wrapper.Run(s_taskContext, s_input);
                },
                (wrapper, result) =>
                {
                    result.Should().Be(s_input);
                    InvokedContext.Should().Be(s_taskContext);
                    InvokedInput.Should().Be(s_input);
                });

        [Fact]
        public async Task Run_Method_InvokesInner()
        {
            // arrange
            var methodInfo = typeof(IMyService).GetMethod(nameof(IMyService.MyMethodAsync));
            var descriptor = new TaskActivityDescriptor(methodInfo);
            var wrapperActivity = new WrapperActivity(descriptor);
            var services = new ServiceCollection();
            services.AddSingleton<IMyService, MyService>();
            var input = new JArray()
            {
                "some_string",
                10
            };

            // act
            CreateScope();
            wrapperActivity.Initialize(services.BuildServiceProvider());
            string result = await wrapperActivity.RunAsync(s_taskContext, input.ToString());

            // assert
            string parsed = JsonConvert.DeserializeObject<string>(result);
            parsed.Should().Be("some_string|10");
        }

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
                    wrapper.Initialize(CreateServiceProvider());
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
            var services = new WrapperActivity(new TaskActivityDescriptor(innerType));
            TResult result = act(services);
            verify?.Invoke(services, result);
        }

        private static async Task RunTestAsync<TResult>(
            Type innerType,
            Func<WrapperActivity, Task<TResult>> act,
            Action<WrapperActivity, TResult> verify)
        {
            var services = new WrapperActivity(new TaskActivityDescriptor(innerType));
            TResult result = await act(services);
            verify?.Invoke(services, result);
        }

        private IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(this);
            return services.BuildServiceProvider();
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

        private interface IMyService
        {
            Task<string> MyMethodAsync(string input1, int input2);
        }

        private class MyService : IMyService
        {
            public Task<string> MyMethodAsync(string input1, int input2)
            {
                return Task.FromResult($"{input1}|{input2}");
            }
        }
    }
}
