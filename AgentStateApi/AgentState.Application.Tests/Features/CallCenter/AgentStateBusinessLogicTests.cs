using AgentState.Application.Features.CallCenter;
using AgentState.Domain.Constants;

namespace AgentState.Application.Tests.Features.CallCenter;

public class AgentStateBusinessLogicTests
{
    private readonly AgentStateBusinessLogic _logic = new();

    [Fact]
    public void State_Should_Return_OnCall_When_Action_Is_CallStarted()
    {
        // Arrange
        var command = new CallCenterEventCommand(
            "agent-1", "Agent Name", DateTime.UtcNow, Activity.CallStarted, new List<string> { "queue1" });

        // Act
        var result = AgentStateBusinessLogic.State(command);

        // Assert
        Assert.Equal(Domain.Enums.AgentStateEnum.OnCall, result);
    }

    [Fact]
    public void State_Should_Return_OnLunch_When_Action_Is_StartDoNotDisturb_And_Timestamp_Is_Lunch()
    {
        // Arrange
        var lunchTime =
            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 12, 0, 0); // 12:00 PM
        var command = new CallCenterEventCommand(
            "agent-1", "Agent Name", lunchTime, Activity.StartDoNoDisturb, new List<string> { "queue1" });

        // Act
        var result = AgentStateBusinessLogic.State(command);

        // Assert
        Assert.Equal(Domain.Enums.AgentStateEnum.OnLunch, result);
    }

    [Fact]
    public void State_Should_Return_OnCall_When_Action_Is_StartDoNotDisturb_And_Not_Lunch()
    {
        // Arrange
        var nonLunchTime =
            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 10, 0, 0); // 10:00 AM
        var command = new CallCenterEventCommand(
            "agent-1", "Agent Name", nonLunchTime, Activity.StartDoNoDisturb, new List<string> { "queue1" });

        // Act
        var result = AgentStateBusinessLogic.State(command);

        // Assert
        Assert.Equal(Domain.Enums.AgentStateEnum.OnCall, result);
    }

    [Fact]
    public void State_Should_Throw_Exception_When_Action_Is_Invalid()
    {
        // Arrange
        var command = new CallCenterEventCommand(
            "agent-1", "Agent Name", DateTime.UtcNow, "UnknownAction", new List<string> { "queue1" });

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => AgentStateBusinessLogic.State(command));
        Assert.Equal("Not possible do determine the agent state", ex.Message);
    }
}