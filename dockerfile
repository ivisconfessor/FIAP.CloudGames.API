FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar tudo
COPY . .

# Restaurar usando o arquivo .sln
RUN dotnet restore FIAP.CloudGames.API.sln

# Publicar o projeto específico
RUN dotnet publish src/ -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FIAP.CloudGames.API.dll"]