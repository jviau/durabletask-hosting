// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;

namespace DurableTask.DependencyInjection
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
        public static T NotNull<T>(T t, string name)
        {
            if (t == null)
            {
                throw new ArgumentNullException(name);
            }

            return t;
        }

        /// <summary>
        /// Checks if the supplied type is a concrete non-abstract type and implements the provided generic type.
        /// Throws <see cref="ArgumentException" /> if the conditions are not met.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="argument">The name of the argument for the exception message.</param>
        /// <typeparam name="TImplements">The type <paramref name="type" /> must implement.</typeparam>
        public static void ConcreteType<TImplements>(Type type, string argument)
        {
            NotNull(type, argument);
            if (!typeof(TImplements).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract)
            {
                throw new ArgumentException(
                    $"Type parameter {argument} [{type}] must inherit from {typeof(TImplements)}, be a class, and not be abstract");
            }
        }
    }
}
