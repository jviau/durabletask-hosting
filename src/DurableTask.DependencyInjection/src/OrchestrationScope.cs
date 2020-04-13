// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Holds the <see cref="IServiceScope"/> per orchestration instance execution.
    /// </summary>
    internal class OrchestrationScope : IOrchestrationScope
    {
        private static readonly IDictionary<OrchestrationInstance, IOrchestrationScope> s_scopes
            = new Dictionary<OrchestrationInstance, IOrchestrationScope>();

        private readonly IServiceScope _innerScope;
        private readonly TaskCompletionSource<bool> _middlewareCompleted
            = new TaskCompletionSource<bool>();

        private OrchestrationScope(IServiceScope scope)
        {
            _innerScope = Check.NotNull(scope, nameof(scope));
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => _innerScope.ServiceProvider;

        /// <summary>
        /// Gets the current scope for the orchestration instance. Throws if not found.
        /// </summary>
        /// <param name="orchestrationInstance">The orchestration instance. Not null.</param>
        /// <returns>A non-null <see cref="IOrchestrationScope"/>.</returns>
        public static IOrchestrationScope GetScope(OrchestrationInstance orchestrationInstance)
        {
            Check.NotNull(orchestrationInstance, nameof(orchestrationInstance));
            lock (s_scopes)
            {
                return s_scopes[orchestrationInstance];
            }
        }

        /// <summary>
        /// Creates a new <see cref="IOrchestrationScope"/> for the orchestration instance.
        /// </summary>
        /// <param name="orchestrationInstance">The orchestration instance. Not null.</param>
        /// <param name="serviceProvider">The service provider. Not null.</param>
        /// <returns>The newly created scope.</returns>
        public static IOrchestrationScope CreateScope(
            OrchestrationInstance orchestrationInstance, IServiceProvider serviceProvider)
        {
            Check.NotNull(orchestrationInstance, nameof(orchestrationInstance));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            lock (s_scopes)
            {
                if (s_scopes.ContainsKey(orchestrationInstance))
                {
                    throw new InvalidOperationException(
                        $"Scope already exists for orchestration {orchestrationInstance.InstanceId}");
                }

                IOrchestrationScope scope = new OrchestrationScope(serviceProvider.CreateScope());
                s_scopes[orchestrationInstance] = scope;
                return scope;
            }
        }

        /// <summary>
        /// Waits for middleware completion then disposes the <see cref="IOrchestrationScope"/>
        /// for the provided orchestration instance, if found.
        /// </summary>
        /// <param name="orchestrationInstance">The orchestration instance, not null.</param>
        /// <returns>A task that completes when the orchestration scope is disposed.</returns>
        public static async Task SafeDisposeScopeAsync(OrchestrationInstance orchestrationInstance)
        {
            Check.NotNull(orchestrationInstance, nameof(orchestrationInstance));

            IOrchestrationScope scope;
            lock (s_scopes)
            {
                if (s_scopes.TryGetValue(orchestrationInstance, out scope))
                {
                    s_scopes.Remove(orchestrationInstance);
                }
            }

            if (scope != null)
            {
                await scope.WaitForMiddlewareCompletionAsync().ConfigureAwait(false);
                scope.Dispose();
            }
        }

        /// <inheritdoc />
        public void SignalMiddlewareCompletion() => _middlewareCompleted.TrySetResult(true);

        /// <inheritdoc />
        public Task WaitForMiddlewareCompletionAsync() => _middlewareCompleted.Task;

        /// <inheritdoc />
        public void Dispose()
        {
            _innerScope.Dispose();

            if (!_middlewareCompleted.Task.IsCompleted)
            {
                _middlewareCompleted.TrySetCanceled();
            }
        }
    }
}
