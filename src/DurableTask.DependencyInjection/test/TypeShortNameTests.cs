// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Tests
{
    public class TypeShortNameTests
    {
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(TaskActivity))]
        [InlineData(typeof(TestNestedSimple))]
        public void Ctor_SimpleType(Type type)
        {
            var typeName = new TypeShortName(type);
            VerifyTypeNameWithAssembly(typeName, type);
        }

        [Theory]
        [InlineData(typeof(List<>))]
        [InlineData(typeof(ObjectCreator<>))]
        [InlineData(typeof(KeyValuePair<,>))]
        [InlineData(typeof(TestNestedGeneric<>))]
        public void Ctor_OpenGenericType(Type type)
        {
            var typeName = new TypeShortName(type);
            VerifyTypeNameWithAssembly(typeName, type);
        }

        [Theory]
        [InlineData(typeof(List<object>), "System.Collections.Generic.List`1[[System.Object, System.Private.CoreLib]], System.Private.CoreLib")]
        [InlineData(typeof(ObjectCreator<TaskActivity>), "DurableTask.Core.ObjectCreator`1[[DurableTask.Core.TaskActivity, DurableTask.Core]], DurableTask.Core")]
        [InlineData(typeof(KeyValuePair<string, TaskActivity>), "System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib]|[DurableTask.Core.TaskActivity, DurableTask.Core]], System.Private.CoreLib")]
        [InlineData(typeof(TestNestedGeneric<string>), "DurableTask.DependencyInjection.Tests.TypeShortNameTests+TestNestedGeneric`1[[System.String, System.Private.CoreLib]], Vio.DurableTask.DependencyInjection.Tests")]
        [InlineData(typeof(ObjectCreator<TestNestedSimple>), "DurableTask.Core.ObjectCreator`1[[DurableTask.DependencyInjection.Tests.TypeShortNameTests+TestNestedSimple, Vio.DurableTask.DependencyInjection.Tests]], DurableTask.Core")]
        [InlineData(typeof(ObjectCreator<Dictionary<string, object>>), "DurableTask.Core.ObjectCreator`1[[System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib]|[System.Object, System.Private.CoreLib]], System.Private.CoreLib]], DurableTask.Core")]
        public void Ctor_ClosedGenericType(Type type, string typeNameString)
        {
            var typeName = new TypeShortName(type);
            VerifyTypeNameWithAssembly(typeName, type, typeNameString);
        }

        [Theory]
        [InlineData("System.Object, System.Private.CoreLib", typeof(object))]
        [InlineData("DurableTask.Core.TaskActivity, DurableTask.Core", typeof(TaskActivity))]
        public void Ctor_SimpleString(string typeNameString, Type type)
        {
            var typeName = new TypeShortName(typeNameString);
            VerifyTypeNameWithAssembly(typeName, type);
        }

        [Theory]
        [InlineData("System.Object", typeof(object))]
        [InlineData("DurableTask.Core.TaskActivity", typeof(TaskActivity))]
        public void Ctor_SimpleString_NoAssembly(string typeNameString, Type type)
        {
            var typeName = new TypeShortName(typeNameString);
            VerifyTypeNameWithoutAssembly(typeName, type);
        }

        [Theory]
        [InlineData("System.Collections.Generic.List`1, System.Private.CoreLib", typeof(List<>))]
        [InlineData("DurableTask.Core.ObjectCreator`1, DurableTask.Core", typeof(ObjectCreator<>))]
        public void Ctor_OpenGenericString(string typeNameString, Type type)
        {
            var typeName = new TypeShortName(typeNameString);
            VerifyTypeNameWithAssembly(typeName, type);
        }

        [Theory]
        [InlineData("System.Collections.Generic.List`1", typeof(List<>))]
        [InlineData("DurableTask.Core.ObjectCreator`1", typeof(ObjectCreator<>))]
        public void Ctor_OpenGenericString_NoAssembly(string typeNameString, Type type)
        {
            var typeName = new TypeShortName(typeNameString);
            VerifyTypeNameWithoutAssembly(typeName, type);
        }

        [Theory]
        [InlineData("System.Collections.Generic.List`1[[System.Object, System.Private.CoreLib]], System.Private.CoreLib", typeof(List<object>))]
        [InlineData("DurableTask.Core.ObjectCreator`1[[DurableTask.Core.TaskActivity, DurableTask.Core]], DurableTask.Core", typeof(ObjectCreator<TaskActivity>))]
        [InlineData("System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib]|[DurableTask.Core.TaskActivity, DurableTask.Core]], System.Private.CoreLib", typeof(KeyValuePair<string, TaskActivity>))]
        [InlineData("DurableTask.Core.ObjectCreator`1[[System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib]|[System.Object, System.Private.CoreLib]], System.Private.CoreLib]], DurableTask.Core", typeof(ObjectCreator<Dictionary<string, object>>))]
        public void Ctor_ClosedGenericString(string typeNameString, Type type)
        {
            var typeName = new TypeShortName(typeNameString);
            VerifyTypeNameWithAssembly(typeName, type, typeNameString);
        }

        [Theory]
        [InlineData("System.Collections.Generic.List`1[[System.Object, System.Private.CoreLib]]", typeof(List<object>))]
        [InlineData("DurableTask.Core.ObjectCreator`1[[DurableTask.Core.TaskActivity, DurableTask.Core]]", typeof(ObjectCreator<TaskActivity>))]
        [InlineData("System.Collections.Generic.KeyValuePair`2[[System.String, System.Private.CoreLib]|[DurableTask.Core.TaskActivity, DurableTask.Core]]", typeof(KeyValuePair<string, TaskActivity>))]
        public void Ctor_ClosedGenericString_NoAssembly(string typeNameString, Type type)
        {
            var typeName = new TypeShortName(typeNameString);
            VerifyTypeNameWithoutAssembly(typeName, type, typeNameString);
        }

        [Fact]
        public void Ctor_NestedGenericString()
        {
            string typeNameString = "System.Collections.Generic.List`1[[DurableTask.Core.ObjectCreator`1[[DurableTask.Core.Task" +
                "Activity, DurableTask.Core]], DurableTask.Core]], System.Private.CoreLib";
            string genericTypeNameString = "DurableTask.Core.ObjectCreator`1[[DurableTask.Core.Task" +
                "Activity, DurableTask.Core]], DurableTask.Core";
            Type type = typeof(List<ObjectCreator<TaskActivity>>);
            Type genericType = typeof(ObjectCreator<TaskActivity>);
            var typeName = new TypeShortName(typeNameString);

            VerifyTypeNameWithAssembly(typeName, type, typeNameString);
            VerifyTypeNameWithAssembly(typeName.GenericParams.First(), genericType, genericTypeNameString);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(ObjectCreator<>))]
        [InlineData(typeof(ObjectCreator<TestNestedSimple>))]
        [InlineData(typeof(ObjectCreator<TaskActivity>))]
        [InlineData(typeof(TestNestedGeneric<string>))]
        public void Load_AssemblyNameSet(Type type)
        {
            var typeName1 = new TypeShortName(type);
            Type actual = typeName1.Load();
            actual.Should().Be(type);

            var typeName2 = new TypeShortName(typeName1.ToString());
            actual = typeName2.Load();
            actual.Should().Be(type);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(ObjectCreator<>))]
        [InlineData(typeof(ObjectCreator<TestNestedSimple>))]
        [InlineData(typeof(ObjectCreator<TaskActivity>))]
        [InlineData(typeof(TestNestedGeneric<string>))]
        public void Load_AssemblyNameProvided(Type type)
        {
            var typeName1 = new TypeShortName(type);
            Type actual = typeName1.Load(type.Assembly);
            actual.Should().Be(type);

            var typeName2 = new TypeShortName(typeName1.ToString(false));
            actual = typeName2.Load(type.Assembly);
            actual.Should().Be(type);
        }

        private static void VerifyTypeNameWithAssembly(
            TypeShortName typeName, Type expectedType, string expectedTypeString = null)
        {
            string name = expectedType.FullName;
            if (expectedType.IsConstructedGenericType)
            {
                name = name[..name.IndexOf('[')];
            }

            string assemblyName = expectedType.Assembly.GetName().Name;
            typeName.Name.Should().Be(name);
            typeName.AssemblyName.Should().Be(assemblyName);
            expectedTypeString ??= $"{name}, {assemblyName}";

            if (expectedType.IsConstructedGenericType)
            {
                typeName.GenericParams.Should().HaveSameCount(expectedType.GetGenericArguments());
            }
            else
            {
                typeName.GenericParams.Should().BeEmpty();
            }

            typeName.ToString().Should().Be(expectedTypeString);
            typeName.ToString(false).Should().Be(expectedTypeString[..expectedTypeString.LastIndexOf(',')]);
        }

        private static void VerifyTypeNameWithoutAssembly(
            TypeShortName typeName, Type expectedType, string expectedTypeString = null)
        {
            string name = expectedType.FullName;
            if (expectedType.IsConstructedGenericType)
            {
                name = name[..name.IndexOf('[')];
            }

            expectedTypeString ??= name;
            typeName.Name.Should().Be(name);
            typeName.AssemblyName.Should().BeNull();

            if (expectedType.IsConstructedGenericType)
            {
                typeName.GenericParams.Should().HaveSameCount(expectedType.GetGenericArguments());
            }
            else
            {
                typeName.GenericParams.Should().BeEmpty();
            }

            typeName.ToString().Should().Be(expectedTypeString);
            typeName.ToString(false).Should().Be(expectedTypeString);
        }

        private class TestNestedSimple
        {
        }

        private class TestNestedGeneric<T>
        {
        }
    }
}
