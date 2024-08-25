FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TelegramBot/TelegramBot.csproj", "TelegramBot/"]
COPY ["TelegramBot/TelegramBotUnitTests.csproj", "TelegramBotUnitTests/"]
RUN dotnet restore "TelegramBot/TelegramBot.csproj"
RUN dotnet restore "TelegramBot/TelegramBotUnitTests.csproj"
COPY . .
WORKDIR "/src/TelegramBot"
RUN dotnet build "TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/build
# Install EF Core CLI tool
RUN dotnet tool install --global dotnet-ef
RUN dotnet test
CMD["dotnet ef database update"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.dll"]
