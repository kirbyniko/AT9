# Use the .NET 9.0 SDK base image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the project files
COPY ["AT9.csproj", "./"]

# Restore the dependencies
RUN dotnet restore "AT9.csproj"

# Copy the rest of the files and build the application
COPY . .
RUN dotnet build "AT9.csproj" -c Release -o /app

# Publish the application
RUN dotnet publish "AT9.csproj" -c Release -o /app/publish

# Use the .NET 9.0 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80


# Set the entry point for the application
ENTRYPOINT ["dotnet", "AT9.dll"]
