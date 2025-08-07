FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TabuAI.API/TabuAI.API.csproj", "src/TabuAI.API/"]
COPY ["src/TabuAI.Application/TabuAI.Application.csproj", "src/TabuAI.Application/"]
COPY ["src/TabuAI.Domain/TabuAI.Domain.csproj", "src/TabuAI.Domain/"]
COPY ["src/TabuAI.Infrastructure/TabuAI.Infrastructure.csproj", "src/TabuAI.Infrastructure/"]
RUN dotnet restore "src/TabuAI.API/TabuAI.API.csproj"
COPY . .
WORKDIR "/src/src/TabuAI.API"
RUN dotnet build "TabuAI.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TabuAI.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TabuAI.API.dll"]