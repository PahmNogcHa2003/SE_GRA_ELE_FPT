# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution
COPY src/backend/backend.sln .

# Copy các project chính (không copy mấy project test)
COPY src/backend/APIUserLayer/APIUserLayer.csproj APIUserLayer/
COPY src/backend/Application/Application.csproj Application/
COPY src/backend/Infrastructure/Infrastructure.csproj Infrastructure/
COPY src/backend/Domain/Domain.csproj Domain/

# Restore dependencies (chỉ những project này)
RUN dotnet restore APIUserLayer/APIUserLayer.csproj

# Copy toàn bộ source code backend
COPY src/backend/ .

# Build và publish APIUserLayer
RUN dotnet publish APIUserLayer/APIUserLayer.csproj -c Release -o /app/out

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "APIUserLayer.dll"]
