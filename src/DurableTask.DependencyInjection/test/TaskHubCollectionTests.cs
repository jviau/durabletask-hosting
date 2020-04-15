using System;
using System.Linq;
using DurableTask.Core;
using FluentAssertions;
using Xunit;

namespace DurableTask.DependencyInjection.Tests
{
    public class TaskHubCollectionTests
    {
        [Fact]
        public void Add_Succeeds()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>();

            // act
            bool result = descriptors.Add(activity);

            // assert
            result.Should().BeTrue();
            descriptors.Should().HaveCount(1);
            descriptors.Single().Should().Be(activity);
        }

        [Fact]
        public void Add_Duplicate()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>();

            // act
            descriptors.Add(activity);
            bool result = descriptors.Add(activity);

            // assert
            result.Should().BeFalse();
            descriptors.Should().HaveCount(1);
            descriptors.Single().Should().Be(activity);
        }

        [Fact]
        public void Get_ByName()
        {
            // arrange
            var activity = new TaskActivityDescriptor(typeof(TestActivity));
            var descriptors = new TaskHubCollection<TaskActivity>()
            {
                activity,
            };

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
            var descriptors = new TaskHubCollection<TaskActivity>()
            {
                activity,
                activity2,
            };

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
            var descriptors = new TaskHubCollection<TaskActivity>()
            {
                activity,
            };

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
            var descriptors = new TaskHubCollection<TaskActivity>();

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
            var descriptors = new TaskHubCollection<TaskActivity>()
            {
                activity,
                activity2,
            };

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
