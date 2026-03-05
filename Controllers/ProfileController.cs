using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_project_2.Data;
using New_project_2.ViewModels;

namespace New_project_2.Controllers;

[Authorize]
[Route("")]
public class ProfileController(AppDbContext context) : Controller
{
    [HttpGet("profile")]
    [HttpGet("profile/index")]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Challenge();
        }

        var user = await context.Users.FirstAsync(u => u.Id == userId);
        var listings = await context.Listings
            .Where(l => l.SellerId == userId)
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync();

        return View(new ProfileViewModel
        {
            User = user,
            Listings = listings
        });
    }
}
