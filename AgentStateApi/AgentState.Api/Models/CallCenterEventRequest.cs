using System.Text.Json.Serialization;

namespace AgentState.Api.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public record CallCenterEventRequest(
    [property: JsonPropertyName("agentId")] string AgentId,
    [property: JsonPropertyName("agentName")] string AgentName,
    [property: JsonPropertyName("timestampUtc")] DateTime TimestampUtc,
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("queueIds")] List<string> QueueIds
);

