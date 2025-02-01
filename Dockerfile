FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5077
EXPOSE 5078

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProvEditorNET.csproj", "./"]
RUN dotnet restore "ProvEditorNET.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ProvEditorNET.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProvEditorNET.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5077
ENTRYPOINT ["dotnet", "ProvEditorNET.dll"]
