// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.DependencyInjection;

/// <summary>
/// A attribute for naming a <see cref="TaskOrchestration"/> or <see cref="TaskActivity"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class TaskAliasAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAliasAttribute"/> class.
    /// </summary>
    public TaskAliasAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAliasAttribute"/> class.
    /// </summary>
    /// <param name="name">The name to use. Populated from type if null.</param>
    /// <param name="version">The version to use. Empty if not specified.</param>
    public TaskAliasAttribute(string name = null, string version = null)
    {
        Name = name;
        Version = version;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAliasAttribute"/> class.
    /// </summary>
    /// <param name="type">The type to get the default name and version from.</param>
    /// <param name="version">The version for this task.</param>
    public TaskAliasAttribute(Type type, string version = null)
    {
        Check.NotNull(type);
        Name = NameVersionHelper.GetDefaultName(type);
        Version = string.IsNullOrEmpty(version) ? NameVersionHelper.GetDefaultVersion(type) : version;
    }

    /// <summary>
    /// Gets or sets the name of the task hub entity.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the version of the task hub entity.
    /// </summary>
    public string Version { get; set; }
}
