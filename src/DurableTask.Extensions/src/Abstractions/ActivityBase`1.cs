// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

namespace DurableTask.Extensions.Abstractions;

/// <inheritdoc />
public abstract class ActivityBase<TInput> : ActivityBase<TInput, Empty>
    where TInput : IActivityRequest<Empty>
{
}
