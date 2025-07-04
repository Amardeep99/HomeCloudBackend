FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./FilesBacked/FilesBackend.csproj" --disable-parallel
RUN dotnet publish "./FilesBacked/FilesBackend.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app
COPY --from=build /app ./
COPY Images /app/Images

EXPOSE 5001

ENTRYPOINT ["dotnet", "FilesBackend.dll"]
