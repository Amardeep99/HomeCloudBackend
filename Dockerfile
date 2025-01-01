FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./FilesBacked/FilesBacked.csproj" --disable-parallel
RUN dotnet publish "./FilesBacked/FilesBacked.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app
COPY --from=build /app ./
COPY Images /app/Images

EXPOSE 5001

ENTRYPOINT ["dotnet", "FilesBacked.dll"]
