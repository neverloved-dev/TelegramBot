# Use the official .NET SDK image for .NET 8.0 as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy everything from the current directory to the working directory inside the container
COPY . .

# Restore NuGet packages
RUN dotnet restore

# Build the application in Release mode
RUN dotnet build -c Release --no-restore

# Publish the application to the out directory in Release mode
RUN dotnet publish -c Release -o out --no-build

# Install a specific version of EF Core CLI tools globally
RUN dotnet tool install --global dotnet-ef --version 8.0.8

# Ensure the global tools are in the PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Run EF Core migrations (specify the path to the project that contains EF Core context)
RUN dotnet ef database update --project /app/TelegramBot/TelegramBot.csproj --startup-project /app/TelegramBot/TelegramBot.csproj

# Use the official .NET runtime image for .NET 8.0 as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime environment
WORKDIR /app

# Copy the published output from the build environment
COPY --from=build-env /app/out .

# Expose port 80 for the Web API
EXPOSE 80

# Set the entry point to run the Web API
ENTRYPOINT ["dotnet", "TelegramBot.dll"]
