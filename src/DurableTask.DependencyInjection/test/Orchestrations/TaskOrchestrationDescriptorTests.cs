// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Orchestrations.Tests
{
    public class TaskOrchestrationDescriptorTests
    {
        [Fact]
        public void Ctor_NullType_Throws()
        {
            Action act = () => new TaskOrchestrationDescriptor(null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(TaskOrchestration))]
        public void Ctor_InvalidType_Throws(Type type)
        {
            Action act = () => new TaskOrchestrationDescriptor(type);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(typeof(TestOrchestration))]
        [InlineData(typeof(TestOrchestration<>))]
        [InlineData(typeof(TestOrchestration<object>))]
        public void Ctor_DefaultNameVersion(Type type)
        {
            var descriptor = new TaskOrchestrationDescriptor(type);
            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(type);
            descriptor.Name.Should().Be(TypeShortName.ToString(type, false));
            descriptor.Version.Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(TestOrchestration))]
        [InlineData(typeof(TestOrchestration<>))]
        [InlineData(typeof(TestOrchestration<object>))]
        public void Ctor_SuppliedNameVersion(Type type)
        {
            const string name = "CustomName";
            const string version = "CustomVersion";
            var descriptor = new TaskOrchestrationDescriptor(type, name, version);
            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(type);
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
        }

        [Fact]
        public void Create_ConcreteType_Succeeds()
        {
            var descriptor = TaskOrchestrationDescriptor.Create<TestOrchestration>();
            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(typeof(TestOrchestration));
            descriptor.Name.Should().Be(typeof(TestOrchestration).FullName);
            descriptor.Version.Should().BeEmpty();
        }

        [Fact]
        public void Create_SuppliedNameVersion()
        {
            const string name = "CustomName";
            const string version = "CustomVersion";
            var descriptor = TaskOrchestrationDescriptor.Create<TestOrchestration>(name, version);
            descriptor.Should().NotBeNull();
            descriptor.Type.Should().Be(typeof(TestOrchestration));
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
        }

        [Fact]
        public void Create_AbstractType_Fails()
        {
            Action act = () => TaskOrchestrationDescriptor.Create<TaskOrchestration>();
            act.Should().ThrowExactly<ArgumentException>();
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

        private class TestOrchestration<T> : TestOrchestration
        {
        }
    }
}
