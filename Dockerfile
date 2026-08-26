# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files first for better Docker layer caching
COPY ["backend/Rentz.Intelligence.Api/Rentz.Intelligence.Api.csproj", "backend/Rentz.Intelligence.Api/"]
COPY ["backend/Rentz.Intelligence.Application/Rentz.Intelligence.Application.csproj", "backend/Rentz.Intelligence.Application/"]
COPY ["backend/Rentz.Intelligence.Domain/Rentz.Intelligence.Domain.csproj", "backend/Rentz.Intelligence.Domain/"]
COPY ["backend/Rentz.Intelligence.Infrastructure/Rentz.Intelligence.Infrastructure.csproj", "backend/Rentz.Intelligence.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "backend/Rentz.Intelligence.Api/Rentz.Intelligence.Api.csproj"

# Copy source code
COPY backend/ backend/

# Publish API
RUN dotnet publish \
    "backend/Rentz.Intelligence.Api/Rentz.Intelligence.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Render expects the application to listen on PORT.
# Render's default PORT is 10000.
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Rentz.Intelligence.Api.dll"]