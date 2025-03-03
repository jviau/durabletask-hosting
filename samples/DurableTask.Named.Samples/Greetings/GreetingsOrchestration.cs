// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Named.Samples.Greetings;

/// <summary>
/// A task orchestration for greeting a user.
/// </summary>
public class GreetingsOrchestration : TaskOrchestration<string, string>
{
    /// <inheritdoc />
    public override async Task<string> RunTask(OrchestrationContext context, string input)
    {
        string user = await context.ScheduleTask<string>(typeof(GetUserTask));
        string greeting = await context.ScheduleTask<string>(typeof(SendGreetingTask), user);
        return greeting;
    }
}
