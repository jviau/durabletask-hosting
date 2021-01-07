// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// Helpers for processing open and closed generic names.
    /// </summary>
    internal static class GenericNameHelper
    {
        /// <summary>
        /// For a name of "Namespace.MyType`N[SomeSubtype]" gets "Namespace.MyType`N".
        /// </summary>
        /// <param name="name">The type name to process.</param>
        /// <param name="genericName">The found generic name.</param>
        /// <returns>True if name represented a generic, false otherwise.</returns>
        public static bool TryGetGenericName(string name, out string genericName)
        {
            // For a name of "Namespace.MyType`N[SomeSubtype]" we want "Namespace.MyType`N"
            int genericSplitter = name.IndexOf('[');
            if (genericSplitter < 0)
            {
                genericName = null;
                return false;
            }

            genericName = name.Substring(0, genericSplitter);
            return true;
        }

        /// <summary>
        /// Gets the default name for a type.
        /// </summary>
        /// <param name="type">The type to get a name for.</param>
        /// <returns>The default name for this type.</returns>
        public static string GetDefaultName(Type type)
        {
            Check.NotNull(type, nameof(type));
            return type.FullName;
        }
    }
}
