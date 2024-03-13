#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/KnifeHub.Web/KnifeHub.Web.csproj", "src/KnifeHub.Web/"]
COPY ["src/KnifeHub.Sdk/KnifeHub.Sdk.csproj", "src/KnifeHub.Sdk/"]
RUN dotnet restore "src/KnifeHub.Web/KnifeHub.Web.csproj"
COPY . .
WORKDIR "/src/src/KnifeHub.Web"
RUN dotnet build --framework net8.0 "KnifeHub.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --framework net8.0 "KnifeHub.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ulimit -c 0
ENTRYPOINT ["dotnet", "KnifeHub.Web.dll"]