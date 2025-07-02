# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 8080 for the application.
EXPOSE 8080

# Configure ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://*:8080

ENTRYPOINT ["dotnet", "AgentState.Api.dll"]