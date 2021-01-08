// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;

namespace DurableTask.DependencyInjection.Orchestrations
{
    /// <summary>
    /// A custom orchestration context that enables open-generic support. Primarily, it retains type info when
    /// scheduling closed generic types (so they can be reconstructed on the worker).
    /// </summary>
    internal class WrapperOrchestrationContext : OrchestrationContext
    {
        private readonly OrchestrationContext _innerContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperOrchestrationContext"/> class.
        /// </summary>
        /// <param name="innerContext">The inner context to wrap.</param>
        public WrapperOrchestrationContext(OrchestrationContext innerContext)
        {
            _innerContext = Check.NotNull(innerContext, nameof(innerContext));
            OrchestrationInstance = _innerContext.OrchestrationInstance;
            IsReplaying = _innerContext.IsReplaying;
            MessageDataConverter = _innerContext.MessageDataConverter;
            ErrorDataConverter = _innerContext.ErrorDataConverter;
        }

        /// <inheritdoc/>
        public override DateTime CurrentUtcDateTime => _innerContext.CurrentUtcDateTime;

        /// <inheritdoc/>
        public override void ContinueAsNew(object input)
        {
            UpdateInnerProperties();
            _innerContext.ContinueAsNew(input);
        }

        /// <inheritdoc/>
        public override void ContinueAsNew(string newVersion, object input)
        {
            UpdateInnerProperties();
            _innerContext.ContinueAsNew(newVersion, input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstance<T>(string name, string version, object input)
        {
            UpdateInnerProperties();
            return _innerContext.CreateSubOrchestrationInstance<T>(name, version, input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstance<T>(
            string name, string version, string instanceId, object input)
        {
            UpdateInnerProperties();
            return _innerContext.CreateSubOrchestrationInstance<T>(name, version, instanceId, input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstance<T>(
            string name, string version, string instanceId, object input, IDictionary<string, string> tags)
        {
            UpdateInnerProperties();
            return _innerContext.CreateSubOrchestrationInstance<T>(name, version, instanceId, input, tags);
        }

        /// <inheritdoc/>
        public override Task<T> CreateTimer<T>(DateTime fireAt, T state)
        {
            UpdateInnerProperties();
            return _innerContext.CreateTimer(fireAt, state);
        }

        /// <inheritdoc/>
        public override Task<T> CreateTimer<T>(DateTime fireAt, T state, CancellationToken cancelToken)
        {
            UpdateInnerProperties();
            return _innerContext.CreateTimer(fireAt, state, cancelToken);
        }

        /// <inheritdoc/>
        public override Task<TResult> ScheduleTask<TResult>(string name, string version, params object[] parameters)
        {
            UpdateInnerProperties();
            return _innerContext.ScheduleTask<TResult>(name, version, parameters);
        }

        /// <inheritdoc/>
        public override void SendEvent(OrchestrationInstance orchestrationInstance, string eventName, object eventData)
        {
            UpdateInnerProperties();
            _innerContext.SendEvent(orchestrationInstance, eventName, eventData);
        }

        /// <inheritdoc/>
        public override Task<TResult> ScheduleTask<TResult>(Type activityType, params object[] parameters)
        {
            UpdateInnerProperties();
            return ScheduleTask<TResult>(
                GenericNameHelper.GetDefaultName(activityType),
                NameVersionHelper.GetDefaultVersion(activityType),
                parameters);
        }

        /// <inheritdoc/>
        public override Task<T> ScheduleWithRetry<T>(
            Type taskActivityType, RetryOptions retryOptions, params object[] parameters)
        {
            UpdateInnerProperties();
            return ScheduleWithRetry<T>(
                GenericNameHelper.GetDefaultName(taskActivityType),
                NameVersionHelper.GetDefaultVersion(taskActivityType),
                retryOptions,
                parameters);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstance<T>(Type orchestrationType, object input)
        {
            UpdateInnerProperties();
            return CreateSubOrchestrationInstance<T>(
                GenericNameHelper.GetDefaultName(orchestrationType),
                NameVersionHelper.GetDefaultVersion(orchestrationType),
                input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstance<T>(
            Type orchestrationType, string instanceId, object input)
        {
            UpdateInnerProperties();
            return CreateSubOrchestrationInstance<T>(
                GenericNameHelper.GetDefaultName(orchestrationType),
                NameVersionHelper.GetDefaultVersion(orchestrationType),
                instanceId,
                input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
            Type orchestrationType, RetryOptions retryOptions, object input)
        {
            UpdateInnerProperties();
            return CreateSubOrchestrationInstanceWithRetry<T>(
                GenericNameHelper.GetDefaultName(orchestrationType),
                NameVersionHelper.GetDefaultVersion(orchestrationType),
                retryOptions,
                input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
            Type orchestrationType, string instanceId, RetryOptions retryOptions, object input)
        {
            UpdateInnerProperties();
            return CreateSubOrchestrationInstanceWithRetry<T>(
                GenericNameHelper.GetDefaultName(orchestrationType),
                NameVersionHelper.GetDefaultVersion(orchestrationType),
                instanceId,
                retryOptions,
                input);
        }

        /// <inheritdoc/>
        public override T CreateClient<T>()
        {
            UpdateInnerProperties();
            return _innerContext.CreateClient<T>();
        }

        /// <inheritdoc/>
        public override T CreateClient<T>(bool useFullyQualifiedMethodNames)
        {
            UpdateInnerProperties();
            return _innerContext.CreateClient<T>(useFullyQualifiedMethodNames);
        }

        /// <inheritdoc/>
        public override T CreateRetryableClient<T>(RetryOptions retryOptions)
        {
            UpdateInnerProperties();
            return _innerContext.CreateRetryableClient<T>(retryOptions);
        }

        /// <inheritdoc/>
        public override T CreateRetryableClient<T>(RetryOptions retryOptions, bool useFullyQualifiedMethodNames)
        {
            UpdateInnerProperties();
            return _innerContext.CreateRetryableClient<T>(retryOptions, useFullyQualifiedMethodNames);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
            string name, string version, RetryOptions retryOptions, object input)
        {
            UpdateInnerProperties();
            return _innerContext.CreateSubOrchestrationInstanceWithRetry<T>(name, version, retryOptions, input);
        }

        /// <inheritdoc/>
        public override Task<T> CreateSubOrchestrationInstanceWithRetry<T>(
            string name, string version, string instanceId, RetryOptions retryOptions, object input)
        {
            UpdateInnerProperties();
            return _innerContext.CreateSubOrchestrationInstanceWithRetry<T>(
                name, version, instanceId, retryOptions, input);
        }

        private void UpdateInnerProperties()
        {
            // We have no way to hook in to the public setter of these properties to also set it on the wrapped context.
            // Instead, we will have to update these properties on *every* call to the wrapped context, to ensure it is
            // up to date.
            _innerContext.ErrorDataConverter = ErrorDataConverter;
            _innerContext.MessageDataConverter = MessageDataConverter;
        }
    }
}
