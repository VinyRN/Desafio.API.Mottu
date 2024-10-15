# Base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files for each referenced project
COPY ["Desafio.backend.Mottu/Desafio.backend.Mottu.csproj", "Desafio.backend.Mottu/"]
COPY ["Desafio.backend.Mottu.Dominio/Desafio.backend.Mottu.Dominio.csproj", "Desafio.backend.Mottu.Dominio/"]
COPY ["Desafio.backend.Mottu.Infra/Desafio.backend.Mottu.Infraestrutura.csproj", "Desafio.backend.Mottu.Infra/"]
COPY ["Desafio.backend.Mottu.Servico/Desafio.backend.Mottu.Servico.csproj", "Desafio.backend.Mottu.Servico/"]

# Run dotnet restore to restore dependencies for all projects
RUN dotnet restore "Desafio.backend.Mottu/Desafio.backend.Mottu.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet build "Desafio.backend.Mottu/Desafio.backend.Mottu.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Desafio.backend.Mottu/Desafio.backend.Mottu.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desafio.backend.Mottu.dll"]
