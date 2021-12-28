# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY InventoryManagement.Api/*.csproj ./InventoryManagement.Api/
COPY InventoryManagement.Domain/*.csproj ./InventoryManagement.Domain/
COPY InventoryManagement.Infrastructure/*.csproj ./InventoryManagement.Infrastructure/
COPY InventoryManagement.Tests/*.csproj ./InventoryManagement.Tests/
RUN dotnet restore

# Copy everything else and build
COPY InventoryManagement.Api/* ./InventoryManagement.Api/
COPY InventoryManagement.Domain/* ./InventoryManagement.Domain/
COPY InventoryManagement.Infrastructure/* ./InventoryManagement.Infrastructure/

RUN dotnet publish -c release -o /app --no-restore ./InventoryManagement.Api/

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# Setup curl command
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "InventoryManagement.Api.dll"]