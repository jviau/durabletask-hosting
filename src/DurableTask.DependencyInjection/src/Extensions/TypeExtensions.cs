// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace DurableTask.DependencyInjection.Extensions;

/// <summary>
/// Extensions for <see cref="Type"/>.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Gets all task name and versions described on a type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>The enumerable of task aliases.</returns>
    public static IEnumerable<(string Name, string Version)> GetTaskAliases(this Type type)
    {
        Check.NotNull(type, nameof(type));
        foreach (TaskAliasAttribute alias in type.GetCustomAttributes<TaskAliasAttribute>())
        {
            yield return (alias.Name, alias.Version);
        }
    }
}
