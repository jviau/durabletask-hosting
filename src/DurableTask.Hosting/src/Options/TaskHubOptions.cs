// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;

namespace DurableTask.Hosting.Options
{
    /// <summary>
    /// The options for the task hub host.
    /// </summary>
    public class TaskHubOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to create the orchestration instance resources if they
        /// do not exist on startup. <see cref="IOrchestrationService.CreateIfNotExistsAsync"/>.
        /// </summary>
        public bool CreateIfNotExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include error details in task failures or not.
        /// <see cref="TaskActivityDispatcher.IncludeDetails"/> and <see cref="TaskOrchestrationDispatcher.IncludeDetails"/>.
        /// </summary>
        public IncludeDetails IncludeDetails { get; set; }
    }
}
