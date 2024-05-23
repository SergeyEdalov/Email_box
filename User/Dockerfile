# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["User/User.csproj", "User/"]
COPY ["RSATools/RSATools.csproj", "RSATools/"]
COPY ["CheckUnputDataLibrary/*", "packages/"]

RUN dotnet nuget add source /app/packages --name LocalPackages
RUN dotnet restore "User/User.csproj"

# Copy everything else and build
COPY User/. ./User
COPY RSATools/. ./RSATools
WORKDIR "/app/User"
RUN dotnet build "User.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "User.csproj" -c Release -o /app/publish

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7205
EXPOSE 5110

COPY certificates/aspnetapp.pfx .
COPY --from=publish /app/publish .
RUN rm -rf /app/RSATools
RUN apt-get update && apt-get install -y \
    curl \
    wget
COPY --from=build /app/RSATools /app/RSATools

ENTRYPOINT ["dotnet", "User.dll"]