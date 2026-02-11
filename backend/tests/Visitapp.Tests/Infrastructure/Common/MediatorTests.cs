using Microsoft.Extensions.DependencyInjection;
using Moq;
using Visitapp.Application.Commands;
using Visitapp.Application.Commands.Auth;
using Visitapp.Application.Common;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Infrastructure.Common;
using Xunit;

namespace Visitapp.Tests.Infrastructure.Common
{
    public class MediatorTests
    {
        [Fact]
        public async Task SendAsync_WithResult_CallsCorrectHandler()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockHandler = new Mock<ICommandHandler<LoginCommand, AuthResponse>>();
            var expectedResponse = new AuthResponse
            {
                Token = "test-token",
                Message = "Success",
                User = new UserResponse { Email = "test@test.com" }
            };

            mockHandler.Setup(h => h.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            services.AddSingleton(mockHandler.Object);
            var serviceProvider = services.BuildServiceProvider();
            var mediator = new Mediator(serviceProvider);

            var command = new LoginCommand { Email = "test@test.com", Password = "password" };

            // Act
            var result = await mediator.SendAsync(command);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-token", result.Token);
            Assert.Equal("Success", result.Message);
            mockHandler.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendAsync_WithoutResult_CallsCorrectHandler()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockHandler = new Mock<ICommandHandler<TestCommand, Unit>>();

            mockHandler.Setup(h => h.HandleAsync(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            services.AddSingleton(mockHandler.Object);
            var serviceProvider = services.BuildServiceProvider();
            var mediator = new Mediator(serviceProvider);

            var command = new TestCommand();

            // Act
            await mediator.SendAsync(command);

            // Assert
            mockHandler.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendAsync_HandlerNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var mediator = new Mediator(serviceProvider);

            var command = new LoginCommand { Email = "test@test.com", Password = "password" };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync(command));
        }

        [Fact]
        public async Task SendAsync_HandlerThrowsException_PropagatesException()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockHandler = new Mock<ICommandHandler<LoginCommand, AuthResponse>>();
            var expectedException = new UnauthorizedAccessException("Invalid credentials");

            mockHandler.Setup(h => h.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            services.AddSingleton(mockHandler.Object);
            var serviceProvider = services.BuildServiceProvider();
            var mediator = new Mediator(serviceProvider);

            var command = new LoginCommand { Email = "test@test.com", Password = "wrong" };

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => mediator.SendAsync(command));
            Assert.Equal("Invalid credentials", thrownException.Message);
        }

        [Fact]
        public async Task SendAsync_CancellationRequested_PropagatesCancellation()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockHandler = new Mock<ICommandHandler<LoginCommand, AuthResponse>>();
            
            mockHandler.Setup(h => h.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .Returns((LoginCommand cmd, CancellationToken ct) => 
                {
                    ct.ThrowIfCancellationRequested();
                    return Task.FromResult(new AuthResponse());
                });

            services.AddSingleton(mockHandler.Object);
            var serviceProvider = services.BuildServiceProvider();
            var mediator = new Mediator(serviceProvider);

            var command = new LoginCommand { Email = "test@test.com", Password = "password" };
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => mediator.SendAsync(command, cts.Token));
        }
    }

    // Test command for testing purposes
    public class TestCommand : ICommand<Unit>
    {
    }
}