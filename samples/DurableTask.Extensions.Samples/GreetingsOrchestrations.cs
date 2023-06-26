// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;

namespace DurableTask.Extensions.Samples;

internal class GreetingsOrchestration : IOrchestrationRequest<string>
{
    public TaskOrchestrationDescriptor GetDescriptor() => new(typeof(Handler));

    public class Handler : OrchestrationBase<GreetingsOrchestration, string>
    {
        protected override async Task<string> RunAsync(GreetingsOrchestration input)
        {
            string user = await Context.SendAsync(new GetUserActivity());
            await Context.SendAsync(new SendGreetingActivity(user));
            return user;
        }
    }
}

internal class GetUserActivity : IActivityRequest<string>
{
    public TaskActivityDescriptor GetDescriptor() => new(typeof(Handler));

    public class Handler : ActivityBase<GetUserActivity, string>
    {
        private readonly IConsole _console;

        public Handler(IConsole console)
            => _console = console ?? throw new ArgumentNullException(nameof(console));

        protected override Task<string> RunAsync(GetUserActivity input)
        {
            _console.WriteLine("What is your name?");
            return Task.FromResult(_console.ReadLine());
        }
    }
}

internal sealed class SendGreetingActivity : IActivityRequest
{
    public SendGreetingActivity(string user) => User = user;

    public string User { get; }

    public TaskActivityDescriptor GetDescriptor() => new(typeof(Handler));

    public class Handler : ActivityBase<SendGreetingActivity>
    {
        private readonly IConsole _console;

        public Handler(IConsole console)
            => _console = console ?? throw new ArgumentNullException(nameof(console));

        /// <inheritdoc />
        protected override async Task RunAsync(SendGreetingActivity input)
        {
            string user = input.User;
            if (!string.IsNullOrWhiteSpace(user) && user.Equals("TimedOut"))
            {
                _console.WriteLine("GetUser Timed out!!!");
                throw new TimeoutException();
            }
            else
            {
                _console.WriteLine($"Sending greetings to user: {user}...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                _console.WriteLine($"Greetings sent to {user}");
            }
        }
    }
}

