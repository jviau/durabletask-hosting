// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Middleware;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Middleware for running in the task hub worker pipeline.
/// </summary>
public interface ITaskMiddleware
{
    /// <summary>
    /// Task hub middleware handling method.
    /// </summary>
    /// <param name="context">The <see cref="DispatchMiddlewareContext"/> context for this pipeline.</param>
    /// <param name="next">The delegate representing the remaining middleware in the pipeline.</param>
    /// <returns>A <see cref="Task"/> that represents the execution of this middleware.</returns>
    Task InvokeAsync(DispatchMiddlewareContext context, Func<Task> next);
}
