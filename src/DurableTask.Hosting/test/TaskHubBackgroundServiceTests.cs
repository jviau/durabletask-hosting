// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using DurableTask.Core;
using DurableTask.Hosting.Options;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using static DurableTask.TestHelpers;

namespace DurableTask.Hosting.Tests
{
    public class TaskHubBackgroundServiceTests
    {
        private static readonly ILogger<TaskHubBackgroundService> s_logger =
            Mock.Of<ILogger<TaskHubBackgroundService>>();

        private static readonly IOptions<TaskHubOptions> s_options =
            Mock.Of<IOptions<TaskHubOptions>>(
                x => x.Value == new TaskHubOptions());

        [Fact]
        public void Ctor_ArgumentNull()
        {
            // arrange, act
            ArgumentNullException ex = Capture<ArgumentNullException>(
                () => new TaskHubBackgroundService(null, null, null));

            // assert
            ex.Should().NotBeNull();
        }

        [Fact]
        public async Task StartAsync_TaskHubStarts()
        {
            // arrange
            Mock<IOrchestrationService> orchestrationMock = GetOrchestrationService();
            var taskHubWorker = new TaskHubWorker(orchestrationMock.Object);
            var service = new TaskHubBackgroundService(taskHubWorker, s_logger, s_options);

            // act
            await service.StartAsync(CancellationToken.None);

            // assert
            orchestrationMock.Verify(x => x.StartAsync(), Times.Once);
        }

        [Fact]
        public async Task StopAsync_TaskHubStops()
        {
            // arrange
            Mock<IOrchestrationService> orchestrationMock = GetOrchestrationService();
            var taskHubWorker = new TaskHubWorker(orchestrationMock.Object);
            var service = new TaskHubBackgroundService(taskHubWorker, s_logger, s_options);

            // act
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            // assert
            orchestrationMock.Verify(x => x.StopAsync(false), Times.Once);
        }

        [Fact]
        public async Task StopAsync_Cancelled()
        {
            // arrange
            Mock<IOrchestrationService> orchestrationMock = GetOrchestrationService();
            var cancellation = new CancellationTokenSource();
            orchestrationMock.Setup(x => x.StopAsync(false))
                .Callback(() => cancellation.Cancel())
                .Returns(Task.Delay(Timeout.Infinite));

            var taskHubWorker = new TaskHubWorker(orchestrationMock.Object);
            var service = new TaskHubBackgroundService(taskHubWorker, s_logger, s_options);

            // act
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(cancellation.Token);

            // assert
            orchestrationMock.Verify(x => x.StopAsync(false), Times.Once);
        }

        private static Mock<IOrchestrationService> GetOrchestrationService()
        {
            var orchestrationMock = new Mock<IOrchestrationService>();
            orchestrationMock.Setup(x => x.MaxConcurrentTaskOrchestrationWorkItems).Returns(1);
            orchestrationMock.Setup(x => x.MaxConcurrentTaskActivityWorkItems).Returns(1);
            orchestrationMock.Setup(x => x.TaskOrchestrationDispatcherCount).Returns(1);
            orchestrationMock.Setup(x => x.TaskActivityDispatcherCount).Returns(1);
            return orchestrationMock;
        }
    }
}
