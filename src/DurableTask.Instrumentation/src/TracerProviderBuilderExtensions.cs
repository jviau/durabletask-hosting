using DurableTask;
using DurableTask.Core;
// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Instrumentation;

namespace OpenTelemetry.Trace;

/// <summary>
/// Extension methods to simplify registering of dependency instrumentation.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    /// <summary>
    /// Enables DurableTask instrumentation.
    /// </summary>
    /// <param name="builder"><see cref="TracerProviderBuilder"/> being configured.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddDurableTaskInstrumentation(this TracerProviderBuilder builder)
    {
        Check.NotNull(builder);

        builder.AddInstrumentation(() => new DurableTaskInstrumentation());
        builder.AddSource(DurableTaskInstrumentation.ClientSourceName);
        builder.AddSource(DurableTaskInstrumentation.WorkerSourceName);

        return builder;
    }
}
