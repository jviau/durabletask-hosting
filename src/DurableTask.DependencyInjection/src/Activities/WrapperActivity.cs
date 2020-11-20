// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection.Activities
{
    /// <summary>
    /// An orchestration that wraps the real activity type.
    /// </summary>
    internal class WrapperActivity : TaskActivity
    {
        private static readonly ConcurrentDictionary<Type, ObjectFactory> s_factories
            = new ConcurrentDictionary<Type, ObjectFactory>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapperActivity"/> class.
        /// </summary>
        /// <param name="innerActivityType">The inner activity type to use.</param>
        public WrapperActivity(Type innerActivityType)
        {
            Check.NotNull(innerActivityType, nameof(innerActivityType));
            Check.ConcreteType<TaskActivity>(innerActivityType, nameof(innerActivityType));

            InnerActivityType = innerActivityType;
        }

        /// <summary>
        /// Gets the inner activity type.
        /// </summary>
        public Type InnerActivityType { get; }

        /// <summary>
        /// Gets the inner activity.
        /// </summary>
        public TaskActivity InnerActivity { get; private set; }

        /// <summary>
        /// Creates the inner activity, setting <see cref="InnerActivity" />.
        /// </summary>
        /// <param name="serviceProvider">The service provider. Not null.</param>
        public void CreateInnerActivity(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            if (serviceProvider.GetService(InnerActivityType) is TaskActivity activity)
            {
                InnerActivity = activity;
                return;
            }

            ObjectFactory factory = s_factories.GetOrAdd(
                InnerActivityType, t => ActivatorUtilities.CreateFactory(t, Array.Empty<Type>()));
            InnerActivity = (TaskActivity)factory.Invoke(serviceProvider, Array.Empty<object>());
        }

        /// <inheritdoc />
        public override string Run(TaskContext context, string input)
        {
            CheckInnerActivity();
            using (OrchestrationScope.EnterScope(context.OrchestrationInstance.InstanceId))
            {
                return InnerActivity.Run(context, input);
            }
        }

        /// <inheritdoc />
        public override async Task<string> RunAsync(TaskContext context, string input)
        {
            CheckInnerActivity();
            using (OrchestrationScope.EnterScope(context.OrchestrationInstance.InstanceId))
            {
                return await InnerActivity.RunAsync(context, input).ConfigureAwait(false);
            }
        }

        private void CheckInnerActivity()
        {
            if (InnerActivity == null)
            {
                throw new InvalidOperationException(Strings.InnerActivityNull);
            }
        }
    }
}
