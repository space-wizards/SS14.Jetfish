FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SS14.Jetfish/SS14.Jetfish.csproj", "SS14.Jetfish/"]
RUN dotnet restore "SS14.Jetfish/SS14.Jetfish.csproj"
COPY . .
WORKDIR "/src/SS14.Jetfish"
RUN dotnet build "SS14.Jetfish.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SS14.Jetfish.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Production

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:80
COPY ./SS14.Jetfish/appsettings.yml .
COPY ./SS14.Jetfish/appsettings.Secret.yml .

COPY --from=publish /app/publish .
RUN mkdir -p /app/data/uploads/Converted
ENTRYPOINT ["dotnet", "SS14.Jetfish.dll"]
