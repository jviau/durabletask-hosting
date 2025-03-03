using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection;

namespace DurableTask.Named.Samples;

/// <summary>
/// Sample middleware
/// </summary>
public class SampleMiddleware : ITaskMiddleware
{
    private readonly IConsole _console;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleMiddleware"/> class.
    /// </summary>
    /// <param name="console">The console output helper.</param>
    public SampleMiddleware(IConsole console)
    {
        _console = console;
    }

    /// <inheritdoc />
    public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        _console.WriteLine("In sample middleware. Dependency Injection works.");
        return next();
    }
}
