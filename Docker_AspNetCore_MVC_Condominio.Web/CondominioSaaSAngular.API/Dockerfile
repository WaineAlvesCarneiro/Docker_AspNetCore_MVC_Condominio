FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8083

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CondominioSaaSAngular.API/CondominioSaaSAngular.API.csproj", "CondominioSaaSAngular.API/"]
COPY ["CondominioSaaSAngular.Application/CondominioSaaSAngular.Application.csproj", "CondominioSaaSAngular.Application/"]
COPY ["CondominioSaaSAngular.Domain/CondominioSaaSAngular.Domain.csproj", "CondominioSaaSAngular.Domain/"]
COPY ["CondominioSaaSAngular.Configurations/CondominioSaaSAngular.Configurations.csproj", "CondominioSaaSAngular.Configurations/"]
COPY ["CondominioSaaSAngular.Infrastructure/CondominioSaaSAngular.Infrastructure.csproj", "CondominioSaaSAngular.Infrastructure/"]

RUN dotnet restore "./CondominioSaaSAngular.API/CondominioSaaSAngular.API.csproj"
COPY . .
WORKDIR "/src/CondominioSaaSAngular.API"
RUN dotnet build "./CondominioSaaSAngular.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CondominioSaaSAngular.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8083

ENTRYPOINT ["dotnet", "CondominioSaaSAngular.API.dll"]

HEALTHCHECK --interval=30s --timeout=10s --start-period=120s --retries=3 \
  CMD curl -f http://localhost:8083/health || exit 1