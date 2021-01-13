// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection.Orchestrations
{
    /// <summary>
    /// Object creator satisfied from the service provider.
    /// </summary>
    internal class OrchestrationObjectCreator : GenericObjectCreator<TaskOrchestration>
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
        public override TaskOrchestration Create()
        {
            if (_descriptor.Type?.IsGenericTypeDefinition == true)
            {
                throw new InvalidOperationException("Cannot create activity for generic definition");
            }

            return new WrapperOrchestration(_descriptor);
        }

        /// <inheritdoc/>
        public override TaskOrchestration Create(TypeShortName typeName)
        {
            if (_descriptor.Type?.IsGenericTypeDefinition != true)
            {
                throw new InvalidOperationException("This is not a generic type definition creator.");
            }

            Type closedType = typeName.Load(_descriptor.Type.Assembly);
            Check.Argument(closedType.IsConstructedGenericType, nameof(closedType), "Type must be closed");

            return new WrapperOrchestration(new TaskOrchestrationDescriptor(closedType));
        }
    }
}
