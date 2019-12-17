# Base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 50051

# Restore dependencies
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS dependencies
WORKDIR /src
COPY . .
RUN dotnet restore

# Build application
FROM dependencies AS build
WORKDIR /src
RUN dotnet build .

# Publish application
FROM build AS publish
RUN dotnet publish -o /app

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SwaggerDiff.dll"]