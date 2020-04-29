// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;

namespace DurableTask.DependencyInjection.Orchestrations
{
    /// <summary>
    /// An orchestration that wraps the real orchestration type.
    /// </summary>
    internal class WrapperOrchestration : TaskOrchestration
    {
        private readonly TaskCompletionSource<object> _scopeDisposal
            = new TaskCompletionSource<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperOrchestration"/> class.
        /// </summary>
        /// <param name="innerOrchestrationType">The inner orchestration type to use.</param>
        public WrapperOrchestration(Type innerOrchestrationType)
        {
            Check.NotNull(innerOrchestrationType, nameof(innerOrchestrationType));
            Check.ConcreteType<TaskOrchestration>(innerOrchestrationType, nameof(innerOrchestrationType));

            InnerOrchestrationType = innerOrchestrationType;
        }

        /// <summary>
        /// Gets the inner orchestrations type.
        /// </summary>
        public Type InnerOrchestrationType { get; }

        /// <summary>
        /// Gets or sets the inner orchestration.
        /// </summary>
        public TaskOrchestration InnerOrchestration { get; set; }

        /// <summary>
        /// Gets the scope disposal task.
        /// </summary>
        public Task ScopeDisposal => _scopeDisposal.Task;

        /// <inheritdoc />
        public override Task<string> Execute(OrchestrationContext context, string input)
        {
            CheckInnerOrchestration();

            // Durable task is very sensitive to how this task returns.
            // Trying to perform addition awaits before returning will cause this
            // orchestration to never complete, so all additional work must be fire & forget.
            Task<string> inner = InnerOrchestration.Execute(context, input);

            Task.Run(async () =>
            {
                await inner.ConfigureAwait(false);
                await OrchestrationScope.SafeDisposeScopeAsync(context.OrchestrationInstance)
                    .ConfigureAwait(false);
                _scopeDisposal.TrySetResult(null);
            });

            return inner;
        }

        /// <inheritdoc />
        public override string GetStatus()
        {
            CheckInnerOrchestration();
            return InnerOrchestration.GetStatus();
        }

        /// <inheritdoc />
        public override void RaiseEvent(OrchestrationContext context, string name, string input)
        {
            CheckInnerOrchestration();
            InnerOrchestration.RaiseEvent(context, name, input);
        }

        private void CheckInnerOrchestration()
        {
            if (InnerOrchestration == null)
            {
                throw new InvalidOperationException(Strings.InnerOrchestrationNull);
            }
        }
    }
}
