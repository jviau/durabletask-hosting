// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for a type that includes a name and version.
    /// </summary>
    /// <typeparam name="TBase">The base type for this descriptor.</typeparam>
    public class NamedTypeDescriptor<TBase> : TypeDescriptor<TBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedTypeDescriptor{TBase}"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        public NamedTypeDescriptor(Type type)
            : this(type, NameVersionHelper.GetDefaultName(type), NameVersionHelper.GetDefaultVersion(type))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedTypeDescriptor{TBase}"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="name">The name of the type.</param>
        /// <param name="version">The version of the type.</param>
        public NamedTypeDescriptor(Type type, string name, string version)
            : base(type)
        {
            Name = Check.NotNullOrEmpty(name, nameof(name));
            Version = Check.NotNull(version, nameof(version));
        }

        /// <summary>
        /// Gets the task name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the task version.
        /// </summary>
        public string Version { get; }
    }
}
