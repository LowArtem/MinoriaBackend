FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MinoriaBackend.Api/MinoriaBackend.Api.csproj", "MinoriaBackend.Api/"]
COPY ["MinoriaBackend.Data/MinoriaBackend.Data.csproj", "MinoriaBackend.Data/"]
COPY ["MinoriaBackend.Core/MinoriaBackend.Core.csproj", "MinoriaBackend.Core/"]
RUN dotnet restore "MinoriaBackend.Api/MinoriaBackend.Api.csproj"
COPY . .
WORKDIR "/src/MinoriaBackend.Api"
RUN dotnet build "MinoriaBackend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinoriaBackend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinoriaBackend.Api.dll"]
