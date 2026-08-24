FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/ ./backend/

RUN dotnet restore backend/src/Hosts/TechCart.Worker/TechCart.Worker.csproj

RUN dotnet publish \
    backend/src/Hosts/TechCart.Worker/TechCart.Worker.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TechCart.Worker.dll"]