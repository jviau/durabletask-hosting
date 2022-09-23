// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Reflection;
using DurableTask.Core;
using FluentAssertions;
using Moq;
using Xunit;

namespace DurableTask.DependencyInjection.Orchestrations.Tests;

public class WrapperOrchestrationContextTests
{
    [Fact]
    public async Task IsReplaying_Synced()
    {
        static object GetDefault(ParameterInfo parameter)
        {
            if (parameter.ParameterType == typeof(string))
            {
                return string.Empty;
            }

            if (parameter.ParameterType == typeof(Type))
            {
                return typeof(object);
            }

            return null;
        }

        foreach (MethodInfo method in typeof(WrapperOrchestrationContext).GetMethods())
        {
            if (!typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                continue;
            }

            Mock<OrchestrationContext> inner = new();
            SetIsReplaying(inner.Object, true);
            TaskCompletionSource<object> tcs = new();
            inner.SetReturnsDefault(tcs.Task);
            WrapperOrchestrationContext context = new(inner.Object);

            MethodInfo toInvoke = method.IsGenericMethodDefinition
                ? method.MakeGenericMethod(typeof(object)) : method;
            object[] parameters = toInvoke.GetParameters().Select(GetDefault).ToArray();
            var task = (Task)toInvoke.Invoke(context, parameters);

            context.IsReplaying.Should().BeTrue();
            SetIsReplaying(inner.Object, false);
            tcs.SetResult(null);

            await task;
            context.IsReplaying.Should().BeFalse();
        }
    }

    private static void SetIsReplaying(OrchestrationContext context, bool isReplaying)
    {
        typeof(OrchestrationContext).GetProperty("IsReplaying").SetValue(context, isReplaying);
    }
}
