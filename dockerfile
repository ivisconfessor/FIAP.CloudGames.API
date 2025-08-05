FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar tudo e deixar o dotnet encontrar o projeto
COPY . .
RUN dotnet restore ./FIAP.CloudGames.API/src/
RUN dotnet publish ./FIAP.CloudGames.API/src/ -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FIAP.CloudGames.API.dll"]