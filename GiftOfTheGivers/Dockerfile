# Use official .NET SDK for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the csproj files for main app and tests
COPY GiftOfTheGivers.csproj ./GiftOfTheGivers/

# Restore dependencies
RUN dotnet restore GiftOfTheGivers/GiftOfTheGivers.csproj

# Copy everything else
COPY . ./GiftOfTheGivers/

# Build and publish
WORKDIR /app/GiftOfTheGivers
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/GiftOfTheGivers/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "GiftOfTheGivers.dll"]
