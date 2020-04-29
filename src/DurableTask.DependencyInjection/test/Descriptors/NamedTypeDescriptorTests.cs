using System;
using DurableTask.Core;
using FluentAssertions;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Descriptors
{
    public class NamedTypeDescriptorTests
    {
        private const string Name = "NamedTypeDescriptorTests_Name";
        private const string Version = "NamedTypeDescriptorTests_Version";

        [Fact]
        public void Ctor_ArgumentNull_NullType()
        {
            // arrange, act
            var ex = Capture<ArgumentNullException>(
                () => new NamedTypeDescriptor<IInterfaceType>(null));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_ArgumentNull_NullName()
        {
            // arrange, act
            var ex = Capture<ArgumentNullException>(
                () => new NamedTypeDescriptor<IInterfaceType>(typeof(ConcreteType), null, Version));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_ArgumentNull_NullVersion()
        {
            // arrange, act
            var ex = Capture<ArgumentNullException>(
                () => new NamedTypeDescriptor<IInterfaceType>(typeof(ConcreteType), Name, null));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_Argument_EmptyName()
        {
            // arrange, act
            var ex = Capture<ArgumentException>(
                () => new NamedTypeDescriptor<IInterfaceType>(typeof(ConcreteType), string.Empty, Version));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_Argument_AbstractType()
        {
            // arrange, act
            var ex = Capture<ArgumentException>(
                () => new NamedTypeDescriptor<IInterfaceType>(typeof(AbstractType)));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_Argument_InterfaceType()
        {
            // arrange, act
            var ex = Capture<ArgumentException>(
                () => new NamedTypeDescriptor<IInterfaceType>(typeof(IInterfaceType)));

            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_TypeSet_ExactType()
        {
            // arrange, act
            var descriptor = new NamedTypeDescriptor<ConcreteType>(typeof(ConcreteType));

            // assert
            descriptor.Type.Should().Be(typeof(ConcreteType));
            descriptor.Name.Should().Be(NameVersionHelper.GetDefaultName(typeof(ConcreteType)));
            descriptor.Version.Should().Be(NameVersionHelper.GetDefaultVersion(typeof(ConcreteType)));
        }

        [Fact]
        public void Ctor_TypeSet_DerivedType()
        {
            // arrange, act
            var descriptor = new NamedTypeDescriptor<IInterfaceType>(typeof(ConcreteType));

            // assert
            descriptor.Type.Should().Be(typeof(ConcreteType));
            descriptor.Name.Should().Be(NameVersionHelper.GetDefaultName(typeof(ConcreteType)));
            descriptor.Version.Should().Be(NameVersionHelper.GetDefaultVersion(typeof(ConcreteType)));
        }

        [Fact]
        public void Ctor_NameSet_VersionSet()
        {
            // arrange, act
            var descriptor = new NamedTypeDescriptor<IInterfaceType>(
                typeof(ConcreteType), Name, Version);

            // assert
            descriptor.Type.Should().Be(typeof(ConcreteType));
            descriptor.Name.Should().Be(Name);
            descriptor.Version.Should().Be(Version);
        }

        [Fact]
        public void Ctor_NameSet_VersionEmpty()
        {
            // arrange, act
            var descriptor = new NamedTypeDescriptor<IInterfaceType>(
                typeof(ConcreteType), Name, string.Empty);

            // assert
            descriptor.Type.Should().Be(typeof(ConcreteType));
            descriptor.Name.Should().Be(Name);
            descriptor.Version.Should().Be(string.Empty);
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
