// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Extensions;

/// <summary>
/// Represents a request to run a <see cref="TaskActivity" />.
/// </summary>
public interface IActivityRequest : IActivityRequest<Unit>
{
}
