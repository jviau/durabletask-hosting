// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Exceptions;

namespace DurableTask.Extensions;

/// <summary>
/// Extensions for durable task exceptions.
/// </summary>
internal static class DurableExceptionExtensions
{
    /// <summary>
    /// Gets the first non <see cref="SubOrchestrationFailedException"/> and
    /// <see cref="TaskFailedException"/> in this exception. If there isn't any, returns the inner most exception.
    /// </summary>
    /// <param name="exception">The thrown exception.</param>
    /// <returns>The first non-DTFx exception, or the inner-most one if they are all DTFx.</returns>
    public static Exception GetDurableFailureCause(this Exception exception)
    {
        Check.NotNull(exception);

        while ((exception is SubOrchestrationFailedException || exception is TaskFailedException)
            && exception.InnerException is not null)
        {
            exception = exception.InnerException;
        }

        return exception;
    }
}
