using AgentState.Application.Features.CallCenter;
using AgentState.Application.Repositories;
using AgentState.Domain.Constants;
using AgentState.Domain.Entities;
using AgentState.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AgentState.Application.Tests.Features.CallCenter;

public class CallCenterEventHandlerTests
{
    private readonly Mock<IValidator<CallCenterEventCommand>> _validatorMock = new();
    private readonly Mock<IAgentRepository> _agentRepositoryMock = new();
    private readonly Mock<ILogger<CallCenterEventHandler>> _loggerMock = new();

    private readonly CallCenterEventHandler _handler;

    public CallCenterEventHandlerTests()
    {
        _handler = new CallCenterEventHandler(_validatorMock.Object, _agentRepositoryMock.Object, _loggerMock.Object);
    }
    
    
    [Fact]
    public async Task HandleAsync_Should_Return_Failure_When_Agent_Not_Found()
    {
        // Arrange
        var command = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(-1), 
            Action: Activity.AllowedActivity[0],
            QueueIds: ["1", "2"]
        );
        
        _agentRepositoryMock.Setup(r => r.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _agentRepositoryMock.Setup(r => r.GetByIdAsync(command.AgentId, default)).ReturnsAsync((Agent)null!);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<AgentNotFoundException>(result.Error);
        _agentRepositoryMock.Verify(r => r.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Success_When_All_Good()
    {
        // Arrange
        var command = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(-1),
            Action: Activity.CallStarted,
            QueueIds: ["1", "2"]
        );

        var agent = new Agent();
        _agentRepositoryMock.Setup(r => r.BeginTransactionAsync(default))
            .Returns(Task.CompletedTask);
        
        _agentRepositoryMock.Setup(r => r.GetByIdAsync(command.AgentId, default))
            .ReturnsAsync(agent);
        
        _agentRepositoryMock.Setup(r => r.SyncAgentSkillsAsync(agent, command.QueueIds!, default))
            .Returns(Task.CompletedTask);
        
        _agentRepositoryMock.Setup(r => r.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _agentRepositoryMock.Verify(r => r.CommitAsync(), Times.Once);
    }
}