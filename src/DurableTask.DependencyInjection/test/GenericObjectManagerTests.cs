// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using DurableTask.DependencyInjection.Activities;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Tests
{
    public class GenericObjectManagerTests
    {
        private static readonly TaskActivityDescriptor s_descriptor = TaskActivityDescriptor.Create<TestActivity>();

        [Fact]
        public void Add_NotExists_Succeeds()
        {
            var manager = new GenericObjectManager<TaskActivity>();
            manager.Add(new ActivityObjectCreator(s_descriptor));
        }

        [Fact]
        public void Add_Exists_Throws()
        {
            var manager = new GenericObjectManager<TaskActivity>();
            manager.Add(new ActivityObjectCreator(s_descriptor));

            Action act = () => manager.Add(new ActivityObjectCreator(s_descriptor));
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Create_NotExists_Null()
        {
            var manager = new GenericObjectManager<TaskActivity>();
            TaskActivity activity = manager.GetObject("DNE", "");
            activity.Should().BeNull();
        }

        [Fact]
        public void Create_Exists_NotNull()
        {
            var manager = new GenericObjectManager<TaskActivity>();
            manager.Add(new ActivityObjectCreator(s_descriptor));
            TaskActivity activity = manager.GetObject(s_descriptor.Name, s_descriptor.Version);
            activity.Should().NotBeNull();
            activity.As<WrapperActivity>().Descriptor.Type.Should().Be(s_descriptor.Type);
        }

        [Theory]
        [InlineData(typeof(TestActivity<object>))]
        [InlineData(typeof(TestActivity<int>))]
        [InlineData(typeof(TestActivity<SomeClass>))]
        public void Create_OpenGeneric_NotNull(Type type)
        {
            var manager = new GenericObjectManager<TaskActivity>();
            manager.Add(new ActivityObjectCreator(new TaskActivityDescriptor(typeof(TestActivity<>))));
            TaskActivity activity = manager.GetObject(TypeShortName.ToString(type, false), string.Empty);
            activity.Should().NotBeNull();
            activity.As<WrapperActivity>().Descriptor.Type.Should().Be(type);
        }

        [Fact]
        public void Create_ClosedGeneric_NotNull()
        {
            var descriptor = TaskActivityDescriptor.Create<TestActivity<object>>();
            var manager = new GenericObjectManager<TaskActivity>();
            manager.Add(new ActivityObjectCreator(descriptor));
            TaskActivity activity = manager.GetObject(descriptor.Name, descriptor.Version);
            activity.Should().NotBeNull();
            activity.As<WrapperActivity>().Descriptor.Type.Should().Be(descriptor.Type);

            TaskActivity activity2 = manager.GetObject(typeof(TestActivity<int>).FullName, descriptor.Version);
            activity2.Should().BeNull();
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

        private class SomeClass
        {
        }
    }
}
