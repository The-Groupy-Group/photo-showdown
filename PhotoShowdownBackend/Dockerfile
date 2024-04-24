FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY PhotoShowdownBackend/PhotoShowdownBackend.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY PhotoShowdownBackend/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port
EXPOSE 5299

# Run the app
ENTRYPOINT ["dotnet", "PhotoShowdownBackend.dll"]
