// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Activities.Tests
{
    public class ActivityObjectCreatorTests
    {
        [Fact]
        public void Ctor_NullDescriptor_Throws()
        {
            Action act = () => new ActivityObjectCreator(null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WrapperCreated()
        {
            var descriptor = TaskActivityDescriptor.Create<TestActivity>();
            var creator = new ActivityObjectCreator(descriptor);
            TaskActivity activity = creator.Create();
            activity.Should().NotBeNull();
            activity.Should().BeOfType<WrapperActivity>()
                .Which.Descriptor.Should().Be(descriptor);
        }

        [Fact]
        public void Create_GenericDef_Throws()
        {
            var descriptor = new TaskActivityDescriptor(typeof(TestActivity<>));
            var creator = new ActivityObjectCreator(descriptor);
            Action act = () => creator.Create();
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateWithName_NotGeneric_Throws()
        {
            var descriptor = TaskActivityDescriptor.Create<TestActivity>();
            var creator = new ActivityObjectCreator(descriptor);
            Action act = () => creator.Create("Some name");
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateWithName_GenericDef_ArgNull_Throws()
        {
            var descriptor = new TaskActivityDescriptor(typeof(TestActivity<>));
            var creator = new ActivityObjectCreator(descriptor);
            Action act = () => creator.Create(null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithName_GenericDef_TypeIssue_Throws()
        {
            var descriptor = new TaskActivityDescriptor(typeof(TestActivity<>));
            var creator = new ActivityObjectCreator(descriptor);
            Action act = () => creator.Create("Invalid name");
            act.Should().ThrowExactly<TypeLoadException>();
        }

        [Fact]
        public void CreateWithName_GenericDef_WrapperCreated()
        {
            var descriptor = new TaskActivityDescriptor(typeof(TestActivity<>));
            var type = typeof(TestActivity<object>);
            var creator = new ActivityObjectCreator(descriptor);
            TaskActivity activity = creator.Create(type.FullName);
            activity.Should().NotBeNull();
            activity.Should().BeOfType<WrapperActivity>()
                .Which.Descriptor.Type.Should().Be(type);
        }

        private class TestActivity : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }

        private class TestActivity<T> : TestActivity
        {
        }
    }
}
