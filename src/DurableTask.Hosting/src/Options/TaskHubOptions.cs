// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Exceptions;

namespace DurableTask.Hosting.Options;

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

    /// <summary>
    /// Gets or sets the error propagation behavior when an activity or orchestration fails with an unhandled exception.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use caution when making changes to this property over the lifetime of an application. In-flight orchestrations
    /// could fail unexpectedly if there is any logic that depends on a particular behavior of exception propagation.
    /// For example, setting <see cref="ErrorPropagationMode.UseFailureDetails"/> causes
    /// <see cref="OrchestrationException.FailureDetails"/> to be populated in <see cref="TaskFailedException"/> and
    /// <see cref="SubOrchestrationFailedException"/> but also causes the <see cref="Exception.InnerException"/>
    /// property to be <c>null</c> for these exception types.
    /// </para><para>
    /// This property must be set before the worker is started. Otherwise it will have no effect.
    /// </para>
    /// </remarks>
    public ErrorPropagationMode ErrorPropagationMode { get; set; }
}
