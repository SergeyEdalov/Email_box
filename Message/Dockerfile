FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7191
COPY certificates/aspnetapp.pfx .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["Message/Message.csproj", "Message/"]
COPY ["RSATools/*", "RSATools/"]
COPY CheckUnputDataLibrary/* packages/
RUN dotnet nuget add source /app/packages --name LocalPackages
RUN dotnet restore "Message/Message.csproj"
WORKDIR "/app/Message"
COPY Message/. .
RUN dotnet build "Message.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Message.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN rm -rf /app/RSATools
COPY --from=build /app/RSATools /app/RSATools
CMD ["sh", "-c", "sleep 20 && dotnet Message.dll"]