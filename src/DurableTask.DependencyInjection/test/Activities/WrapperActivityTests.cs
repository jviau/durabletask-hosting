using System;
using System.Collections.Generic;
using System.Text;
using DurableTask.Core;
using DurableTask.DependencyInjection.Activities;
using FluentAssertions;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.DependencyInjection.Tests.Activities
{
    public class WrapperActivityTests
    {
        [Fact]
        public void Ctor_ArgumentNull()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => new WrapperActivity(null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_ArgumentInvalidType()
        {
            // arrange, act
            ArgumentException ex = Capture<ArgumentException>(
                () => new WrapperActivity(typeof(TaskActivity)));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public void Ctor_InnerActivityTypeSet()
        {
            // arrange, act
            var activity = new WrapperActivity(typeof(TestActivity));

            // assert
            activity.InnerActivityType.Should().Be(typeof(TestActivity));
            activity.InnerActivity.Should().BeNull();
        }

        private class TestActivity : TaskActivity
        {
            public override string Run(TaskContext context, string input)
            {
                throw new NotImplementedException();
            }
        }
    }
}
