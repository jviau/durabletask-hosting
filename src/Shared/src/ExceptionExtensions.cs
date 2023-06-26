// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.InteropServices;
using DurableTask.Core.Exceptions;

namespace DurableTask;

/// <summary>
/// Extensions for exceptions.
/// </summary>
internal static class ExceptionExtensions
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

    /// <summary>
    /// Returns a culture-independent string representation of the given <paramref name="exception"/> object,
    /// appropriate for diagnostics tracing.
    /// </summary>
    /// <param name="exception">Exception to convert to string.</param>
    /// <returns>Exception as string with no culture.</returns>
    public static string ToInvariantString(this Exception exception)
    {
        CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            return exception.ToString();
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }

    /// <summary>
    /// Determines if an exception is fatal, and should not be caught.
    /// </summary>
    /// <param name="exception">The exception to catch.</param>
    /// <returns>True if fatal, false otherwise (or if null).</returns>
    public static bool IsFatal(this Exception exception)
    {
        while (exception is not null)
        {
            if (exception.IsFatalCore())
            {
                return true;
            }

            if (exception is AggregateException aggregate)
            {
                // AggregateExceptions have a collection of inner exceptions, which may themselves be other
                // wrapping exceptions (including nested AggregateExceptions). Recursively walk this
                // hierarchy. The (singular) InnerException is included in the collection.
                foreach (Exception innerException in aggregate.InnerExceptions)
                {
                    if (innerException.IsFatal())
                    {
                        return true;
                    }
                }

                break;
            }
            else
            {
                exception = exception.InnerException;
            }
        }

        return false;
    }

    private static bool IsFatalCore(this Exception exception)
    {
        return exception is OutOfMemoryException ||
                exception is StackOverflowException || // can't actually be caught
                exception is NullReferenceException || // this is a underlying code issue, we want to surface these.
                exception is SEHException ||
                exception is AccessViolationException || // can be caught, but means corrupted state.
                exception is ThreadAbortException; // can be caught, but will always be re-thrown.
    }
}
