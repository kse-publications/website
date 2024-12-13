﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Publications.API/Publications.API.csproj", "Publications.API/"]
RUN dotnet restore "Publications.API/Publications.API.csproj"
COPY . .
WORKDIR "/src/Publications.API"
RUN dotnet build "Publications.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Publications.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT ""
ENV Notion__AuthToken ""
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir ./sqlite_db
RUN chmod -R 777 ./sqlite_db
ENTRYPOINT ["dotnet", "Publications.API.dll"]
