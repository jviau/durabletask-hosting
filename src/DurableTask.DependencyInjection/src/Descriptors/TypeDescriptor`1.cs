// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for a type, enforcing it implements the declared base.
    /// </summary>
    /// <typeparam name="TBase">The base type it must implement.</typeparam>
    public class TypeDescriptor<TBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescriptor{TBase}"/> class.
        /// </summary>
        /// <param name="type">The type for this descriptor.</param>
        public TypeDescriptor(Type type)
        {
            Check.NotNull(type, nameof(type));
            Check.ConcreteType<TBase>(type, nameof(type));
            Type = type;
        }

        /// <summary>
        /// Gets the type held by this descriptor.
        /// </summary>
        public Type Type { get; }
    }
}
