# Use official .NET SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY GiftOfTheGivers/*.csproj ./GiftOfTheGivers/
RUN dotnet restore GiftOfTheGivers/*.csproj

# Copy everything else and build
COPY GiftOfTheGivers/. ./GiftOfTheGivers/
WORKDIR /app/GiftOfTheGivers
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/GiftOfTheGivers/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "GiftOfTheGivers.dll"]
