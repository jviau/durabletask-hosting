// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Named.Samples.Generics;

/// <summary>
/// An example of an open generic activity.
/// </summary>
/// <typeparam name="T">The open generic type.</typeparam>
public class GenericActivity<T> : TaskActivity<T, string>
{
    /// <inheritdoc />
    protected override string Execute(TaskContext context, T input)
    {
        return $"My generic param is {typeof(T)} with value '{input}'";
    }
}
