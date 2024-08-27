# Use the official .NET SDK image for the build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

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

# Use the official .NET runtime image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory for the runtime environment
WORKDIR /app

# Copy the published output from the build environment
COPY --from=build-env /app/out .

# Install a specific version of EF Core CLI tools globally
RUN dotnet tool install --global dotnet-ef --version 7.0.0

# Ensure the global tools are in the PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Update the database using EF Core CLI tools
RUN dotnet ef database update

# Expose port 80 for the Web API
EXPOSE 80

# Set the entry point to run the Web API
ENTRYPOINT ["dotnet", "YourProjectName.dll"]
