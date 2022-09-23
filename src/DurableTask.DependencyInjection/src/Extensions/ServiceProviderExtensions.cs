// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
}
