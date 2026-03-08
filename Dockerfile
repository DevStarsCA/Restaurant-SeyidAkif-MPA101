FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Restaurant-Final.slnx .
COPY Restaurant.Domain/Restaurant.Domain.csproj Restaurant.Domain/
COPY Restaurant.Application/Restaurant.Application.csproj Restaurant.Application/
COPY Restaurant.Persistence/Restaurant.Persistence.csproj Restaurant.Persistence/
COPY Restaurant.Infrastructure/Restaurant.Infrastructure.csproj Restaurant.Infrastructure/
COPY Restaurant.API/Restaurant.API.csproj Restaurant.API/

RUN dotnet restore Restaurant.API/Restaurant.API.csproj

COPY . .
RUN dotnet publish Restaurant.API/Restaurant.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Restaurant.API.dll"]
