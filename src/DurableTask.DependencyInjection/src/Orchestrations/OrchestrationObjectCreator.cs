// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.DependencyInjection.Orchestrations
{
    /// <summary>
    /// Object creator satisfied from the service provider.
    /// </summary>
    internal class OrchestrationObjectCreator : ObjectCreator<TaskOrchestration>
    {
        private readonly TaskOrchestrationDescriptor _descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationObjectCreator"/> class.
        /// </summary>
        /// <param name="descriptor">The orchestration descriptor. Not null.</param>
        public OrchestrationObjectCreator(TaskOrchestrationDescriptor descriptor)
        {
            Check.NotNull(descriptor, nameof(descriptor));
            Name = descriptor.Name;
            Version = descriptor.Version;

            _descriptor = descriptor;
        }

        /// <inheritdoc/>
        public override TaskOrchestration Create() => new WrapperOrchestration(_descriptor);
    }
}
