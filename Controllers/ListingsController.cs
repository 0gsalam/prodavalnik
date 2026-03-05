using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_project_2.Data;
using New_project_2.Models;
using New_project_2.ViewModels;

namespace New_project_2.Controllers;

[Route("")]
public class ListingsController(AppDbContext context) : Controller
{
    [HttpGet("marketplace")]
    [HttpGet("listings")]
    public async Task<IActionResult> Index(string? q, string? category)
    {
        var query = context.Listings.Include(l => l.Seller).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(l => l.Title.Contains(q) || l.Description.Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(l => l.Category == category);
        }

        var listings = await query
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync();

        return View(listings);
    }

    [HttpGet("listing-details/{id:int?}")]
    [HttpGet("listings/details/{id:int?}")]
    public async Task<IActionResult> Details(int? id)
    {
        Listing? listing;

        if (id.HasValue)
        {
            listing = await context.Listings
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.Id == id.Value);
        }
        else
        {
            listing = await context.Listings
                .Include(l => l.Seller)
                .OrderByDescending(l => l.CreatedAtUtc)
                .FirstOrDefaultAsync();
        }

        if (listing is null)
        {
            return NotFound();
        }

        ViewData["Related"] = await context.Listings
            .Where(l => l.Id != listing.Id && l.Category == listing.Category)
            .OrderByDescending(l => l.CreatedAtUtc)
            .Take(3)
            .ToListAsync();

        return View(listing);
    }

    [Authorize]
    [HttpGet("create-listing")]
    [HttpGet("listings/create")]
    public IActionResult Create()
    {
        return View(new CreateListingViewModel());
    }

    [Authorize]
    [HttpPost("create-listing")]
    [HttpPost("listings/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateListingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Challenge();
        }

        var listing = new Listing
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            Price = model.Price,
            Category = model.Category.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl)
                ? "https://images.unsplash.com/photo-1563013544-824ae1b704d3?auto=format&fit=crop&w=900&q=80"
                : model.ImageUrl.Trim(),
            SellerId = userId
        };

        context.Listings.Add(listing);
        await context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = listing.Id });
    }
}

