// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Serializing;
using DurableTask.DependencyInjection.Internal;
using DurableTask.Extensions;
using DurableTask.Extensions.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection;

/// <summary>
/// Extensions for <see cref="ITaskHubWorkerBuilder" />.
/// </summary>
public static class TaskHubWorkerBuilderExtensions
{
    /// <summary>
    /// Adds durable extensions to the worker builder.
    /// </summary>
    /// <param name="builder">The builder to add to.</param>
    /// <returns>The original builder.</returns>
    public static ITaskHubWorkerBuilder AddDurableExtensions(this ITaskHubWorkerBuilder builder)
        => builder.AddDurableExtensions(_ => { });

    /// <summary>
    /// Adds durable extensions to the worker builder.
    /// </summary>
    /// <param name="builder">The builder to add to.</param>
    /// <param name="configure">The options configure action.</param>
    /// <returns>The original builder.</returns>
    public static ITaskHubWorkerBuilder AddDurableExtensions(
        this ITaskHubWorkerBuilder builder, Action<DurableExtensionsOptions> configure)
    {
        Check.NotNull(builder);
        Check.NotNull(configure);

        string name = builder.GetName();
        builder.UseActivityMiddleware(CreateActivityMiddlewareFactory(name));
        builder.UseOrchestrationMiddleware(CreateOrchestrationMiddlewareFactory(name));

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IConfigureOptions<DurableExtensionsOptions>, ConfigureExtensionOptions>());
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostConfigureOptions<InternalTaskHubOptions>, PostConfigureInternalOptions>());
        builder.Services.Configure(name, configure);

        return builder;
    }

    private static Func<IServiceProvider, ITaskMiddleware> CreateActivityMiddlewareFactory(string name)
    {
        return sp =>
        {
            DurableExtensionsOptions options = sp.GetRequiredService<IOptionsMonitor<DurableExtensionsOptions>>()
                .Get(name);

            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new SetActivityDataMiddleware(loggerFactory, options);
        };
    }

    private static Func<IServiceProvider, ITaskMiddleware> CreateOrchestrationMiddlewareFactory(string name)
    {
        return sp =>
        {
            DurableExtensionsOptions options = sp.GetRequiredService<IOptionsMonitor<DurableExtensionsOptions>>()
                .Get(name);

            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new SetOrchestrationDataMiddleware(loggerFactory, options);
        };
    }

    private class ConfigureExtensionOptions(DataConverter? converter = null)
        : IConfigureNamedOptions<DurableExtensionsOptions>
    {
        public void Configure(string name, DurableExtensionsOptions options)
        {
            if (converter is not null && options.DataConverter is null)
            {
                options.DataConverter = converter;
            }
        }

        public void Configure(DurableExtensionsOptions options) => Configure(Options.DefaultName, options);
    }

    private class PostConfigureInternalOptions(IOptionsMonitor<DurableExtensionsOptions> extensionOptions)
        : IPostConfigureOptions<InternalTaskHubOptions>
    {
        public void PostConfigure(string name, InternalTaskHubOptions options)
        {
            name ??= Options.DefaultName;
            options.DataConverter ??= extensionOptions.Get(name).DataConverter;
        }
    }
}
