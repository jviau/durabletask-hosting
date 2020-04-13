// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core;

namespace DurableTask.DependencyInjection.Orchestrations
{
    /// <summary>
    /// An orchestration that wraps the real orchestration type.
    /// </summary>
    internal class WrapperOrchestration : TaskOrchestration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperOrchestration"/> class.
        /// </summary>
        /// <param name="innerOrchestrationType">The inner orchestration type to use.</param>
        public WrapperOrchestration(Type innerOrchestrationType)
        {
            InnerOrchestrationType = Check.NotNull(innerOrchestrationType, nameof(innerOrchestrationType));
        }

        /// <summary>
        /// Gets the inner orchestrations type.
        /// </summary>
        public Type InnerOrchestrationType { get; }

        /// <summary>
        /// Gets or sets the inner orchestration.
        /// </summary>
        public TaskOrchestration InnerOrchestration { get; set; }

        /// <inheritdoc />
        public override async Task<string> Execute(OrchestrationContext context, string input)
        {
            if (InnerOrchestration == null)
            {
                throw new InvalidOperationException($"{InnerOrchestration} not set!");
            }

            try
            {
                return await InnerOrchestration.Execute(context, input).ConfigureAwait(false);
            }
            finally
            {
                await OrchestrationScope.SafeDisposeScopeAsync(context.OrchestrationInstance)
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override string GetStatus()
            => InnerOrchestration.GetStatus();

        /// <inheritdoc />
        public override void RaiseEvent(OrchestrationContext context, string name, string input)
            => InnerOrchestration.RaiseEvent(context, name, input);
    }
}
