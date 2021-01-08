// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection.Activities
{
    /// <summary>
    /// Object creator driven by the provided descriptor.
    /// </summary>
    internal class ActivityObjectCreator : GenericObjectCreator<TaskActivity>
    {
        private readonly TaskActivityDescriptor _descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityObjectCreator"/> class.
        /// </summary>
        /// <param name="descriptor">The activity descriptor. Not null.</param>
        public ActivityObjectCreator(TaskActivityDescriptor descriptor)
        {
            Check.NotNull(descriptor, nameof(descriptor));
            Name = descriptor.Name;
            Version = descriptor.Version;

            _descriptor = descriptor;
        }

        /// <inheritdoc/>
        public override TaskActivity Create()
        {
            if (_descriptor.Type?.IsGenericTypeDefinition == true)
            {
                throw new InvalidOperationException("Cannot create activity for generic definition");
            }

            return new WrapperActivity(_descriptor);
        }

        /// <inheritdoc/>
        public override TaskActivity Create(string closedName)
        {
            Check.NotNullOrEmpty(closedName, nameof(closedName));
            if (_descriptor.Type?.IsGenericTypeDefinition != true)
            {
                throw new InvalidOperationException("This is not a generic type definition descriptor.");
            }

            Type closedType = _descriptor.Type.Assembly.GetType(closedName, throwOnError: true);
            Check.Argument(closedType.IsConstructedGenericType, nameof(closedType), "Type must be closed");

            return new WrapperActivity(new TaskActivityDescriptor(closedType));
        }
    }
}
