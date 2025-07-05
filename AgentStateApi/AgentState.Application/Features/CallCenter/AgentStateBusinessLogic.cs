using AgentState.Application.Shared;
using AgentState.Domain.Constants;

namespace AgentState.Application.Features.CallCenter;

public class AgentStateBusinessLogic
{
    public static Domain.Enums.AgentStateEnum State(CallCenterEventCommand request)
    {
        switch (request.Action)
        {
            case Activity.CallStarted:
                return Domain.Enums.AgentStateEnum.OnCall;
            case Activity.StartDoNoDisturb:
            {
                if (request.TimestampUtc.IsLunchTime())
                    return Domain.Enums.AgentStateEnum.OnLunch;

                // not sure when not lunch time what should return
                return Domain.Enums.AgentStateEnum.OnCall;
            }
        }

        // this should never have happened as it is validating the action to be one of the allowed actions
        throw new Exception("Not possible do determine the agent state");
    }
}