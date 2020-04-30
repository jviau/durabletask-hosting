// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Holds the <see cref="IServiceScope"/> per orchestration instance execution.
    /// </summary>
    internal class OrchestrationScope : IOrchestrationScope
    {
        private static readonly IDictionary<string, IOrchestrationScope> s_scopes
            = new Dictionary<string, IOrchestrationScope>();

        private readonly IServiceScope _innerScope;
        private readonly string _scopeId;
        private int _references = 0;

        private OrchestrationScope(IServiceScope scope, string scopeId)
        {
            _innerScope = Check.NotNull(scope, nameof(scope));
            _scopeId = Check.NotNull(scopeId, nameof(scopeId));
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => _innerScope.ServiceProvider;

        /// <summary>
        /// Enters the scope identified by <paramref name="orchestrationInstanceId"/>.
        /// </summary>
        /// <param name="orchestrationInstanceId">The orchestration instance id. Not null or empty.</param>
        /// <returns>A disposalable that will exit the scope when disposed.</returns>
        public static IDisposable EnterScope(string orchestrationInstanceId)
        {
            IOrchestrationScope scope = GetScope(orchestrationInstanceId);
            return scope?.Enter() ?? EmptyDisposable.Instance;
        }

        /// <summary>
        /// Gets the current scope for the orchestration instance. Throws if not found.
        /// </summary>
        /// <param name="orchestrationInstanceId">The orchestration instance id. Not null or empty.</param>
        /// <returns>A non-null <see cref="IOrchestrationScope"/>.</returns>
        public static IOrchestrationScope GetScope(string orchestrationInstanceId)
        {
            Check.NotNullOrEmpty(orchestrationInstanceId, nameof(orchestrationInstanceId));
            lock (s_scopes)
            {
                return s_scopes[orchestrationInstanceId];
            }
        }

        /// <summary>
        /// Gets or creates a new <see cref="IOrchestrationScope"/> for the orchestration instance.
        /// </summary>
        /// <param name="orchestrationInstanceId">The orchestration instance id. Not null or empty.</param>
        /// <param name="serviceProvider">The service provider. Not null.</param>
        /// <returns>The newly created scope.</returns>
        public static IOrchestrationScope GetOrCreateScope(
            string orchestrationInstanceId, IServiceProvider serviceProvider)
        {
            Check.NotNullOrEmpty(orchestrationInstanceId, nameof(orchestrationInstanceId));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            lock (s_scopes)
            {
                return s_scopes.ContainsKey(orchestrationInstanceId)
                    ? GetScope(orchestrationInstanceId)
                    : CreateScope(orchestrationInstanceId, serviceProvider);
            }
        }

        /// <summary>
        /// Creates a new <see cref="IOrchestrationScope"/> for the orchestration instance.
        /// </summary>
        /// <param name="orchestrationInstanceId">The orchestration instance id. Not null or empty.</param>
        /// <param name="serviceProvider">The service provider. Not null.</param>
        /// <returns>The newly created scope.</returns>
        public static IOrchestrationScope CreateScope(
            string orchestrationInstanceId, IServiceProvider serviceProvider)
        {
            Check.NotNullOrEmpty(orchestrationInstanceId, nameof(orchestrationInstanceId));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            lock (s_scopes)
            {
                if (s_scopes.ContainsKey(orchestrationInstanceId))
                {
                    throw new InvalidOperationException(Strings.ScopeAlreadyExists(orchestrationInstanceId));
                }

#pragma warning disable CA2000 // Dispose objects before losing scope
                IOrchestrationScope scope = new OrchestrationScope(serviceProvider.CreateScope(), orchestrationInstanceId);
                s_scopes[orchestrationInstanceId] = scope;
                return scope;
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }

        /// <inheritdoc />
        public IDisposable Enter() => new ScopeRef(this);

        /// <inheritdoc />
        public void Dispose() => _innerScope.Dispose();

        private void Increment()
        {
            lock (s_scopes)
            {
                _references++;
            }
        }

        private void Decrement()
        {
            bool dispose = false;
            lock (s_scopes)
            {
                _references--;
                if (_references == 0)
                {
                    dispose = true;
                    s_scopes.Remove(_scopeId);
                }
            }

            if (dispose)
            {
                Dispose();
            }
        }

        private readonly struct ScopeRef : IDisposable
        {
            private readonly OrchestrationScope _scope;

            public ScopeRef(OrchestrationScope scope)
            {
                _scope = scope;
                scope.Increment();
            }

            /// <inheritdoc />
            public void Dispose()
            {
                _scope.Decrement();
            }
        }

        private class EmptyDisposable : IDisposable
        {
            public static IDisposable Instance { get; } = new EmptyDisposable();

            public void Dispose()
            {
                // no op
            }
        }
    }
}
