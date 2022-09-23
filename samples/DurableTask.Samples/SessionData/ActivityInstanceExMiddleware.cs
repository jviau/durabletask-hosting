using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection;

namespace DurableTask.Samples;

/// <summary>
/// This middleware configures the <see cref="OrchestrationInstanceEx"/> for this activity.
/// </summary>
public class ActivityInstanceExMiddleware : ITaskMiddleware
{
    /// <inheritdoc />
    public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
    {
        var customInstance = OrchestrationInstanceEx.Get(context.GetProperty<OrchestrationInstance>());
        context.SetProperty<OrchestrationInstance>(customInstance);

        // Do something with the session data, such as starting a logging scope with correlation id property.

        return next();
    }
}
