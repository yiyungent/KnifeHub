#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/KnifeHub.Web/KnifeHub.Web.csproj", "src/KnifeHub.Web/"]
COPY ["src/KnifeHub.Sdk/KnifeHub.Sdk.csproj", "src/KnifeHub.Sdk/"]
RUN dotnet restore "src/KnifeHub.Web/KnifeHub.Web.csproj"
COPY . .
WORKDIR "/src/src/KnifeHub.Web"
RUN dotnet build "KnifeHub.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KnifeHub.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ADD railway-entrypoint.sh ./railway-entrypoint.sh
RUN chmod +x ./railway-entrypoint.sh

ENTRYPOINT ["/bin/sh", "./entrypoint.sh"]
# RUN ulimit -c 0
# ENTRYPOINT ["dotnet", "KnifeHub.Web.dll"]