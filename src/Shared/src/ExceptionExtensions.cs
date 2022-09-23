// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace DurableTask;

/// <summary>
/// Extensions for exceptions.
/// </summary>
internal static class ExceptionExtensions
{
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
