// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Activitys.Tests
{
    public class TaskActivityDescriptorTests
    {
        [Fact]
        public void Ctor_NullType_Throws()
        {
            Action act = () => new TaskActivityDescriptor((Type)null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullMethodInfo_Throws()
        {
            Action act = () => new TaskActivityDescriptor((MethodInfo)null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(TaskActivity))]
        public void Ctor_InvalidType_Throws(Type type)
        {
            Action act = () => new TaskActivityDescriptor(type);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(typeof(TestActivity))]
        [InlineData(typeof(TestActivity<>))]
        [InlineData(typeof(TestActivity<object>))]
        public void Ctor_Type_DefaultNameVersion(Type type)
        {
            var descriptor = new TaskActivityDescriptor(type);
            descriptor.Should().NotBeNull();
            descriptor.Method.Should().BeNull();
            descriptor.Type.Should().Be(type);
            descriptor.Name.Should().Be(type.FullName);
            descriptor.Version.Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(TestActivity))]
        [InlineData(typeof(TestActivity<>))]
        [InlineData(typeof(TestActivity<object>))]
        public void Ctor_Type_SuppliedNameVersion(Type type)
        {
            const string name = "CustomName";
            const string version = "CustomVersion";
            var descriptor = new TaskActivityDescriptor(type, name, version);
            descriptor.Should().NotBeNull();
            descriptor.Method.Should().BeNull();
            descriptor.Type.Should().Be(type);
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
        }

        [Fact]
        public void Ctor_MethodInfo_DefaultNameVersion()
        {
            var methodInfo = typeof(IMyServce).GetMethod(nameof(IMyServce.SomethingAsync));
            var descriptor = new TaskActivityDescriptor(methodInfo);
            descriptor.Type.Should().BeNull();
            descriptor.Method.Should().BeSameAs(methodInfo);
            descriptor.Name.Should().Be(NameVersionHelper.GetDefaultName(methodInfo));
            descriptor.Version.Should().Be(NameVersionHelper.GetDefaultVersion(methodInfo));
        }

        [Fact]
        public void Ctor_MethodInfo_SuppliedNameVersion()
        {
            const string name = "CustomName";
            const string version = "CustomVersion";
            var methodInfo = typeof(IMyServce).GetMethod(nameof(IMyServce.SomethingAsync));
            var descriptor = new TaskActivityDescriptor(methodInfo, name, version);
            descriptor.Type.Should().BeNull();
            descriptor.Method.Should().BeSameAs(methodInfo);
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
        }

        [Fact]
        public void Create_ConcreteType_Succeeds()
        {
            var descriptor = TaskActivityDescriptor.Create<TestActivity>();
            descriptor.Should().NotBeNull();
            descriptor.Method.Should().BeNull();
            descriptor.Type.Should().Be(typeof(TestActivity));
            descriptor.Name.Should().Be(typeof(TestActivity).FullName);
            descriptor.Version.Should().BeEmpty();
        }

        [Fact]
        public void Create_SuppliedNameVersion()
        {
            const string name = "CustomName";
            const string version = "CustomVersion";
            var descriptor = TaskActivityDescriptor.Create<TestActivity>(name, version);
            descriptor.Should().NotBeNull();
            descriptor.Method.Should().BeNull();
            descriptor.Type.Should().Be(typeof(TestActivity));
            descriptor.Name.Should().Be(name);
            descriptor.Version.Should().Be(version);
        }

        [Fact]
        public void Create_AbstractType_Fails()
        {
            Action act = () => TaskActivityDescriptor.Create<TaskActivity>();
            act.Should().ThrowExactly<ArgumentException>();
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

        private interface IMyServce
        {
            Task<string> SomethingAsync();
        }
    }
}
