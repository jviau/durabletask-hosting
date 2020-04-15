// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection
{
    /// <summary>
    /// The scope for a running orchestration instance.
    /// </summary>
    internal interface IOrchestrationScope : IServiceScope
    {
        /// <summary>
        /// Signals that the middleware portion has completed, so the service scope can be disposed.
        /// </summary>
        void SignalMiddlewareCompletion();

        /// <summary>
        /// Wait for middleware to complete processing.
        /// </summary>
        /// <returns>A task that completes when middleware is done.</returns>
        Task WaitForMiddlewareCompletionAsync();
    }
}
