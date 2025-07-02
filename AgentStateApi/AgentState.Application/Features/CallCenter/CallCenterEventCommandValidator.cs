using AgentState.Domain.Constants;
using FluentValidation;

namespace AgentState.Application.Features.CallCenter;

public class CallCenterEventCommandValidator : AbstractValidator<CallCenterEventCommand>
{
    public CallCenterEventCommandValidator()
    {
        RuleFor(x => x.AgentId)
            .NotEmpty().WithMessage("AgentId is required.");

        RuleFor(x => x.AgentName)
            .NotEmpty().WithMessage("AgentName is required.")
            .MaximumLength(100);

        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action is required.")
            .Must(a => Activity.AllowedActivity.Contains(a))
            .WithMessage($"Action must be one of: {string.Join(", ", Activity.AllowedActivity)}");

        RuleFor(x => x.QueueIds)
            .NotNull().WithMessage("QueueIds must not be null.")
            .Must(q => q.Count > 0).WithMessage("QueueIds must contain at least one item.");

        RuleFor(x => x.TimestampUtc)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Timestamp cannot be in the future.");
    }
}