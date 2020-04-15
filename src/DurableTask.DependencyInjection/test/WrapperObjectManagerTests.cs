using System;
using DurableTask.Core;
using FluentAssertions;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests
{
    public class ServiceObjectManagerTests
    {
        [Fact]
        public void CtorArgumentNull1()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => new WrapperObjectManager<object>(null, _ => null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CtorArgumentNull2()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => new WrapperObjectManager<object>(
                    Mock.Of<ITaskObjectCollection<object>>(), null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void AddNotSupported()
        {
            // arrange
            var manager = new WrapperObjectManager<object>(
                Mock.Of<ITaskObjectCollection<object>>(), _ => new object());

            // act
            NotSupportedException ex = Capture<NotSupportedException>(
                () => manager.Add(Mock.Of<ObjectCreator<object>>()));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void GetUsesFactory()
        {
            // arrange
            string name = "WrapperObjectManagerTests_Name";
            string version = "WrapperObjectManagerTests_Version";

            var descriptors = new Mock<ITaskObjectCollection<MyType>>();
            descriptors.Setup(x => x[name, version]).Returns(typeof(MyType));

            var manager = new WrapperObjectManager<MyType>(descriptors.Object, t => new MyTypeWrapped(t));

            // act
            MyType actual = manager.GetObject(name, version);
            var wrapper = actual as MyTypeWrapped;

            // assert
            wrapper.Should().NotBeNull();
            wrapper.Type.Should().Be(typeof(MyType));
        }

        public class MyType
        {
        }

        private class MyTypeWrapped : MyType
        {
            public MyTypeWrapped(Type t)
            {
                Type = t;
            }

            public Type Type { get; }
        }
    }
}
