#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/QQBotHub.Web/QQBotHub.Web.csproj", "src/QQBotHub.Web/"]
COPY ["src/QQBotHub.Sdk/QQBotHub.Sdk.csproj", "src/QQBotHub.Sdk/"]
RUN dotnet restore "src/QQBotHub.Web/QQBotHub.Web.csproj"
COPY . .
WORKDIR "/src/src/QQBotHub.Web"
RUN dotnet build "QQBotHub.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QQBotHub.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QQBotHub.Web.dll"]