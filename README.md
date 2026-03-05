# Prodavalnik (Demo MVC App)

## What this project demonstrates
- ASP.NET Core MVC views + controllers
- SQL Server (LocalDB) database with EF Core
- Cookie authentication (login/register/logout)
- Basic marketplace flow: browse listings, view details, create listing

## Database
Connection string is in `appsettings.json`:
- Server: `(localdb)\\MSSQLLocalDB`
- Database: `ProdavalnikDb`

The app auto-creates and seeds data on startup (`Data/DbInitializer.cs`).

## Demo account
- Username: `demo`
- Password: `demo123`

## Main code to present
- Startup and DI: `Program.cs`
- Database context: `Data/AppDbContext.cs`
- Models: `Models/AppUser.cs`, `Models/Listing.cs`
- Auth flow: `Controllers/AccountController.cs`
- Listings flow: `Controllers/ListingsController.cs`
- Profile flow: `Controllers/ProfileController.cs`
- Admin page: `Controllers/AdminController.cs`

## Run
```powershell
dotnet run --project ".\\New project 2.csproj"
```
