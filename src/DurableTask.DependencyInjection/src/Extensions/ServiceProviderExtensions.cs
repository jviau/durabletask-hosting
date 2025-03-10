// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceProvider"/>.
/// </summary>
internal static class ServiceProviderExtensions
{
    /// <summary>
    /// Creates a logger for the provided type from the service provider.
    /// </summary>
    /// <typeparam name="T">The type to derive the logger category from.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The created logger.</returns>
    public static ILogger<T> CreateLogger<T>(this IServiceProvider serviceProvider)
    {
        Check.NotNull(serviceProvider);
        ILoggerFactory factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return factory.CreateLogger<T>();
    }

    /// <summary>
    /// Creates a logger for the provided type from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="type">The type to derive the logger category from.</param>
    /// <returns>The created logger.</returns>
    public static ILogger CreateLogger(this IServiceProvider serviceProvider, Type type)
    {
        Check.NotNull(serviceProvider);
        ILoggerFactory factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return factory.CreateLogger(type);
    }

    /// <summary>
    /// Gets the options of the specified type from the service provider.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="name">The name of the options.</param>
    /// <returns>The options resolved from the <paramref name="serviceProvider"/>.</returns>
    public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider, string? name = null)
    {
        Check.NotNull(serviceProvider);
        name ??= Options.DefaultName;

        IOptionsMonitor<TOptions> optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>();
        return optionsMonitor.Get(name);
    }
}
