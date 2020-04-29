using System;
using FluentAssertions;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Descriptors
{
    public class TypeDescriptorTests
    {
        [Fact]
        public void Ctor_ArgumentNull_NullType()
        {
            // arrange, act
            var ex = Capture<ArgumentNullException>(() => new TypeDescriptor<IInterfaceType>(null));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_Argument_AbstractType()
        {
            // arrange, act
            var ex = Capture<ArgumentException>(() => new TypeDescriptor<IInterfaceType>(typeof(AbstractType)));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_Argument_InterfaceType()
        {
            // arrange, act
            var ex = Capture<ArgumentException>(() => new TypeDescriptor<IInterfaceType>(typeof(IInterfaceType)));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_TypeSet_ExactType()
        {
            // arrange, act
            var descriptor = new TypeDescriptor<ConcreteType>(typeof(ConcreteType));

            // assert
            descriptor.Type.Should().Be(typeof(ConcreteType));
        }

        [Fact]
        public void Ctor_TypeSet_DerivedType()
        {
            // arrange, act
            var descriptor = new TypeDescriptor<IInterfaceType>(typeof(ConcreteType));

            // assert
            descriptor.Type.Should().Be(typeof(ConcreteType));
        }

        private class ConcreteType : AbstractType
        {
        }

        private abstract class AbstractType : IInterfaceType
        {
        }

        private interface IInterfaceType
        {
        }
    }
}
