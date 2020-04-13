//  ----------------------------------------------------------------------------------
//  Copyright Microsoft Corporation
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------

using System;
using DurableTask.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.DependencyInjection.Tests.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Extensions
{
    public class TaskHubWorkerBuilderActivityExtensionsTests
    {
        private const string Name = "TaskHubWorkerBuilderActivityExtensionsTests_Name";
        private const string Version = "TaskHubWorkerBuilderActivityExtensionsTests_Version";

        [Fact]
        public void AddActivityTypeNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(null, typeof(TestActivity)));

        [Fact]
        public void AddActivityTypeNullType()
            => RunTestException<ArgumentNullException>(
                builder => builder.AddActivity((Type)null));

        [Fact]
        public void AddActivityGenericNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<TestActivity>(null));

        [Fact]
        public void AddActivityFactoryNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(null, __ => new TestActivity()));

        [Fact]
        public void AddActivityTypeNamedNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(null, typeof(TestActivity), Name, Version));

        [Fact]
        public void AddActivityTypeNamedNullType()
            => RunTestException<ArgumentNullException>(
                builder => builder.AddActivity(null, Name, Version));

        [Fact]
        public void AddActivityGenericNamedNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity<TestActivity>(null, Name, Version));

        [Fact]
        public void AddActivityFactoryNamedNullBuilder()
            => RunTestException<ArgumentNullException>(
                _ => TaskHubWorkerBuilderActivityExtensions.AddActivity(null, __ => new TestActivity(), Name, Version));

        [Fact]
        public void AddActivityType()
            => RunTest(
                builder => builder.AddActivity(typeof(TestActivity)),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(IsDescriptor(typeof(TestActivity))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddActivityGeneric()
            => RunTest(
                builder => builder.AddActivity<TestActivity>(),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(IsDescriptor(typeof(TestActivity))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddActivityFactory()
            => RunTest(
                builder => builder.AddActivity(_ => new TestActivity()),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(IsDescriptor(typeof(TestActivity))), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddActivityTypeNamed()
            => RunTest(
                builder => builder.AddActivity(typeof(TestActivity), Name, Version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(IsDescriptor(typeof(TestActivity), Name, Version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddActivityGenericNamed()
            => RunTest(
                builder => builder.AddActivity<TestActivity>(Name, Version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(IsDescriptor(typeof(TestActivity), Name, Version)), Times.Once);
                    mock.VerifyNoOtherCalls();
                });

        [Fact]
        public void AddActivityFactoryNamed()
            => RunTest(
                builder => builder.AddActivity(_ => new TestActivity(), Name, Version),
                (mock, builder) =>
                {
                    builder.Should().NotBeNull();
                    builder.Should().BeSameAs(mock.Object);
                    mock.Verify(m => m.AddActivity(IsDescriptor(typeof(TestActivity), Name, Version)), Times.Once);
                    mock.VerifyNoOtherCalls();
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
            mock.Setup(m => m.AddActivity(It.IsAny<TaskActivityDescriptor>())).Returns(mock.Object);

            TResult result = act(mock.Object);
            verify?.Invoke(mock, result);
        }

        private class TestActivity : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }

        private TaskActivityDescriptor IsDescriptor(Type type)
            => Match.Create<TaskActivityDescriptor>(
                descriptor => descriptor.Type == type
                    && descriptor.Descriptor.Lifetime == ServiceLifetime.Transient);

        private TaskActivityDescriptor IsDescriptor(Type type, string name, string version)
            => Match.Create<TaskActivityDescriptor>(
                descriptor => descriptor.Name == name
                    && descriptor.Version == version
                    && descriptor.Type == type
                    && descriptor.Descriptor.Lifetime == ServiceLifetime.Transient);
    }
}
