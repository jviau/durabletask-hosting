// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Reflection;
using DurableTask.Core;
using DurableTask.Core.Middleware;
using DurableTask.DependencyInjection.Extensions;

namespace DurableTask.DependencyInjection;

/// <summary>
/// TaskHub <see cref="TaskOrchestration" /> extensions for <see cref="ITaskHubWorkerBuilder"/>.
/// </summary>
public static class TaskHubWorkerBuilderOrchestrationExtensions
{
    /// <summary>
    /// Adds the provided descriptor to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="descriptor">The descriptor to add.</param>
    /// <returns>This instance, for chaining calls.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration(
        this ITaskHubWorkerBuilder builder, TaskOrchestrationDescriptor descriptor)
    {
        Check.NotNull(builder);
        Check.NotNull(descriptor);

        builder.Orchestrations.Add(descriptor);
        return builder;
    }

    /// <summary>
    /// Adds the provided orchestration type to the builder.
    /// Includes <see cref="TaskAliasAttribute"/>.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="type">The orchestration type to add, not null.</param>
    /// <returns>The original builder with orchestration added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration(this ITaskHubWorkerBuilder builder, Type type)
        => builder.AddOrchestration(type, includeAliases: true);

    /// <summary>
    /// Adds the provided orchestration type to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="type">The orchestration type to add, not null.</param>
    /// <param name="includeAliases">Include <see cref="TaskAliasAttribute"/>.</param>
    /// <returns>The original builder with orchestration added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration(
        this ITaskHubWorkerBuilder builder, Type type, bool includeAliases)
    {
        Check.NotNull(builder);

        builder.AddOrchestration(new TaskOrchestrationDescriptor(type));
        if (includeAliases)
        {
            foreach ((string? name, string? version) in type.GetTaskAliases())
            {
                builder.AddOrchestration(new TaskOrchestrationDescriptor(type, name, version));
            }
        }

        return builder;
    }

    /// <summary>
    /// Adds the provided orchestration type to the builder.
    /// Includes <see cref="TaskAliasAttribute"/>.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
    /// <returns>The original builder with orchestration added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(this ITaskHubWorkerBuilder builder)
        where TOrchestration : TaskOrchestration
        => builder.AddOrchestration(typeof(TOrchestration));

    /// <summary>
    /// Adds the provided orchestration type to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="includeAliases">Include <see cref="TaskAliasAttribute"/>.</param>
    /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
    /// <returns>The original builder with orchestration added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
        this ITaskHubWorkerBuilder builder, bool includeAliases)
        where TOrchestration : TaskOrchestration
        => builder.AddOrchestration(typeof(TOrchestration), includeAliases);

    /// <summary>
    /// Adds the provided orchestration type to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="type">The orchestration type to add, not null.</param>
    /// <param name="name">The name of the orchestration. Not null or empty.</param>
    /// <param name="version">The version of the orchestration.static Not null.</param>
    /// <returns>The original builder with orchestration added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration(
        this ITaskHubWorkerBuilder builder, Type type, string name, string version)
    {
        Check.NotNull(builder);
        Check.NotNullOrEmpty(name);
        Check.NotNull(version);

        builder.AddOrchestration(new TaskOrchestrationDescriptor(type, name, version));
        return builder;
    }

    /// <summary>
    /// Adds the provided orchestration type to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="name">The name of the orchestration. Not null or empty.</param>
    /// <param name="version">The version of the orchestration.static Not null.</param>
    /// <typeparam name="TOrchestration">The orchestration type to add.</typeparam>
    /// <returns>The original builder with orchestration added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestration<TOrchestration>(
        this ITaskHubWorkerBuilder builder, string name, string version)
        where TOrchestration : TaskOrchestration
        => builder.AddOrchestration(typeof(TOrchestration), name, version);

    /// <summary>
    /// Adds all <see cref="TaskOrchestration"/> in the provided assembly.
    /// Includes <see cref="TaskAliasAttribute"/>.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="assembly">The assembly to discover types from. Not null.</param>
    /// <param name="includePrivate">True to include private/protected/internal types, false for public only.</param>
    /// <returns>The original builder with activity added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestrationsFromAssembly(
        this ITaskHubWorkerBuilder builder, Assembly assembly, bool includePrivate = false)
    {
        Check.NotNull(builder);
        Check.NotNull(assembly);

        foreach (Type type in assembly.GetConcreteTypes<TaskOrchestration>(includePrivate))
        {
            builder.AddOrchestration(type);
        }

        return builder;
    }

    /// <summary>
    /// Adds all <see cref="TaskOrchestration"/> in the provided assembly.
    /// Includes <see cref="TaskAliasAttribute"/>.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="includePrivate">True to also include private/protected/internal types, false for public only.</param>
    /// <typeparam name="T">The type contained in the assembly to discover types from.</typeparam>
    /// <returns>The original builder with activity added.</returns>
    public static ITaskHubWorkerBuilder AddOrchestrationsFromAssembly<T>(
        this ITaskHubWorkerBuilder builder, bool includePrivate = false)
        => builder.AddOrchestrationsFromAssembly(typeof(T).Assembly, includePrivate);

    /// <summary>
    /// Adds the provided middleware for task orchestrations.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="descriptor">The middleware descriptor to add.</param>
    /// <returns>This instance, for chaining calls.</returns>
    public static ITaskHubWorkerBuilder UseOrchestrationMiddleware(
        this ITaskHubWorkerBuilder builder, TaskMiddlewareDescriptor descriptor)
    {
        Check.NotNull(builder);
        Check.NotNull(descriptor);

        builder.OrchestrationMiddleware.Add(descriptor);
        return builder;
    }

    /// <summary>
    /// Adds the provided orchestration middleware to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="type">The orchestration middleware type to add, not null.</param>
    /// <returns>The original builder with orchestration middleware added.</returns>
    public static ITaskHubWorkerBuilder UseOrchestrationMiddleware(this ITaskHubWorkerBuilder builder, Type type)
    {
        Check.NotNull(builder);

        builder.UseOrchestrationMiddleware(new TaskMiddlewareDescriptor(type));
        return builder;
    }

    /// <summary>
    /// Adds the provided orchestration middleware to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <typeparam name="TMiddleware">The orchestration middleware type to add.</typeparam>
    /// <returns>The original builder with orchestration middleware added.</returns>
    public static ITaskHubWorkerBuilder UseOrchestrationMiddleware<TMiddleware>(this ITaskHubWorkerBuilder builder)
        where TMiddleware : ITaskMiddleware
        => builder.UseOrchestrationMiddleware(typeof(TMiddleware));

    /// <summary>
    /// Adds the provided orchestration middleware to the builder.
    /// </summary>
    /// <param name="builder">The builder to add to, not null.</param>
    /// <param name="func">The orchestration middleware func to add, not null.</param>
    /// <returns>The original builder with orchestration middleware added.</returns>
    public static ITaskHubWorkerBuilder UseOrchestrationMiddleware(
        this ITaskHubWorkerBuilder builder, Func<DispatchMiddlewareContext, Func<Task>, Task> func)
    {
        Check.NotNull(builder);
        Check.NotNull(func);

        builder.UseOrchestrationMiddleware(new TaskMiddlewareDescriptor(func));
        return builder;
    }
}
