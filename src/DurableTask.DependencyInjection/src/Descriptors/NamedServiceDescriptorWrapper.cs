// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Describes a task hub activity or orchestration.
    /// </summary>
    public abstract class NamedServiceDescriptorWrapper : ServiceDescriptorWrapper
    {
        private string _name;
        private string _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedServiceDescriptorWrapper"/> class.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="descriptor">The service descriptor.</param>
        protected NamedServiceDescriptorWrapper(Type type, ServiceDescriptor descriptor)
            : base(type, descriptor)
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
