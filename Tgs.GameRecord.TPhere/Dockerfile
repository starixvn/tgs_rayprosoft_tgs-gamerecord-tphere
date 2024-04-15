#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.TPhere/Tgs.GameRecord.TPhere.csproj", "Tgs.GameRecord.TPhere/"]
COPY ["Tgs.GameRecord.TPhere.Common/Tgs.GameRecord.TPhere.Common.csproj", "Tgs.GameRecord.TPhere.Common/"]
COPY ["Tgs.GameRecord.TPhere.Core/Tgs.GameRecord.TPhere.Core.csproj", "Tgs.GameRecord.TPhere.Core/"]
RUN dotnet restore "./Tgs.GameRecord.TPhere/./Tgs.GameRecord.TPhere.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.TPhere"
RUN dotnet build "./Tgs.GameRecord.TPhere.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.TPhere.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.TPhere.dll"]