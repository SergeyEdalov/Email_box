FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RSATools.csproj", "RSATools/"]
WORKDIR "/src/RSATools"
RUN dotnet restore "RSATools.csproj"
COPY . .
RUN dotnet build "RSATools.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RSATools.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RSATools.dll"]