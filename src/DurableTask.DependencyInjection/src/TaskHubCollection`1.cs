// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Contains all task hub instances of the given type.
    /// </summary>
    /// <typeparam name="TDescribed">The described type in this collection.</typeparam>
    internal class TaskHubCollection<TDescribed> : ITaskObjectCollection<TDescribed>
    {
        private readonly HashSet<NamedTypeDescriptor<TDescribed>> _descriptors
            = new HashSet<NamedTypeDescriptor<TDescribed>>();

        private readonly ConcurrentDictionary<TaskVersion, Type> _typeMap
            = new ConcurrentDictionary<TaskVersion, Type>();

        /// <inheritdoc />
        public int Count => _descriptors.Count;

        /// <inheritdoc />
        public Type this[string taskName, string taskVersion]
            => GetTaskType(taskName, taskVersion);

        /// <inheritdoc />
        public IEnumerator<NamedTypeDescriptor<TDescribed>> GetEnumerator()
            => _descriptors.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Adds the descriptor to this collection.
        /// </summary>
        /// <param name="descriptor">The descriptor to add.</param>
        /// <returns>True if descriptor added, false otherwise.</returns>
        public bool Add(NamedTypeDescriptor<TDescribed> descriptor)
            => _descriptors.Add(descriptor);

        private bool IsTaskMatch(
            string name, string version, NamedTypeDescriptor<TDescribed> descriptor)
        {
            return string.Equals(name, descriptor.Name, StringComparison.Ordinal)
                && string.Equals(version, descriptor.Version, StringComparison.Ordinal);
        }

        private Type GetTaskType(string name, string version)
        {
            var taskVersion = new TaskVersion(name, version);
            if (_typeMap.TryGetValue(taskVersion, out Type type))
            {
                return type;
            }

            foreach (NamedTypeDescriptor<TDescribed> descriptor in _descriptors)
            {
                if (IsTaskMatch(name, version, descriptor))
                {
                    _typeMap.TryAdd(taskVersion, descriptor.Type);
                    return descriptor.Type;
                }
            }

            return null;
        }

        private readonly struct TaskVersion : IEquatable<TaskVersion>
        {
            public TaskVersion(string name, string version)
            {
                Name = name;
                Version = version;
            }

            public string Name { get; }

            public string Version { get; }

            public bool Equals(TaskVersion other)
            {
                return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (obj is TaskVersion other)
                {
                    return Equals(other);
                }

                return false;
            }

            public override int GetHashCode()
            {
                string n = Name ?? string.Empty;
                string v = Version ?? string.Empty;

                return n.ToUpper().GetHashCode() ^ v.ToUpper().GetHashCode();
            }
        }
    }
}
