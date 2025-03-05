using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection;

namespace DurableTask.Samples;

/// <summary>
/// Sample middleware
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SampleMiddleware"/> class.
/// </remarks>
/// <param name="console">The console output helper.</param>
public class SampleMiddleware(IConsole console) : ITaskMiddleware
{
    private readonly IConsole _console = console;

    /// <inheritdoc />
    public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        _console.WriteLine("In sample middleware. Dependency Injection works.");
        return next();
    }
}
