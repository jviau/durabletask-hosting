// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// An object manager that enables open-generic creators.
    /// </summary>
    /// <typeparam name="T">The type managed by this.</typeparam>
    internal sealed class GenericObjectManager<T> : INameVersionObjectManager<T>
    {
        private readonly ConcurrentDictionary<string, ObjectCreator<T>> _creators
            = new ConcurrentDictionary<string, ObjectCreator<T>>();

        /// <inheritdoc/>
        public void Add(ObjectCreator<T> creator)
        {
            string key = GetKey(creator.Name, creator.Version);
            if (!_creators.TryAdd(key, creator))
            {
                throw new InvalidOperationException($"Key '{key}' already exists.");
            }
        }

        /// <inheritdoc/>
        public T GetObject(string name, string version)
        {
            (ObjectCreator<T> creator, bool isGenericType) = GetCreator(name, version);

            if (creator is null)
            {
                return default;
            }

            if (isGenericType && creator is GenericObjectCreator<T> genericCreator)
            {
                return genericCreator.Create(name);
            }

            return creator.Create();
        }

        private static string GetKey(string name, string version) => $"{name}|{version}";

        private (ObjectCreator<T> Creator, bool IsGenericType) GetCreator(string name, string version)
        {
            // First check if the name is registered directly.
            if (_creators.TryGetValue(GetKey(name, version), out ObjectCreator<T> creator))
            {
                return (creator, false);
            }

            // Then check if this represents a generic type, and find the generic definition name.
            if (GenericNameHelper.TryGetGenericName(name, out string genericName) &&
                _creators.TryGetValue(GetKey(genericName, version), out creator))
            {
                return (creator, true);
            }

            return (default, false);
        }
    }
}
