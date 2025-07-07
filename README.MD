Startup:

Kjør kommando fra FilesBackend.API for å skape sqlite database:

dotnet ef migrations add InitialCreate --project ../FilesBackend.Database --startup-project .
dotnet ef database update --project ../FilesBackend.Database --startup-project .
