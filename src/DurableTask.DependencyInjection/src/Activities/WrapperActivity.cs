// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using DurableTask.Core;

namespace DurableTask.DependencyInjection.Activities
{
    /// <summary>
    /// An orchestration that wraps the real activity type.
    /// </summary>
    internal class WrapperActivity : TaskActivity
    {
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
        /// Gets or sets the inner activity.
        /// </summary>
        public TaskActivity InnerActivity { get; set; }

        /// <inheritdoc />
        public override string Run(TaskContext context, string input)
        {
            CheckInnerActivity();
            return InnerActivity.Run(context, input);
        }

        /// <inheritdoc />
        public override Task<string> RunAsync(TaskContext context, string input)
        {
            CheckInnerActivity();
            return InnerActivity.RunAsync(context, input);
        }

        private void CheckInnerActivity()
        {
            if (InnerActivity == null)
            {
                throw new InvalidOperationException($"{InnerActivity} not set.");
            }
        }
    }
}
