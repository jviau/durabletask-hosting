// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;
using DurableTask.Extensions.Abstractions;

namespace DurableTask.Extensions;

/// <summary>
/// Helpers for creating activity requests.
/// </summary>
public static class ActivityRequest
{
    /// <summary>
    /// Gets an <see cref="IActivityRequest{TResult}" /> which has an explicitly provided input.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to use an existing type for input (like <see cref="string" />) and not derive an
    /// entirely new type.
    /// </remarks>
    /// <typeparam name="TResult">The result type of the activity.</typeparam>
    /// <param name="descriptor">The descriptor of the activity to run.</param>
    /// <param name="input">The input for the activity.</param>
    /// <returns>A request that can be used to enqueue an activity.</returns>
    public static IActivityRequest<TResult> Create<TResult>(TaskActivityDescriptor descriptor, object? input = null)
        => new Request<TResult>(Check.NotNull(descriptor), input);

    /// <summary>
    /// Gets an <see cref="IActivityRequest" /> which has an explicitly provided input.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to use an existing type for input (like <see cref="string" />) and not derive an
    /// entirely new type.
    /// </remarks>
    /// <param name="descriptor">The descriptor of the activity to run.</param>
    /// <param name="input">The input for the activity.</param>
    /// <returns>A request that can be used to enqueue an activity.</returns>
    public static IActivityRequest Create(TaskActivityDescriptor descriptor, object? input = null)
        => new Request(descriptor, input);

    /// <summary>
    /// Gets the activity input from a <see cref="IBaseActivityRequest" />.
    /// </summary>
    /// <param name="request">The request to get input for.</param>
    /// <returns>The input.</returns>
    internal static object? GetInput(this IBaseActivityRequest request)
    {
        if (request is IProvidesInput provider)
        {
            return provider.GetInput();
        }

        return request;
    }

    private class Request<TResult> : RequestCore, IActivityRequest<TResult>
    {
        public Request(TaskActivityDescriptor descriptor, object? input)
            : base(descriptor, input)
        {
        }
    }

    private class Request : RequestCore, IActivityRequest
    {
        public Request(TaskActivityDescriptor descriptor, object? input)
            : base(descriptor, input)
        {
        }
    }

    private class RequestCore : IBaseActivityRequest, IProvidesInput
    {
        private readonly TaskActivityDescriptor _descriptor;
        private readonly object? _input;

        public RequestCore(TaskActivityDescriptor descriptor, object? input)
        {
            _descriptor = descriptor;
            _input = input;
        }

        public object? GetInput() => _input;

        public TaskActivityDescriptor GetDescriptor() => _descriptor;
    }
}
