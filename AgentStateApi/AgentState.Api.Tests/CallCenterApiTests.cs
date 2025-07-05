using System.Net;
using System.Net.Http.Json;
using AgentState.Api.Models;
using AgentState.Api.Tests.Factories;
using AgentState.Api.Tests.Fixtures;
using AgentState.Domain.Constants;
using AgentState.Domain.Entities;
using AgentState.Domain.Enums;
using AgentState.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AgentState.Api.Tests;

public class CallCenterApiTests : IClassFixture<TestContainerFixture>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    
    public CallCenterApiTests(TestContainerFixture fixture)
    {
        _factory = new CustomWebApplicationFactory(fixture.Container.GetConnectionString());
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task PostCallCenterEvent_Should_Return_202Accepted()
    {
        // Arrange
        var agentId = Guid.NewGuid().ToString();
        
        // Seed Agent into the container DB
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Agents.Add(new Agent
            {
                Id = agentId,
                Name = "John",
                Skills = [],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                State = AgentStateEnum.OnCall.ToString(),
                CreatedBy = "test",
                UpdatedBy = "test"
            });
            await db.SaveChangesAsync();
        }
        
        // create the request
        var request = new CallCenterEventRequest(
            AgentId: agentId,
            AgentName: "John",
            TimestampUtc: DateTime.UtcNow,
            Action: Activity.CallStarted,
            QueueIds: ["1", "2"]
        );

        var content = JsonContent.Create(request);

        // Act
        var response = await _client.PostAsync("/events/call-center", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}