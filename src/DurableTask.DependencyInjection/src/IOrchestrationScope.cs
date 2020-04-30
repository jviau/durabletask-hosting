// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// The scope for a running orchestration instance.
    /// </summary>
    internal interface IOrchestrationScope : IServiceScope
    {
        /// <summary>
        /// Enters the scope.
        /// </summary>
        /// <returns>A disposable that exits this scope.</returns>
        IDisposable Enter();
    }
}
