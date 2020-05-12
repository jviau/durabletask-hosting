// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// An object manager that gets its items via the service provider.
    /// </summary>
    /// <typeparam name="TObject">The type of object to create.</typeparam>
    internal class WrapperObjectManager<TObject> : INameVersionObjectManager<TObject>
    {
        private readonly ITaskObjectCollection<TObject> _descriptors;
        private readonly Func<Type, TObject> _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperObjectManager{TObject}"/> class.
        /// </summary>
        /// <param name="descriptors">The descriptors of.</param>
        /// <param name="factory">The factory function for creating the wrapper.</param>
        public WrapperObjectManager(ITaskObjectCollection<TObject> descriptors, Func<Type, TObject> factory)
        {
            _descriptors = Check.NotNull(descriptors, nameof(descriptors));
            _factory = Check.NotNull(factory, nameof(factory));
        }

        /// <inheritdoc />
        public void Add(ObjectCreator<TObject> creator)
        {
            throw new NotSupportedException(Strings.AddToObjectManagerNotSupported);
        }

        /// <inheritdoc />
        public TObject GetObject(string name, string version)
        {
            Type type = _descriptors[name, version];

            if (type == null)
            {
                return default;
            }

            return _factory(type);
        }
    }
}
