// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.DependencyInjection.Activities
{
    /// <summary>
    /// Object creator driven by the provided descriptor.
    /// </summary>
    internal class ActivityObjectCreator : ObjectCreator<TaskActivity>
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
        public override TaskActivity Create() => new WrapperActivity(_descriptor);
    }
}
