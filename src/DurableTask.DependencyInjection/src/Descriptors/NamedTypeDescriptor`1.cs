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
        private string _name;
        private string _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedTypeDescriptor{TBase}"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        protected NamedTypeDescriptor(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets the task name.
        /// </summary>
        public string Name
        {
            get
            {
                Initialize();
                return _name;
            }

            protected set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the task version.
        /// </summary>
        public string Version
        {
            get
            {
                Initialize();
                return _version;
            }

            protected set
            {
                _version = value;
            }
        }

        private void Initialize()
        {
            if (_name == null)
            {
                _name = NameVersionHelper.GetDefaultName(Type);
            }

            if (_version == null)
            {
                _version = NameVersionHelper.GetDefaultVersion(Type);
            }
        }
    }
}
