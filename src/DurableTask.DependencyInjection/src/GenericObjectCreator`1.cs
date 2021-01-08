// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// A descriptor object creator.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    internal abstract class GenericObjectCreator<T> : ObjectCreator<T>
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/>, given the closed type name for T.
        /// </summary>
        /// <param name="closedName">The closed type name of T.</param>
        /// <returns>An instance of T.</returns>
        /// <remarks>
        /// This is called when this creator represents an open-generic version of T, with the provided type
        /// <paramref name="closedName"/> being the closed form to create.
        /// </remarks>
        public abstract T Create(string closedName);
    }
}
