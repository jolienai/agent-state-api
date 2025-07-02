namespace AgentState.Domain.Constants;

public static class Activity
{
    public const string StartDoNoDisturb = "START_DO_NOT_DISTURB";
    public const string CallStarted = "CALL_STARTED";
    
    public static readonly string[] AllowedActivity = [StartDoNoDisturb, CallStarted];
}