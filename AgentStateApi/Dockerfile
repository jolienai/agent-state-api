# ----------- Build Stage -----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app

COPY . ./

RUN dotnet restore AgentState.sln
RUN dotnet publish AgentState.Api/AgentState.Api.csproj -c Release -o /app/out

#RUN dotnet publish AgentState.Api/AgentState.Api.csproj \
#    -c Release -o /app/out \
#    -r linux-musl-x64 --self-contained true /p:PublishTrimmed=true /p:EnableCompressionInSingleFile=true /p:PublishSingleFile=true

# ----------- Runtime Stage -----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

ENTRYPOINT ["dotnet", "AgentState.Api.dll"]