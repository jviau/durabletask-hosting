// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

namespace System;

/// <summary>
/// Abstraction for <see cref="Console"/>.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// <see cref="Console.ReadLine"/>.
    /// </summary>
    /// <returns>The line input from the console.</returns>
    string ReadLine();

    /// <summary>
    /// <see cref="Console.WriteLine()"/>.
    /// </summary>
    void WriteLine();

    /// <summary>
    /// <see cref="Console.WriteLine(string)"/>.
    /// </summary>
    /// <param name="line">The line to write.</param>
    void WriteLine(string line);
}
