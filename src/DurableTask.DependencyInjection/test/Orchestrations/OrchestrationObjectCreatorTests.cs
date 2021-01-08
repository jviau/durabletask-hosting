// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Orchestrations.Tests
{
    public class OrchestrationObjectCreatorTests
    {
        [Fact]
        public void Ctor_NullDescriptor_Throws()
        {
            Action act = () => new OrchestrationObjectCreator(null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WrapperCreated()
        {
            var descriptor = TaskOrchestrationDescriptor.Create<TestOrchestration>();
            var creator = new OrchestrationObjectCreator(descriptor);
            TaskOrchestration Orchestration = creator.Create();
            Orchestration.Should().NotBeNull();
            Orchestration.Should().BeOfType<WrapperOrchestration>()
                .Which.Descriptor.Should().Be(descriptor);
        }

        [Fact]
        public void Create_GenericDef_Throws()
        {
            var descriptor = new TaskOrchestrationDescriptor(typeof(TestOrchestration<>));
            var creator = new OrchestrationObjectCreator(descriptor);
            Action act = () => creator.Create();
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateWithName_NotGeneric_Throws()
        {
            var descriptor = TaskOrchestrationDescriptor.Create<TestOrchestration>();
            var creator = new OrchestrationObjectCreator(descriptor);
            Action act = () => creator.Create("Some name");
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateWithName_GenericDef_ArgNull_Throws()
        {
            var descriptor = new TaskOrchestrationDescriptor(typeof(TestOrchestration<>));
            var creator = new OrchestrationObjectCreator(descriptor);
            Action act = () => creator.Create(null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithName_GenericDef_TypeIssue_Throws()
        {
            var descriptor = new TaskOrchestrationDescriptor(typeof(TestOrchestration<>));
            var creator = new OrchestrationObjectCreator(descriptor);
            Action act = () => creator.Create("Invalid name");
            act.Should().ThrowExactly<TypeLoadException>();
        }

        [Fact]
        public void CreateWithName_GenericDef_WrapperCreated()
        {
            var descriptor = new TaskOrchestrationDescriptor(typeof(TestOrchestration<>));
            var type = typeof(TestOrchestration<object>);
            var creator = new OrchestrationObjectCreator(descriptor);
            TaskOrchestration Orchestration = creator.Create(type.FullName);
            Orchestration.Should().NotBeNull();
            Orchestration.Should().BeOfType<WrapperOrchestration>()
                .Which.Descriptor.Type.Should().Be(type);
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
