FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8085
ENV ASPNETCORE_URLS=http://+:8085

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ServiceDiscovery/ServiceDiscovery.csproj", "ServiceDiscovery/"]
RUN dotnet restore "ServiceDiscovery/ServiceDiscovery.csproj"
COPY . .
WORKDIR "/src/ServiceDiscovery"
RUN dotnet build "ServiceDiscovery.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ServiceDiscovery.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceDiscovery.dll"]
