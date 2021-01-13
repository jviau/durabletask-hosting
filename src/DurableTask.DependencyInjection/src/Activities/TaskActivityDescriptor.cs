// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor for <see cref="TaskActivity"/>.
    /// </summary>
    public sealed class TaskActivityDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskActivityDescriptor"/> class.
        /// This activity will have its type fetched from the <see cref="IServiceProvider"/> and executed.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="name">The name of the type.</param>
        /// <param name="version">The version of the type.</param>
        public TaskActivityDescriptor(Type type, string name = null, string version = null)
        {
            Check.NotNull(type, nameof(type));
            Check.ConcreteType<TaskActivity>(type, nameof(type));

            Type = type;
            Name = name ?? TypeShortName.ToString(type, false);
            Version = version ?? NameVersionHelper.GetDefaultVersion(type);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskActivityDescriptor"/> class.
        /// This activity will have its <see cref="MemberInfo.DeclaringType"/> fetched from the
        /// <see cref="IServiceProvider"/> and invoked on it.
        /// </summary>
        /// <param name="method">The method info.</param>
        /// <param name="name">The name of the type.</param>
        /// <param name="version">The version of the type.</param>
        public TaskActivityDescriptor(MethodInfo method, string name = null, string version = null)
        {
            Method = Check.NotNull(method, nameof(method));
            Check.NotNull(method.DeclaringType, nameof(method) + nameof(method.DeclaringType));

            Name = name ?? NameVersionHelper.GetDefaultName(method);
            Version = version ?? NameVersionHelper.GetDefaultVersion(method);
        }

        /// <summary>
        /// Gets the task name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the task version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the <see cref="TaskActivity"/> type to fetch and execute.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> to fetch and execute.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Creates a new descriptor for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The activity type to describe.</typeparam>
        /// <param name="name">The name of the activity. Optional.</param>
        /// <param name="version">The version of the activity. Optional.</param>
        /// <returns>A new descriptor.</returns>
        public static TaskActivityDescriptor Create<T>(string name = null, string version = null)
            where T : TaskActivity
            => new TaskActivityDescriptor(typeof(T), name, version);
    }
}
