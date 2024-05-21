FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 7205

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["User.csproj", "User/"]
#COPY ["RSATools/RSATools.csproj", "RSATools/"]
WORKDIR "/src/User"
RUN dotnet restore "User.csproj"
COPY . .
RUN dotnet build "User.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "User.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.dll"]