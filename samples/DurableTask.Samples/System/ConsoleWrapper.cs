// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

namespace System;

/// <summary>
/// Default implementation of <see cref="IConsole"/>.
/// </summary>
public class ConsoleWrapper : IConsole
{
    /// <inheritdoc />
    public string ReadLine() => Console.ReadLine();

    /// <inheritdoc />
    public void WriteLine() => Console.WriteLine();

    /// <inheritdoc />
    public void WriteLine(string line) => Console.WriteLine(line);
}
