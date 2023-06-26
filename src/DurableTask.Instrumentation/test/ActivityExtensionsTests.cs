// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection;
using FluentAssertions;

namespace DurableTask.Instrumentation.Tests;

public class ActivityExtensionsTests
{
    private static readonly AssemblyName s_assemblyName = typeof(ActivityExtensionsTests).Assembly.GetName();
    private static readonly ActivitySource s_source = new(s_assemblyName.Name, "0.1");

    [Fact]
    public void SetSource_Succeeds()
    {
        Activity activity = new("Test");
        activity.SetSource(s_source);
        activity.Source.Should().Be(s_source);
    }

    [Fact]
    public void SetKind_Succeeds()
    {
        Activity activity = new("Test");
        activity.SetKind(ActivityKind.Server);
        activity.Kind.Should().Be(ActivityKind.Server);
    }

    [Fact]
    public void SetException_Succeeds()
    {
        InvalidOperationException expected = new("test message");
        Activity activity = new("Test");
        activity.SetException(expected);

        activity.Status.Should().Be(ActivityStatusCode.Error);
        activity.StatusDescription.Should().Be("test message");

        Exception actual = activity.GetException();
        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void SetId_Succeeds()
    {
        Activity activity = new("Test");

        activity.Start();
        string expected = GetId();
        activity.SetId(expected);

        activity.Id.Should().Be(expected);
    }

    [Fact]
    public void SetTraceId_Succeeds()
    {
        Activity activity = new("Test");

        activity.Start();
        ActivityTraceId expected = GetTraceId();
        activity.SetTraceId(expected.ToHexString());

        activity.TraceId.Should().Be(expected);
    }

    [Fact]
    public void SetSpanId_Succeeds()
    {
        Activity activity = new("Test");

        activity.Start();
        ActivitySpanId expected = GetSpanId();
        activity.SetSpanId(expected.ToHexString());

        activity.SpanId.Should().Be(expected);
    }

    private static string GetId()
    {
        using Activity activity = new("generate_id");
        activity.Start();
        return activity.Id;
    }

    private static ActivityTraceId GetTraceId()
    {
        using Activity activity = new("generate_trace_id");
        activity.Start();
        return activity.TraceId;
    }

    private static ActivitySpanId GetSpanId()
    {
        using Activity activity = new("generate_span_id");
        activity.Start();
        return activity.SpanId;
    }
}
