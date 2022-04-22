// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Orchestrations.Tests
{
    public class WrapperOrchestrationTests
    {
        private const string Input = "Input";
        private const string Event = "Event";
        private const string InstanceId = "WrapperOrchestrationTests_InstanceId";

        private static readonly OrchestrationContext s_orchestrationContext
            = new TestOrchestrationContext(
                new OrchestrationInstance
                {
                    InstanceId = InstanceId,
                });

        private OrchestrationContext InvokedContext { get; set; }

        private string InvokedInput { get; set; }

        private string EventRaised { get; set; }

        [Fact]
        public void Ctor_ArgumentNull()
            => RunTestException<ArgumentNullException>(
                _ => new WrapperOrchestration(null));

        [Fact]
        public void Ctor_InnerActivityTypeSet()
            => RunTest(
                typeof(TestOrchestration),
                _ => new WrapperOrchestration(new TaskOrchestrationDescriptor(typeof(TestOrchestration))),
                (_, wrapper) =>
                {
                    wrapper.Descriptor.Should().NotBeNull();
                    wrapper.Descriptor.Type.Should().Be(typeof(TestOrchestration));
                    wrapper.InnerOrchestration.Should().BeNull();
                });

        [Fact]
        public Task Execute_InvalidOperation()
            => RunTestExceptionAsync<InvalidOperationException>(
                wrapper => wrapper.Execute(s_orchestrationContext, Input));

        [Theory]
        [InlineData(typeof(TestOrchestration))]
        [InlineData(typeof(TestOrchestrationOfT))]
        public Task Execute_InvokesInner(Type type)
            => RunTestAsync(
                type,
                wrapper =>
                {
                    wrapper.Initialize(CreateServiceProvider());
                    return wrapper.Execute(s_orchestrationContext, $"\"{Input}\"");
                },
                (wrapper, result) =>
                {
                    result.Should().BeOneOf(Input, $"\"{Input}\"");
                    InvokedContext.Should().BeOfType<WrapperOrchestrationContext>();
                    InvokedInput.Should().BeOneOf(Input, $"\"{Input}\"");
                });

        [Fact]
        public void GetStatus_InvalidOperation()
            => RunTestException<InvalidOperationException>(
                wrapper => wrapper.GetStatus());

        [Theory]
        [InlineData(typeof(TestOrchestration))]
        [InlineData(typeof(TestOrchestrationOfT))]
        public void GetStatus_InvokesInner(Type type)
            => RunTest(
                type,
                wrapper =>
                {
                    wrapper.Initialize(CreateServiceProvider());
                    return wrapper.GetStatus();
                },
                (wrapper, result) =>
                {
                    result.Should().BeOneOf(type.Name, $"\"{type.Name}\"");
                });

        [Fact]
        public void RaiseEvent_InvalidOperation()
            => RunTestException<InvalidOperationException>(
                wrapper => wrapper.RaiseEvent(s_orchestrationContext, Event, Input));

        [Theory]
        [InlineData(typeof(TestOrchestration))]
        [InlineData(typeof(TestOrchestrationOfT))]
        public void RaiseEvent_InvokesInner(Type type)
            => RunTest(
                type,
                wrapper =>
                {
                    wrapper.Initialize(CreateServiceProvider());
                    wrapper.RaiseEvent(s_orchestrationContext, Event, $"\"{Input}\"");
                    return true;
                },
                (wrapper, _) =>
                {
                    EventRaised.Should().Be(Event);
                    InvokedContext.Should().BeOfType<WrapperOrchestrationContext>();
                    InvokedInput.Should().BeOneOf(Input, $"\"{Input}\"");
                });

        private static void RunTestException<TException>(Action<WrapperOrchestration> act)
            where TException : Exception
        {
            bool Act(WrapperOrchestration wrapper)
            {
                act(wrapper);
                return true;
            }

            TException exception = Capture<TException>(
                () => RunTest(typeof(TestOrchestration), Act, null));
            exception.Should().NotBeNull();
        }

        private static async Task RunTestExceptionAsync<TException>(Func<WrapperOrchestration, Task> act)
            where TException : Exception
        {
            async Task<bool> Act(WrapperOrchestration wrapper)
            {
                await act(wrapper);
                return true;
            }

            TException exception = await Capture<TException>(
                () => RunTestAsync(typeof(TestOrchestration), Act, null));
            exception.Should().NotBeNull();
        }

        private static void RunTest<TResult>(
            Type innerType,
            Func<WrapperOrchestration, TResult> act,
            Action<WrapperOrchestration, TResult> verify)
        {
            var services = new WrapperOrchestration(new TaskOrchestrationDescriptor(innerType));
            TResult result = act(services);
            verify?.Invoke(services, result);
        }

        private static async Task RunTestAsync<TResult>(
            Type innerType,
            Func<WrapperOrchestration, Task<TResult>> act,
            Action<WrapperOrchestration, TResult> verify)
        {
            var services = new WrapperOrchestration(new TaskOrchestrationDescriptor(innerType));
            TResult result = await act(services);
            verify?.Invoke(services, result);
        }

        private IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(this);
            return services.BuildServiceProvider();
        }

        private class TestOrchestration : TaskOrchestration
        {
            private readonly WrapperOrchestrationTests _test;

            public TestOrchestration(WrapperOrchestrationTests test)
            {
                _test = test;
            }

            public override Task<string> Execute(OrchestrationContext context, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                return Task.FromResult(input);
            }

            public override string GetStatus()
            {
                return nameof(TestOrchestration);
            }

            public override void RaiseEvent(OrchestrationContext context, string name, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                _test.EventRaised = name;
            }
        }

        private class TestOrchestrationOfT : TaskOrchestration<string, string>
        {
            private readonly WrapperOrchestrationTests _test;

            public TestOrchestrationOfT(WrapperOrchestrationTests test)
            {
                _test = test;
            }

            public override Task<string> RunTask(OrchestrationContext context, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                return Task.FromResult(input);
            }

            public override void OnEvent(OrchestrationContext context, string name, string input)
            {
                _test.InvokedContext = context;
                _test.InvokedInput = input;
                _test.EventRaised = name;
            }

            public override string OnGetStatus()
            {
                return nameof(TestOrchestrationOfT);
            }
        }

        private class TestOrchestrationContext : OrchestrationContext
        {
            public TestOrchestrationContext(OrchestrationInstance instance)
            {
                OrchestrationInstance = instance;
            }

            public override void ContinueAsNew(object input)
            {
                throw new NotImplementedException();
            }

            public override void ContinueAsNew(string newVersion, object input)
            {
                throw new NotImplementedException();
            }

            public override Task<T> CreateSubOrchestrationInstance<T>(string name, string version, object input)
            {
                throw new NotImplementedException();
            }

            public override Task<T> CreateSubOrchestrationInstance<T>(string name, string version, string instanceId, object input)
            {
                throw new NotImplementedException();
            }

            public override Task<T> CreateSubOrchestrationInstance<T>(string name, string version, string instanceId, object input, IDictionary<string, string> tags)
            {
                throw new NotImplementedException();
            }

            public override Task<T> CreateTimer<T>(DateTime fireAt, T state)
            {
                throw new NotImplementedException();
            }

            public override Task<T> CreateTimer<T>(DateTime fireAt, T state, CancellationToken cancelToken)
            {
                throw new NotImplementedException();
            }

            public override Task<TResult> ScheduleTask<TResult>(string name, string version, params object[] parameters)
            {
                throw new NotImplementedException();
            }

            public override void SendEvent(OrchestrationInstance orchestrationInstance, string eventName, object eventData)
            {
                throw new NotImplementedException();
            }
        }
    }
}
