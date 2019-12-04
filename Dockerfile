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




# # Base
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 50051

# # Restore dependencies
# FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS dependencies
# WORKDIR /src
# COPY . .
# RUN dotnet restore

# # Build application
# FROM dependencies AS build
# WORKDIR /src
# RUN dotnet build -c Release --no-restore .

# # Publish application
# FROM build AS publish
# RUN dotnet publish --no-restore --no-build -c Release -o /app

# # Final
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# ENTRYPOINT ["dotnet", "SwaggerDiff.dll"]




# base
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 50051

# #Install Datadog APM package
# RUN curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v1.9.0/datadog-dotnet-apm_1.9.0_amd64.deb
# RUN dpkg -i datadog-dotnet-apm_1.9.0_amd64.deb

# # restore dependencies
# FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS dependencies
# WORKDIR /src
# COPY UtilityService.sln ./
# COPY UtilityService/UtilityService.csproj UtilityService/
# COPY UtilityService.Data/UtilityService.Data.csproj UtilityService.Data/
# RUN dotnet restore -nowarn:msb3202,nu1503 -s http://templumnugetserver.azurewebsites.net/nuget -s https://api.nuget.org/v3/index.json UtilityService

# # build
# FROM dependencies as build
# ARG DOTNET_CONFIG=Release
# WORKDIR /src
# COPY . .
# RUN dotnet build -c ${DOTNET_CONFIG} --no-restore UtilityService

# # publish
# FROM build AS publish
# ARG DOTNET_CONFIG=Release
# WORKDIR UtilityService
# RUN dotnet publish --no-restore --no-build -c ${DOTNET_CONFIG} -o /app

# # final
# FROM base AS final
# ARG INSTALL_CLRDBG=exit
# RUN bash -c "${INSTALL_CLRDBG}"
# WORKDIR /app
# COPY --from=publish /app .
# ENTRYPOINT ["dotnet", "UtilityService.dll"]