// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A wrapper for <see cref="ServiceDescriptor"/> to track the concrete implementation type.
    /// </summary>
    public abstract class ServiceDescriptorWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptorWrapper"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="descriptor">The service descriptor.</param>
        protected ServiceDescriptorWrapper(Type type, ServiceDescriptor descriptor)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(descriptor, nameof(descriptor));

            Type = type;
            Descriptor = descriptor;
        }

        /// <summary>
        /// Gets the implementation type. Never null.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the service descriptor being wrapped.
        /// </summary>
        internal ServiceDescriptor Descriptor { get; }
    }
}
