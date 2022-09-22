// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace DurableTask.DependencyInjection.Extensions;

/// <summary>
/// Extensions for <see cref="Assembly"/>.
/// </summary>
internal static class AssemblyExtensions
{
    /// <summary>
    /// Gets all concrete types that implement/extend <typeparamref name="TBase"/>.
    /// </summary>
    /// <typeparam name="TBase">The base type or interface.</typeparam>
    /// <param name="assembly">The assembly to check.</param>
    /// <param name="includePrivate">Include private types.</param>
    /// <returns>An enumerable of types.</returns>
    public static IEnumerable<Type> GetConcreteTypes<TBase>(this Assembly assembly, bool includePrivate = false)
    {
        Check.NotNull(assembly, nameof(assembly));
        return assembly
            .GetTypes()
            .Where(t =>
                typeof(TBase).IsAssignableFrom(t)
                && t.IsClass
                && !t.IsAbstract
                && (includePrivate || t.IsVisible));
    }
}
