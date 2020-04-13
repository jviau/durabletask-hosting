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
        public void AddSucceeds()
        {
            // arrange
            var activity = TaskActivityDescriptor.Singleton<TestActivity>();
            var descriptors = new TaskHubCollection();

            // act
            bool result = descriptors.Add(activity);

            // assert
            result.Should().BeTrue();
            descriptors.Should().HaveCount(1);
            descriptors.Single().Should().Be(activity);
        }

        [Fact]
        public void AddDuplicate()
        {
            // arrange
            var activity = TaskActivityDescriptor.Singleton<TestActivity>();
            var descriptors = new TaskHubCollection();

            // act
            descriptors.Add(activity);
            bool result = descriptors.Add(activity);

            // assert
            result.Should().BeFalse();
            descriptors.Should().HaveCount(1);
            descriptors.Single().Should().Be(activity);
        }

        [Fact]
        public void GetByName()
        {
            // arrange
            var activity = TaskActivityDescriptor.Singleton<TestActivity>();
            var descriptors = new TaskHubCollection()
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
        public void GetByName2()
        {
            // arrange
            var activity = TaskActivityDescriptor.Singleton<TestActivity>();
            var activity2 = TaskActivityDescriptor.Singleton<TestActivity2>();
            var descriptors = new TaskHubCollection()
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
