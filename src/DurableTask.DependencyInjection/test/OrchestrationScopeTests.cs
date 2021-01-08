// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests
{
    public class OrchestrationScopeTests
    {
        [Fact]
        public void GetScope_ArgumentNull()
        {
            // arrange, act
            var ex = Capture<ArgumentNullException>(
                () => OrchestrationScope.GetScope(null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void GetScope_KeyNotFound()
        {
            // arrange, act
            var ex = Capture<KeyNotFoundException>(
                () => OrchestrationScope.GetScope(Guid.NewGuid().ToString()));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void GetScope_ScopeFound()
        {
            // arrange
            string instanceId = Guid.NewGuid().ToString();
            OrchestrationScope.CreateScope(instanceId, GetServiceProvider());

            // act
            IOrchestrationScope scope = OrchestrationScope.GetScope(instanceId);

            // assert
            scope.Should().NotBeNull();
            scope.ServiceProvider.Should().NotBeNull();
        }

        [Fact]
        public void CreateScope_ArgumentNullInstance()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => OrchestrationScope.CreateScope(null, GetServiceProvider()));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CreateScope_ArgumentNullServiceProvider()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => OrchestrationScope.CreateScope(Guid.NewGuid().ToString(), null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CreateScope_AlreadyExists()
        {
            // arrange
            string instanceId = Guid.NewGuid().ToString();
            OrchestrationScope.CreateScope(instanceId, GetServiceProvider());

            // act
            InvalidOperationException ex = Capture<InvalidOperationException>(
                () => OrchestrationScope.CreateScope(instanceId, GetServiceProvider()));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CreateScope_Created()
        {
            // arrange
            string instanceId = Guid.NewGuid().ToString();

            // act
            IOrchestrationScope scope = OrchestrationScope.CreateScope(instanceId, GetServiceProvider());

            // assert
            scope.Should().NotBeNull();
            scope.ServiceProvider.Should().NotBeNull();
        }

        [Fact]
        public void GetOrCreateScope_Created()
        {
            // arrange
            string instanceId = Guid.NewGuid().ToString();

            // act
            IOrchestrationScope scope = OrchestrationScope.GetOrCreateScope(instanceId, GetServiceProvider());

            // assert
            scope.Should().NotBeNull();
            scope.ServiceProvider.Should().NotBeNull();
        }

        [Fact]
        public void GetOrCreateScope_Found()
        {
            // arrange
            string instanceId = Guid.NewGuid().ToString();

            // act
            IOrchestrationScope first = OrchestrationScope.CreateScope(instanceId, GetServiceProvider());
            IOrchestrationScope second = OrchestrationScope.GetOrCreateScope(instanceId, GetServiceProvider());

            // assert
            second.Should().NotBeNull();
            second.ServiceProvider.Should().NotBeNull();
            second.Should().BeSameAs(first);
        }

        private static IServiceProvider GetServiceProvider()
        {
            IServiceScope scope = Mock.Of<IServiceScope>(
                m => m.ServiceProvider == Mock.Of<IServiceProvider>());
            IServiceScopeFactory factory = Mock.Of<IServiceScopeFactory>(
                m => m.CreateScope() == scope);
            IServiceProvider serviceProvider = Mock.Of<IServiceProvider>(
                m => m.GetService(typeof(IServiceScopeFactory)) == factory);

            return serviceProvider;
        }
    }
}
