// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.Hosting.Extensions.Tests;

public class TaskHubHostBuilderExtensionsTests
{
    [Fact]
    public void Configure_HostBuilder_TaskHubWorker_ArgumentNullBuilder()
        => RunTestException<ArgumentNullException>(
            (IHostBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker((IHostBuilder)null, b => { }));
    [Fact]
    public void Configure_HostApplicationBuilder_TaskHubWorker_ArgumentNullBuilder()
        => RunTestException<ArgumentNullException>(
            (IHostApplicationBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker((IHostApplicationBuilder)null, b => { }));
    [Fact]
    public void Configure_HostBuilder_TaskHubWorker_ArgumentNullConfigure()
        => RunTestException<ArgumentNullException>(
            (IHostBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker(builder, (Action<ITaskHubWorkerBuilder>)null));

    [Fact]
    public void Configure_HostApplicationBuilder_TaskHubWorker_ArgumentNullConfigure()
        => RunTestException<ArgumentNullException>(
            (IHostApplicationBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker(builder, (Action<ITaskHubWorkerBuilder>)null));

    [Fact]
    public void Configure_HostBuilder_TaskHubWorker_ArgumentNullBuilder2()
        => RunTestException<ArgumentNullException>(
            (IHostBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker((IHostBuilder)null, (c, b) => { }));
    [Fact]
    public void Configure_HostApplicationBuilder_TaskHubWorker_ArgumentNullBuilder2()
        => RunTestException<ArgumentNullException>(
            (IHostApplicationBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker((IHostApplicationBuilder)null, (c, b) => { }));

    [Fact]
    public void Configure_HostBuilder_TaskHubWorker_ArgumentNullConfigure2()
        => RunTestException<ArgumentNullException>(
            (IHostBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker(builder, (Action<HostBuilderContext, ITaskHubWorkerBuilder>)null));
    [Fact]
    public void Configure_HostApplicationBuilder_TaskHubWorker_ArgumentNullConfigure2()
        => RunTestException<ArgumentNullException>(
            (IHostApplicationBuilder builder) => TaskHubHostBuilderExtensions
                .ConfigureTaskHubWorker(builder, (Action<IHostApplicationBuilder, ITaskHubWorkerBuilder>)null));

    [Fact]
    public void Configure_HostBuilder_TaskHubWorker_Callback()
    {
        ITaskHubWorkerBuilder capturedBuilder = null;
        RunTest(
            (IHostBuilder builder) => builder.ConfigureTaskHubWorker(b => capturedBuilder = b),
            (builder, returned) =>
            {
                returned.Should().BeSameAs(builder);
                capturedBuilder.Should().NotBeNull();
            });
    }

    [Fact]
    public void Configure_HostApplicationBuilder_TaskHubWorker_Callback()
    {
        ITaskHubWorkerBuilder capturedBuilder = null;
        RunTest(
            (IHostApplicationBuilder builder) => builder.ConfigureTaskHubWorker(b => capturedBuilder = b),
            (builder, returned) =>
            {
                returned.Should().BeSameAs(builder);
                capturedBuilder.Should().NotBeNull();
            });
    }

    [Fact]
    public void Configure_HostBuilder_TaskHubWorker_Callback2()
    {
        HostBuilderContext capturedContext = null;
        ITaskHubWorkerBuilder capturedBuilder = null;
        RunTest(
            (IHostBuilder builder) => builder.ConfigureTaskHubWorker((c, b) =>
            {
                capturedContext = c;
                capturedBuilder = b;
            }),
            (builder, returned) =>
            {
                returned.Should().BeSameAs(builder);
                capturedContext.Should().NotBeNull();
                capturedBuilder.Should().NotBeNull();
            });
    }

    [Fact]
    public void Configure_HostApplicationBuilder_TaskHubWorker_Callback2()
    {
        IHostApplicationBuilder capturedContext = null;
        ITaskHubWorkerBuilder capturedBuilder = null;
        RunTest(
            (IHostApplicationBuilder builder) => builder.ConfigureTaskHubWorker((c, b) =>
            {
                capturedContext = c;
                capturedBuilder = b;
            }),
            (builder, returned) =>
            {
                returned.Should().BeSameAs(builder);
                capturedContext.Should().NotBeNull();
                capturedBuilder.Should().NotBeNull();
            });
    }

    private static void RunTestException<TException>(Action<IHostBuilder> act)
        where TException : Exception
    {
        bool Act(IHostBuilder builder)
        {
            act(builder);
            return true;
        }

        TException exception = Capture<TException>(() => RunTest(Act, null));
        exception.Should().NotBeNull();
    }

    private static void RunTestException<TException>(Action<IHostApplicationBuilder> act)
        where TException : Exception
    {
        bool Act(IHostApplicationBuilder builder)
        {
            act(builder);
            return true;
        }

        TException exception = Capture<TException>(() => RunTest(Act, null));
        exception.Should().NotBeNull();
    }

    private static void RunTest<TResult>(
        Func<IHostBuilder, TResult> act,
        Action<IHostBuilder, TResult> verify)
    {
        HostBuilder builder = new();

        TResult result = act(builder);
        builder.Build();
        verify?.Invoke(builder, result);
    }

    private static void RunTest<TResult>(
        Func<IHostApplicationBuilder, TResult> act,
        Action<IHostApplicationBuilder, TResult> verify)
    {
        HostApplicationBuilder builder = new();

        TResult result = act(builder);
        builder.Build();
        verify?.Invoke(builder, result);
    }
}
