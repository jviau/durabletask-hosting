// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using DurableTask.DependencyInjection.Properties;

namespace DurableTask
{
    /// <summary>
    /// Helpers for assertions.
    /// </summary>
    internal static class Check
    {
        /// <summary>
        /// Checks in the provided element is null, throwing if it is.
        /// Throws <see cref="ArgumentException" /> if the conditions are not met.
        /// </summary>
        /// <param name="t">The element to check.</param>
        /// <param name="name">The name of the element for the exception.</param>
        /// <typeparam name="T">The type of element to check.</typeparam>
        /// <returns>The original element.</returns>
        public static T NotNull<T>([ValidatedNotNull]T t, string name)
        {
            if (t is null)
            {
                throw new ArgumentNullException(name);
            }

            return t;
        }

        /// <summary>
        /// Checks in the provided string is null, throwing if it is.
        /// Throws <see cref="ArgumentException" /> if the conditions are not met.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="name">The name of the string for the exception.</param>
        /// <returns>The original string.</returns>
        public static string NotNullOrEmpty([ValidatedNotNull]string value, string name)
        {
            if (value is null)
            {
                throw new ArgumentNullException(name);
            }

            if (value.Length == 0 || value[0] == '\0')
            {
                throw new ArgumentException(Strings.StringEmpty(name), name);
            }

            return value;
        }

        /// <summary>
        /// Checks if the supplied type is a concrete non-abstract type and implements the provided generic type.
        /// Throws <see cref="ArgumentException" /> if the conditions are not met.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="name">The name of the argument for the exception message.</param>
        /// <typeparam name="TImplements">The type <paramref name="type" /> must implement.</typeparam>
        public static void ConcreteType<TImplements>(Type type, string name)
        {
            NotNull(type, name);
            if (!typeof(TImplements).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract)
            {
                throw new ArgumentException(
                    Strings.InvalidType(name, type, typeof(TImplements)), name);
            }
        }
    }
}
