// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Extensions.Tests
{
    public class AssemblyExtensionTests
    {
        [Fact]
        public void GetConcreteTypes_PublicOnly_Interface()
        {
            // arrage, act
            IEnumerable<Type> types = GetType().Assembly
                .GetConcreteTypes<ITestInterface>(includePrivate: false);

            // assert
            types.Should().HaveCount(1);
            types.Should().Contain(typeof(PublicTest));
        }

        [Fact]
        public void GetConcreteTypes_PublicOnly_AbstractBase()
        {
            // arrage, act
            IEnumerable<Type> types = GetType().Assembly
                .GetConcreteTypes<TestBase>(includePrivate: false);

            // assert
            types.Should().HaveCount(1);
            types.Should().Contain(typeof(PublicTest));
        }

        [Fact]
        public void GetConcreteTypes_IncludePrivate_Interface()
        {
            // arrage, act
            IEnumerable<Type> types = GetType().Assembly
                .GetConcreteTypes<ITestInterface>(includePrivate: true);

            // assert
            types.Should().HaveCount(4);
            types.Should().Contain(typeof(PublicTest));
            types.Should().Contain(typeof(InternalTest));
            types.Should().Contain(typeof(ProtectedTest));
            types.Should().Contain(typeof(PrivateTest));
        }

        [Fact]
        public void GetConcreteTypes_IncludePrivate_AbstractBase()
        {
            // arrage, act
            IEnumerable<Type> types = GetType().Assembly
                .GetConcreteTypes<TestBase>(includePrivate: true);

            // assert
            types.Should().HaveCount(4);
            types.Should().Contain(typeof(PublicTest));
            types.Should().Contain(typeof(InternalTest));
            types.Should().Contain(typeof(ProtectedTest));
            types.Should().Contain(typeof(PrivateTest));
        }

        public interface ITestInterface
        {
        }

        public abstract class TestBase
        {
        }

        public class PublicTest : TestBase, ITestInterface
        {
        }

        internal class InternalTest : TestBase, ITestInterface
        {
        }

        protected class ProtectedTest : TestBase, ITestInterface
        {
        }

        private class PrivateTest : TestBase, ITestInterface
        {
        }
    }
}
