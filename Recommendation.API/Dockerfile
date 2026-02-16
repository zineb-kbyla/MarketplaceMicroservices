FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Recommendation.API/Recommendation.API.csproj", "Recommendation.API/"]
RUN dotnet restore "Recommendation.API/Recommendation.API.csproj"
COPY . .
WORKDIR "/src/Recommendation.API"
RUN dotnet build "Recommendation.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Recommendation.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Recommendation.API.dll"]
