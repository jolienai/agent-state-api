# ----------- Build Stage -----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app

# Copy solution and project files
COPY . ./
COPY AgentState.Api/*.csproj AgentState.Api/
COPY AgentState.Application/*.csproj AgentState.Application/
COPY AgentState.Api.Tests/*.csproj AgentState.Api.Tests/
COPY AgentState.Application.Tests/*.csproj AgentState.Application.Tests/

# Restore dependencies
RUN dotnet restore AgentState.sln

# Copy all source
COPY . ./

# Publish the app (not the tests)
RUN dotnet publish AgentState.Api/AgentState.Api.csproj -c Release -o /app/out

# ----------- Runtime Stage -----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

ENTRYPOINT ["dotnet", "AgentState.Api.dll"]