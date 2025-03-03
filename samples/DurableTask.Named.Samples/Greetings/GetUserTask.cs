// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Named.Samples.Greetings;

/// <summary>
/// A task activity for getting a username from console.
/// </summary>
public class GetUserTask : TaskActivity<string, string>
{
    private readonly IConsole _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserTask"/> class.
    /// </summary>
    /// <param name="console">The console output helper.</param>
    public GetUserTask(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc />
    protected override string Execute(TaskContext context, string input)
    {
        _console.WriteLine("Please enter your name:");
        return _console.ReadLine();
    }
}
