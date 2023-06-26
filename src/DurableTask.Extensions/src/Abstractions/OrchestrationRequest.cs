// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;
using DurableTask.Extensions.Abstractions;

namespace DurableTask.Extensions;

/// <summary>
/// Helpers for creating orchestration requests.
/// </summary>
public static class OrchestrationRequest
{
    /// <summary>
    /// Gets an <see cref="IOrchestrationRequest{TResult}" /> which has an explicitly provided input.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to use an existing type for input (like <see cref="string" />) and not derive an
    /// entirely new type.
    /// </remarks>
    /// <typeparam name="TResult">The result type of the orchestration.</typeparam>
    /// <param name="descriptor">The descriptor of the orchestration to run.</param>
    /// <param name="input">The input for the orchestration.</param>
    /// <returns>A request that can be used to enqueue an orchestration.</returns>
    public static IOrchestrationRequest<TResult> Create<TResult>(TaskOrchestrationDescriptor descriptor, object? input = null)
        => new Request<TResult>(Check.NotNull(descriptor), input);

    /// <summary>
    /// Gets an <see cref="IOrchestrationRequest" /> which has an explicitly provided input.
    /// </summary>
    /// <remarks>
    /// This is useful when you want to use an existing type for input (like <see cref="string" />) and not derive an
    /// entirely new type.
    /// </remarks>
    /// <param name="descriptor">The descriptor of the orchestration to run.</param>
    /// <param name="input">The input for the orchestration.</param>
    /// <returns>A request that can be used to enqueue an orchestration.</returns>
    public static IOrchestrationRequest Create(TaskOrchestrationDescriptor descriptor, object? input = null)
        => new Request(descriptor, input);

    /// <summary>
    /// Gets the orchestration input from a <see cref="IBaseOrchestrationRequest" />.
    /// </summary>
    /// <param name="request">The request to get input for.</param>
    /// <returns>The input.</returns>
    internal static object? GetInput(this IBaseOrchestrationRequest request)
    {
        if (request is IProvidesInput provider)
        {
            return provider.GetInput();
        }

        return request;
    }

    private class Request<TResult> : RequestCore, IOrchestrationRequest<TResult>
    {
        public Request(TaskOrchestrationDescriptor descriptor, object? input)
            : base(descriptor, input)
        {
        }
    }

    private class Request : RequestCore, IOrchestrationRequest
    {
        public Request(TaskOrchestrationDescriptor descriptor, object? input)
            : base(descriptor, input)
        {
        }
    }

    private class RequestCore : IBaseOrchestrationRequest, IProvidesInput
    {
        private readonly TaskOrchestrationDescriptor _descriptor;
        private readonly object? _input;

        public RequestCore(TaskOrchestrationDescriptor descriptor, object? input)
        {
            _descriptor = descriptor;
            _input = input;
        }

        public object? GetInput() => _input;

        public TaskOrchestrationDescriptor GetDescriptor() => _descriptor;
    }
}
