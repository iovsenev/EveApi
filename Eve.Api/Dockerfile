FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
#EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Eve.Api/Eve.Api.csproj", "Eve.Api/"]
COPY ["Eve.Application/Eve.Application.csproj", "Eve.Application/"]
COPY ["Eve.Domain/Eve.Domain.csproj", "Eve.Domain/"]
COPY ["Eve.Infrastructure/Eve.Infrastructure.csproj", "Eve.Infrastructure/"]
RUN dotnet restore "./Eve.Api/Eve.Api.csproj"
COPY . .
WORKDIR "/src/Eve.Api"
RUN dotnet build "./Eve.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Eve.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eve.Api.dll"]