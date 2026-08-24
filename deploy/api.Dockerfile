FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/ ./backend/

RUN dotnet restore backend/src/Hosts/TechCart.Api/TechCart.Api.csproj

RUN dotnet publish \
    backend/src/Hosts/TechCart.Api/TechCart.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TechCart.Api.dll"]