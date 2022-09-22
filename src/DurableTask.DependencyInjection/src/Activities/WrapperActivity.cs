// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;

using ActivityFactory = System.Func<System.IServiceProvider, DurableTask.Core.TaskActivity>;

namespace DurableTask.DependencyInjection.Activities;

/// <summary>
/// An activity that wraps the real activity type.
/// </summary>
internal class WrapperActivity : TaskActivity
{
    private static readonly ConcurrentDictionary<TaskActivityDescriptor, ActivityFactory> s_factories = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperActivity"/> class.
    /// </summary>
    /// <param name="descriptor">The inner orchestration descriptor.</param>
    public WrapperActivity(TaskActivityDescriptor descriptor)
    {
        Descriptor = Check.NotNull(descriptor, nameof(descriptor));
    }

    /// <summary>
    /// Gets the activity descriptor.
    /// </summary>
    public TaskActivityDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the inner activity.
    /// </summary>
    public TaskActivity InnerActivity { get; private set; }

    /// <summary>
    /// Creates the inner activity, setting <see cref="InnerActivity" />.
    /// </summary>
    /// <param name="serviceProvider">The service provider. Not null.</param>
    public void Initialize(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider, nameof(serviceProvider));

        // Reflection activity
        if (Descriptor.Method is not null)
        {
            // The "DeclaringType" must be an interface so that DTFx can emit a dynamic wrapper in an orchestration.
            // As such, we expect the implementation to be registered in the service provider.
            object obj = serviceProvider.GetRequiredService(Descriptor.Method.DeclaringType);
            InnerActivity = new ReflectionBasedTaskActivity(obj, Descriptor.Method);
            return;
        }

        // Service activity
        if (!s_factories.TryGetValue(Descriptor, out ActivityFactory factory))
        {
            if (serviceProvider.GetService(Descriptor.Type) is TaskActivity activity)
            {
                InnerActivity = activity;
                s_factories.TryAdd(Descriptor, sp => (TaskActivity)sp.GetRequiredService(Descriptor.Type));
                return; // already created it this time, so return now.
            }
            else
            {
                ObjectFactory objectFactory = ActivatorUtilities.CreateFactory(
                    Descriptor.Type, Array.Empty<Type>());
                factory = s_factories.GetOrAdd(
                    Descriptor, sp => (TaskActivity)objectFactory.Invoke(sp, Array.Empty<object>()));
            }
        }

        InnerActivity = factory.Invoke(serviceProvider);
        return;
    }

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
        if (InnerActivity is null)
        {
            throw new InvalidOperationException(Strings.InnerActivityNull);
        }
    }
}
