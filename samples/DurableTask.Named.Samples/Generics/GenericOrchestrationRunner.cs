// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Named.Samples.Generics;

/// <summary>
/// Runner for generic tasks example.
/// </summary>
public class GenericOrchestrationRunner : TaskOrchestration<string, string>
{
    /// <inheritdoc />
    public override async Task<string> RunTask(OrchestrationContext context, string input)
    {
        string result = await context.ScheduleTask<string>(typeof(GenericActivity<int>), 10);
        await PrintAsync(context, result);

        result = await context.ScheduleTask<string>(typeof(GenericActivity<string>), "example");
        await PrintAsync(context, result);

        result = await context.ScheduleTask<string>(typeof(GenericActivity<MyClass>), new MyClass());
        await PrintAsync(context, result);

        return string.Empty;
    }

    private static Task<string> PrintAsync(OrchestrationContext context, string input)
    {
        return context.ScheduleTask<string>(typeof(PrintTask), input);
    }

    private class MyClass
    {
        public override string ToString() => "Example private class";
    }
}
