// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Samples.Greetings;

/// <summary>
/// A task for sending a greeting.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SendGreetingTask"/> class.
/// </remarks>
/// <param name="console">The console output helper.</param>
public sealed class SendGreetingTask(IConsole console) : AsyncTaskActivity<string, string>
{
    private readonly IConsole _console = console ?? throw new ArgumentNullException(nameof(console));

    /// <inheritdoc />
    protected override async Task<string> ExecuteAsync(TaskContext context, string user)
    {
        string message;
        if (!string.IsNullOrWhiteSpace(user) && user.Equals("TimedOut"))
        {
            message = "GetUser Timed out!!!";
            _console.WriteLine(message);
        }
        else
        {
            _console.WriteLine("Sending greetings to user: " + user + "...");
            await Task.Delay(5 * 1000);
            message = "Greeting sent to " + user;
            _console.WriteLine(message);
        }

        return message;
    }
}
