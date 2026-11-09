# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ShopLite.Api/ShopLite.Api.csproj", "ShopLite.Api/"]
RUN dotnet restore "ShopLite.Api/ShopLite.Api.csproj"

COPY ["ShopLite.Api/", "ShopLite.Api/"]
WORKDIR "/src/ShopLite.Api"
RUN dotnet publish "ShopLite.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ShopLite.Api.dll"]
