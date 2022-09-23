// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using DurableTask.Core;
using DurableTask.DependencyInjection.Properties;
using Microsoft.Extensions.DependencyInjection;

using OrchestrationFactory = System.Func<System.IServiceProvider, DurableTask.Core.TaskOrchestration>;

namespace DurableTask.DependencyInjection.Orchestrations;

/// <summary>
/// An orchestration that wraps the real orchestration type.
/// </summary>
internal class WrapperOrchestration : TaskOrchestration
{
    private static readonly ConcurrentDictionary<
        TaskOrchestrationDescriptor, OrchestrationFactory> s_factories = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapperOrchestration"/> class.
    /// </summary>
    /// <param name="descriptor">The inner orchestration descriptor.</param>
    public WrapperOrchestration(TaskOrchestrationDescriptor descriptor)
    {
        Descriptor = Check.NotNull(descriptor);
    }

    /// <summary>
    /// Gets the orchestration descriptor.
    /// </summary>
    public TaskOrchestrationDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the inner orchestration.
    /// </summary>
    public TaskOrchestration InnerOrchestration { get; private set; }

    /// <summary>
    /// Creates the inner orchestration, setting <see cref="InnerOrchestration" />.
    /// </summary>
    /// <param name="serviceProvider">The service provider. Not null.</param>
    public void Initialize(IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider);

        if (!s_factories.TryGetValue(Descriptor, out OrchestrationFactory factory))
        {
            if (serviceProvider.GetService(Descriptor.Type) is TaskOrchestration orchestration)
            {
                InnerOrchestration = orchestration;
                s_factories.TryAdd(Descriptor, sp => (TaskOrchestration)sp.GetRequiredService(Descriptor.Type));
                return; // already created it this time, so just return now.
            }
            else
            {
                ObjectFactory objectFactory = ActivatorUtilities.CreateFactory(
                    Descriptor.Type, Array.Empty<Type>());
                factory = s_factories.GetOrAdd(
                    Descriptor, sp => (TaskOrchestration)objectFactory.Invoke(sp, Array.Empty<object>()));
            }
        }

        InnerOrchestration = factory.Invoke(serviceProvider);
        return;
    }

    /// <inheritdoc />
    public override Task<string> Execute(OrchestrationContext context, string input)
    {
        CheckInnerOrchestration();
        context = new WrapperOrchestrationContext(context);
        return InnerOrchestration.Execute(context, input);
    }

    /// <inheritdoc />
    public override string GetStatus()
    {
        CheckInnerOrchestration();
        return InnerOrchestration.GetStatus();
    }

    /// <inheritdoc />
    public override void RaiseEvent(OrchestrationContext context, string name, string input)
    {
        CheckInnerOrchestration();
        context = new WrapperOrchestrationContext(context);
        InnerOrchestration.RaiseEvent(context, name, input);
    }

    private void CheckInnerOrchestration()
    {
        if (InnerOrchestration is null)
        {
            throw new InvalidOperationException(Strings.InnerOrchestrationNull);
        }
    }
}
