using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Tests
{
    public class TaskHubCollectionTests
    {
        [Fact]
        public void EnumeratorOfT_ContainsAll()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity });

            // act
            IEnumerator<NamedTypeDescriptor<TaskActivity>> enumerator = descriptors.GetEnumerator();

            // assert
            enumerator.Should().NotBeNull();
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be(activity);
            enumerator.MoveNext().Should().BeFalse();
            ((IEnumerable)descriptors).GetEnumerator().Should().NotBeNull();
        }

        [Fact]
        public void Enumerator_ContainsAll()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity });

            // act
            IEnumerator enumerator = ((IEnumerable)descriptors).GetEnumerator();

            // assert
            enumerator.Should().NotBeNull();
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().Be(activity);
            enumerator.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void Get_Null()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity });

            // act
            Type actual = descriptors["DNE", string.Empty];

            // assert
            actual.Should().BeNull();
        }

        [Fact]
        public void Get_ByName()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity });

            // act
            Type actual = descriptors[activity.Name, activity.Version];

            // assert
            actual.Should().NotBeNull();
            actual.Should().Be(typeof(TestActivity));
        }

        [Fact]
        public void Get_ByName2()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var activity2 = new TaskActivityDescriptor(typeof(TestActivity2));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity, activity2 });

            // act
            Type actual = descriptors[activity2.Name, activity2.Version];

            // assert
            actual.Should().NotBeNull();
            actual.Should().Be(typeof(TestActivity2));
        }

        [Fact]
        public void Get_Cached()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity });

            // act
            Type actual = descriptors[activity.Name, activity.Version];
            Type actual2 = descriptors[activity.Name, activity.Version];

            // assert
            actual.Should().NotBeNull();
            actual.Should().Be(typeof(TestActivity));
            actual.Should().Be(actual2);
        }

        [Fact]
        public void Count_Zero()
        {
            // arrange
            var descriptors = new TaskHubCollection<TaskActivity>(Enumerable.Empty<TaskActivityDescriptor>());

            // act
            int count = descriptors.Count;

            // assert
            count.Should().Be(0);
        }

        [Fact]
        public void Count_Multiple()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var activity2 = new TaskActivityDescriptor(typeof(TestActivity2));
            var descriptors = new TaskHubCollection<TaskActivity>(new[] { activity, activity2 });

            // act
            int count = descriptors.Count;

            // assert
            count.Should().Be(2);
        }

        private class TestActivity : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }

        private class TestActivity2 : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }
    }
}
