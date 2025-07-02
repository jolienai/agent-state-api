using AgentState.Application.Features.CallCenter;
using AgentState.Domain.Constants;
using FluentValidation.TestHelper;

namespace AgentState.Application.Tests.Features.CallCenter;

public class CallCenterEventCommandValidatorTests
{
    private readonly CallCenterEventCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_AgentId_Is_Empty()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "",
            AgentName: "",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: "",
            QueueIds: ["1"]
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AgentId);
    }

    [Fact]
    public void Should_Have_Error_When_AgentName_Is_Empty()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: "",
            QueueIds: ["1"]
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AgentName);
    }

    [Fact]
    public void Should_Have_Error_When_AgentName_Exceeds_MaxLength()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: new string('A', 101),
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: "",
            QueueIds: ["1"]
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.AgentName);
    }

    [Fact]
    public void Should_Have_Error_When_Action_Is_Empty()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: "",
            QueueIds: ["1"]
        );
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Action);
    }

    [Fact]
    public void Should_Have_Error_When_Action_Is_Invalid()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: "InvalidAction",
            QueueIds: ["1"]
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Action);
    }

    [Fact]
    public void Should_Have_Error_When_QueueIds_Is_Null()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: Activity.AllowedActivity[0],
            QueueIds: null
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QueueIds);
    }

    [Fact]
    public void Should_Have_Error_When_QueueIds_Is_Empty()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: Activity.AllowedActivity[0],
            QueueIds: []
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.QueueIds);
    }

    [Fact]
    public void Should_Have_Error_When_Timestamp_Is_In_The_Future()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(1), 
            Action: Activity.AllowedActivity[0],
            QueueIds: ["1", "2"]
        );
        
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TimestampUtc);
    }

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Model_Is_Valid()
    {
        var model = new CallCenterEventCommand
        (
            AgentId: "123",
            AgentName: "John Doe",
            TimestampUtc: DateTime.UtcNow.AddMinutes(-1), 
            Action: Activity.AllowedActivity[0],
            QueueIds: ["1", "2"]
        );

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}