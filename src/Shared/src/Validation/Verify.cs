// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace DurableTask;

/// <summary>
/// Helpers for assertions.
/// </summary>
internal static class Verify
{
    /// <summary>
    /// Verify some argument is not null, throwing <see cref="InvalidOperationException" /> if it is.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="argument">The argument to verify.</param>
    /// <param name="message">The optional exception message.</param>
    /// <returns>The <paramref name="argument" /> parameter, unchanged.</returns>
    [return: NotNullIfNotNull("argument")]
    public static T NotNull<T>([NotNull] T argument, string? message = default)
        where T : class
    {
        if (argument is null)
        {
            throw message is null
                ? new InvalidOperationException()
                : new InvalidOperationException(message);
        }

        return argument;
    }
}
