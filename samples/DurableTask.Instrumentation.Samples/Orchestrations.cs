// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;
using DurableTask.Extensions.Abstractions;
using Newtonsoft.Json;

namespace DurableTask.Extensions.Samples;

/// <summary>
/// A sample orchestration.
/// </summary>
public class TopOrchestration : OrchestrationBase<TopOrchestration.Request, string>
{
    /// <inheritdoc/>
    protected override async Task<string> RunAsync(Request input)
    {
        static async Task Catch(Func<Task> act)
        {
            try
            {
                await act();
            }
            catch
            {
            }
        }

        await Context.RunAsync(new FanOutOrchestration.Request());
        await Context.RunAsync(SimpleOrchestration.CreateRequest(0));

        await Catch(() => Context.RunAsync(SimpleOrchestration.CreateRequest(2)));
        await Catch(() => Context.RunAsync(SimpleOrchestration.CreateRequest(4)));
        await Catch(() => Context.RunAsync(SimpleActivity.CreateRequest(true)));
        await Context.RunAsync(SimpleActivity.CreateRequest(false));

        return "succeeded";
    }

    /// <summary>
    /// The request for <see cref="TopOrchestration" />.
    /// </summary>
    public class Request : IOrchestrationRequest<string>
    {
        /// <inheritdoc/>
        public TaskOrchestrationDescriptor GetDescriptor() => new(typeof(TopOrchestration));
    }
}

/// <summary>
/// A sample orchestration.
/// </summary>
public class FanOutOrchestration : OrchestrationBase<FanOutOrchestration.Request, string>
{
    /// <inheritdoc/>
    protected override async Task<string> RunAsync(Request input)
    {
        List<Task> tasks = new()
        {
            Context.RunAsync(SimpleOrchestration.CreateRequest(0)),
            Context.RunAsync(SimpleOrchestration.CreateRequest(1)),
            Context.RunAsync(SimpleOrchestration.CreateRequest(3)),
            Context.RunAsync(SimpleOrchestration.CreateRequest(1)),
        };

        await Task.WhenAll(tasks);
        return "success";
    }

    /// <summary>
    /// The request for <see cref="FanOutOrchestration" />.
    /// </summary>
    public class Request : IOrchestrationRequest<string>
    {
        /// <inheritdoc/>
        public TaskOrchestrationDescriptor GetDescriptor() => new(typeof(FanOutOrchestration));
    }
}

/// <summary>
/// A sample orchestration.
/// </summary>
[TaskAlias(version: "0")]
[TaskAlias(version: "1")]
[TaskAlias(version: "2")]
[TaskAlias(version: "3")]
[TaskAlias(version: "4")]
public class SimpleOrchestration : OrchestrationBase<SimpleOrchestration.Request, string>
{
    /// <summary>
    /// Creates a request for this activity.
    /// </summary>
    /// <param name="mode">The behavior mode.</param>
    /// <returns>A request object.</returns>
    /// <remarks>
    /// Mode values:
    /// 0 - succeed, no activity call.
    /// 1 - call activity then succeed.
    /// 2 - throw directly, no activity call.
    /// 3 - activity throws, catch, then succeed.
    /// 4 - activity throws, uncaught, fail.
    /// </remarks>
    public static Request CreateRequest(int mode) => new() { Mode = mode };

    /// <inheritdoc/>
    protected override async Task<string> RunAsync(Request input)
    {
        switch (input.Mode)
        {
            case 1:
                await Context.RunAsync(SimpleActivity.CreateRequest(false));
                break;
            case 2:
                throw new InvalidOperationException("orchestration failed");
            case 3:
                try
                {
                    await Context.RunAsync(SimpleActivity.CreateRequest(true));
                }
                catch
                {
                }

                break;
            case 4:
                await Context.RunAsync(SimpleActivity.CreateRequest(true));
                break;
        }

        return "success";
    }

    /// <summary>
    /// The request for <see cref="SimpleOrchestration" />.
    /// </summary>
    public class Request : IOrchestrationRequest<string>
    {
        /// <summary>
        /// Gets or sets the orchestration mode.
        /// 0 - succeed, no activity call.
        /// 1 - call activity then succeed.
        /// 2 - throw directly, no activity call.
        /// 3 - activity throws, catch, then succeed.
        /// 4 - activity throws, uncaught, fail.
        /// </summary>
        public int Mode { get; set; }

        /// <inheritdoc/>
        public TaskOrchestrationDescriptor GetDescriptor()
            => new(typeof(SimpleOrchestration), version: Mode.ToString());
    }
}

/// <summary>
/// A simple activity.
/// </summary>
public class SimpleActivity : ActivityBase<SimpleActivity.Request, string>
{
    /// <summary>
    /// Creates a request for this activity.
    /// </summary>
    /// <param name="throws">True to throw, false to not.</param>
    /// <returns>A request object.</returns>
    public static Request CreateRequest(bool throws) => new() { Throws = throws };

    /// <inheritdoc/>
    protected override Task<string> RunAsync(Request input)
    {
        if (input.Throws)
        {
            throw new InvalidOperationException("activity failed");
        }

        return Task.FromResult("success");
    }

    /// <summary>
    /// The request for <see cref="SimpleActivity" />.
    /// </summary>
    public class Request : IActivityRequest<string>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this activity will throw an exception.
        /// </summary>
        public bool Throws { get; set; }

        /// <inheritdoc/>
        public TaskActivityDescriptor GetDescriptor() => new(typeof(SimpleActivity));
    }
}
