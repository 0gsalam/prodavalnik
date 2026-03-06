using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_project_2.Data;
using New_project_2.ViewModels;

namespace New_project_2.Controllers;

[Route("")]
public class AdminController(AppDbContext context, IConfiguration configuration) : Controller
{
    private const string AdminCookieName = "Prodavalnik.Admin";

    [HttpGet("admin")]
    [HttpGet("admin/index")]
    public async Task<IActionResult> Index(string tab = "users")
    {
        if (!HasAdminAccess())
        {
            return View("Login", new AdminAccessViewModel());
        }

        var activeTab = NormalizeTab(tab);
        var model = new AdminDashboardViewModel
        {
            ActiveTab = activeTab,
            Users = await context.Users.OrderBy(u => u.Username).ToListAsync(),
            Listings = await context.Listings.Include(l => l.Seller).OrderByDescending(l => l.CreatedAtUtc).ToListAsync()
        };

        return View(model);
    }

    [HttpPost("admin/unlock")]
    [ValidateAntiForgeryToken]
    public IActionResult Unlock(AdminAccessViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Login", model);
        }

        var adminPassword = configuration["Admin:Password"] ?? "admin123";
        if (model.Password != adminPassword)
        {
            ModelState.AddModelError(string.Empty, "Невалидна Admin парола.");
            return View("Login", model);
        }

        Response.Cookies.Append(AdminCookieName, "1", new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("admin/logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AdminCookieName);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("admin/delete-user/{id:int}")]
    [HttpPost("admin/deleteuser/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (!HasAdminAccess())
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await context.Users.FindAsync(id);
        if (user is not null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index), new { tab = "users" });
    }

    [HttpPost("admin/delete-listing/{id:int}")]
    [HttpPost("admin/deletelisting/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteListing(int id)
    {
        if (!HasAdminAccess())
        {
            return RedirectToAction(nameof(Index));
        }

        var listing = await context.Listings.FindAsync(id);
        if (listing is not null)
        {
            context.Listings.Remove(listing);
            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index), new { tab = "listings" });
    }

    private bool HasAdminAccess()
    {
        return Request.Cookies.TryGetValue(AdminCookieName, out var value) && value == "1";
    }

    private static string NormalizeTab(string? tab)
    {
        return string.Equals(tab, "listings", StringComparison.OrdinalIgnoreCase) ? "listings" : "users";
    }
}
