FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY FIAP.CloudGames.API/src/*.csproj ./FIAP.CloudGames.API/src/
RUN dotnet restore ./FIAP.CloudGames.API/src/
COPY . .
RUN dotnet publish ./FIAP.CloudGames.API/src/ -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FIAP.CloudGames.API.dll"]
