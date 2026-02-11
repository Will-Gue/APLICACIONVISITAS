# Multi-stage build para optimizar tamaño
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5254

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Visitapp/Visitapp.csproj", "Visitapp/"]
COPY ["Visitapp.Tests/Visitapp.Tests.csproj", "Visitapp.Tests/"]
RUN dotnet restore "Visitapp/Visitapp.csproj"

COPY . .
WORKDIR "/src/Visitapp"
RUN dotnet build "Visitapp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Visitapp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar usuario no-root para seguridad
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Visitapp.dll"]