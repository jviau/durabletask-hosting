// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace DurableTask.DependencyInjection.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IServiceProvider"/>.
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service from the provider, or activates a new one if it is not available.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The fetched or activated service.</returns>
        public static object GetServiceOrCreateInstance(this IServiceProvider serviceProvider, Type serviceType)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(serviceType, nameof(serviceType));
            return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, serviceType);
        }
    }
}
