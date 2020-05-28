using System;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection;

namespace DurableTask.Samples
{
    /// <summary>
    /// This middleware configures the <see cref="OrchestrationInstanceEx"/> for this orchestration.
    /// </summary>
    public class OrchestrationInstanceExMiddleware : ITaskMiddleware
    {
        /// <inheritdoc />
        public Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next)
        {
            // Initialize the OrchestrationInstance with our session data. Or if it already exists,
            // then set it to the appropriate context properties.
            OrchestrationRuntimeState runtimeState = context.GetProperty<OrchestrationRuntimeState>();
            var customInstance = OrchestrationInstanceEx.Initialize(runtimeState);
            context.SetProperty<OrchestrationInstance>(customInstance);

            // Do something with the session data, such as starting a logging scope with correlation id property.

            return next();
        }
    }
}
