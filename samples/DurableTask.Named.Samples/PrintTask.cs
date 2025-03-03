// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Named.Samples;

/// <summary>
/// An activity to print to the console.
/// </summary>
public class PrintTask : TaskActivity<string, string>
{
    private readonly IConsole _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintTask"/> class.
    /// </summary>
    /// <param name="console">The console to print to.</param>
    public PrintTask(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc />
    protected override string Execute(TaskContext context, string input)
    {
        _console.WriteLine(input);
        return string.Empty;
    }
}
