# Etapa base para a execução da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar o arquivo nuget.config para o contêiner
COPY nuget.config /root/.nuget/NuGet/

# Copiar todos os arquivos de projeto (.csproj) para restaurar as dependências
COPY ["Desafio.backend.Mottu/Desafio.backend.Mottu.csproj", "Desafio.backend.Mottu/"]
COPY ["Desafio.backend.Mottu.Dominio/Desafio.backend.Mottu.Dominio.csproj", "Desafio.backend.Mottu.Dominio/"]
COPY ["Desafio.backend.Mottu.Infra/Desafio.backend.Mottu.Infraestrutura.csproj", "Desafio.backend.Mottu.Infra/"]
COPY ["Desafio.backend.Mottu.Servico/Desafio.backend.Mottu.Servico.csproj", "Desafio.backend.Mottu.Servico/"]
COPY ["Desafio.backend.Mottu.Queue/Desafio.backend.Mottu.Queue.csproj", "Desafio.backend.Mottu.Queue/"]

# Restaurar dependências
RUN dotnet restore "Desafio.backend.Mottu/Desafio.backend.Mottu.csproj" --disable-parallel --no-cache

# Copiar o restante do código-fonte
COPY . .

# Compilar o projeto
RUN dotnet build "Desafio.backend.Mottu/Desafio.backend.Mottu.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Desafio.backend.Mottu/Desafio.backend.Mottu.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final - Executar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desafio.backend.Mottu.dll"]
