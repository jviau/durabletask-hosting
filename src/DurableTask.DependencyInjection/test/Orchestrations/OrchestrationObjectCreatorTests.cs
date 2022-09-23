// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Orchestrations.Tests;

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
        OrchestrationObjectCreator creator = new(descriptor);
        TaskOrchestration Orchestration = creator.Create();
        Orchestration.Should().NotBeNull();
        Orchestration.Should().BeOfType<WrapperOrchestration>()
            .Which.Descriptor.Should().Be(descriptor);
    }

    [Fact]
    public void Create_GenericDef_Throws()
    {
        TaskOrchestrationDescriptor descriptor = new(typeof(TestOrchestration<>));
        OrchestrationObjectCreator creator = new(descriptor);
        Action act = () => creator.Create();
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void CreateWithName_NotGeneric_Throws()
    {
        var descriptor = TaskOrchestrationDescriptor.Create<TestOrchestration>();
        OrchestrationObjectCreator creator = new(descriptor);
        Action act = () => creator.Create(new TypeShortName(typeof(string)));
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void CreateWithName_GenericDef_ArgNull_Throws()
    {
        TaskOrchestrationDescriptor descriptor = new(typeof(TestOrchestration<>));
        OrchestrationObjectCreator creator = new(descriptor);
        Action act = () => creator.Create(default);
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CreateWithName_GenericDef_TypeIssue_Throws()
    {
        TaskOrchestrationDescriptor descriptor = new(typeof(TestOrchestration<>));
        OrchestrationObjectCreator creator = new(descriptor);
        Action act = () => creator.Create(new TypeShortName("Namespace.Dne"));
        act.Should().ThrowExactly<TypeLoadException>();
    }

    [Fact]
    public void CreateWithName_GenericDef_WrapperCreated()
    {
        TaskOrchestrationDescriptor descriptor = new(typeof(TestOrchestration<>));
        var type = typeof(TestOrchestration<object>);
        OrchestrationObjectCreator creator = new(descriptor);
        TaskOrchestration Orchestration = creator.Create(new TypeShortName(type));
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
