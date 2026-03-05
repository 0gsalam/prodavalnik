# Prodavalnik (Демо MVC приложение)

## Какво демонстрира този проект
- ASP.NET Core MVC изгледи (views) и контролери  
- SQL Server (LocalDB) база данни с EF Core  
- Cookie автентикация (вход / регистрация / изход)  
- Основен marketplace процес: разглеждане на обяви, преглед на детайли, създаване на обява  

## База данни
Connection string е в `appsettings.json`:

- Server: `(localdb)\\MSSQLLocalDB`  
- Database: `ProdavalnikDb`

Приложението автоматично създава базата и добавя начални данни при стартиране (`Data/DbInitializer.cs`).

## Демо акаунт
- Потребителско име: `demo`  
- Парола: `demo123`

<!--
## Основен код за представяне
- Startup и Dependency Injection: `Program.cs`
- Database context: `Data/AppDbContext.cs`
- Models: `Models/AppUser.cs`, `Models/Listing.cs`
- Auth flow: `Controllers/AccountController.cs`
- Listings flow: `Controllers/ListingsController.cs`
- Profile flow: `Controllers/ProfileController.cs`
- Admin страница: `Controllers/AdminController.cs`
-->

## Стартиране
```powershell
dotnet run --project ".\\New project 2.csproj"