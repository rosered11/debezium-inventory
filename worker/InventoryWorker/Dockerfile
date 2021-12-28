# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .

RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# Setup curl command
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "InventoryWorker.dll"]