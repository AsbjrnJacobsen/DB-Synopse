FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID

WORKDIR /app
EXPOSE 8082

ENV ASPNETCORE_URLS=http://+:8082

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["OrderService/OrderService.csproj", "OrderService/"]
RUN dotnet restore "OrderService/OrderService.csproj"
COPY . .
WORKDIR "/src/OrderService"
RUN dotnet build "OrderService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "OrderService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
