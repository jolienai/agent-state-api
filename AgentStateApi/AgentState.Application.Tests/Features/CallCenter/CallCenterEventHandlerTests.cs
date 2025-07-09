using AgentState.Application.Features.CallCenter;
using AgentState.Application.Repositories;
using AgentState.Application.Shared;
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
    private readonly Mock<ICallCenterRepository> _agentRepositoryMock = new();
    private readonly Mock<ILogger<CallCenterEventHandler>> _loggerMock = new();
    private readonly Mock<ITimeProvider> _timeProviderMock = new();

    private readonly CallCenterEventHandler _handler;

    public CallCenterEventHandlerTests()
    {
        _handler = new CallCenterEventHandler(
            _validatorMock.Object,
            _agentRepositoryMock.Object,
            _timeProviderMock.Object,
            _loggerMock.Object
        );
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

        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult()); // simulate valid input

        _agentRepositoryMock.Setup(r => r.GetByIdAsync(command.AgentId, default))
            .ReturnsAsync((Agent)null!);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<AgentNotFoundException>(result.Error);
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

        var agent = new Agent
        {
            Id = "123",
            Skills = [new AgentSkill { QueueId = "3" }]
        };

        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _agentRepositoryMock.Setup(r => r.GetByIdAsync(command.AgentId, default))
            .ReturnsAsync(agent);

        _agentRepositoryMock.Setup(r => r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        
        // Skills should be updated
        Assert.Equal(2, agent.Skills.Count);
        Assert.Contains(agent.Skills, s => s.QueueId == "1");
        Assert.Contains(agent.Skills, s => s.QueueId == "2");
        Assert.DoesNotContain(agent.Skills, s => s.QueueId == "3");

        // State should be set correctly
        Assert.Equal(Domain.Enums.AgentStateEnum.OnCall.ToString(), agent.State);
    }
    
    [Theory]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    public async Task HandleAsync_Should_SetStateToOnLunch_WhenActionIsStartDndAndTimeIsLunch(int hour)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var lunchTime = now.Date.AddHours(hour);

        _timeProviderMock.Setup(x => x.UtcNow).Returns(lunchTime);
        
        var command = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: lunchTime,
            Action: Activity.StartDoNoDisturb,
            QueueIds: ["1"]
        );

        var agent = new Agent
        {
            Id = "123",
            Skills = []
        };

        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _agentRepositoryMock.Setup(r => r.GetByIdAsync(command.AgentId, default))
            .ReturnsAsync(agent);

        _agentRepositoryMock.Setup(r => r.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.Equal(Domain.Enums.AgentStateEnum.OnLunch.ToString(), agent.State);
    }
}