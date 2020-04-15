using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DurableTask.Core;
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
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => OrchestrationScope.GetScope(null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void GetScope_KeyNotFound()
        {
            // arrange, act
            KeyNotFoundException ex = Capture<KeyNotFoundException>(
                () => OrchestrationScope.GetScope(new OrchestrationInstance()));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void GetScope_ScopeFound()
        {
            // arrange
            var instance = new OrchestrationInstance();
            OrchestrationScope.CreateScope(instance, GetServiceProvider());

            // act
            IOrchestrationScope scope = OrchestrationScope.GetScope(instance);

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
                () => OrchestrationScope.CreateScope(new OrchestrationInstance(), null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CreateScope_AlreadyExists()
        {
            // arrange
            var instance = new OrchestrationInstance();
            OrchestrationScope.CreateScope(instance, GetServiceProvider());

            // act
            InvalidOperationException ex = Capture<InvalidOperationException>(
                () => OrchestrationScope.CreateScope(instance, GetServiceProvider()));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CreateScope_Created()
        {
            // arrange
            var instance = new OrchestrationInstance();

            // act
            IOrchestrationScope scope = OrchestrationScope.CreateScope(instance, GetServiceProvider());

            // assert
            scope.Should().NotBeNull();
            scope.ServiceProvider.Should().NotBeNull();
        }

        [Fact]
        public async Task SafeDisposeScopeAsync_ArgumentNull()
        {
            // arrange, act
            ArgumentNullException ex = await Capture<ArgumentNullException>(
                () => OrchestrationScope.SafeDisposeScopeAsync(null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public async Task SafeDisposeScopeAsync_WaitsForMiddleware()
        {
            // arrange
            var instance = new OrchestrationInstance();
            IOrchestrationScope scope = OrchestrationScope.CreateScope(instance, GetServiceProvider());

            // act
            Task task = OrchestrationScope.SafeDisposeScopeAsync(instance);
            bool isCompleted = task.IsCompleted;
            scope.SignalMiddlewareCompletion();
            await task;

            // assert
            isCompleted.Should().BeFalse();
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
