FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Cloudot.Migrator/Cloudot.Migrator.csproj", "src/Cloudot.Migrator/"]
RUN dotnet restore "src/Cloudot.Migrator/Cloudot.Migrator.csproj"
COPY . .
WORKDIR "/src/src/Cloudot.Migrator"
RUN dotnet build "./Cloudot.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cloudot.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cloudot.Migrator.dll"]
