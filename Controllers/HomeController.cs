using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_project_2.Data;
using New_project_2.Models;

namespace New_project_2.Controllers;

[Route("")]
public class HomeController(AppDbContext context) : Controller
{
    [HttpGet("")]
    [HttpGet("home")]
    public async Task<IActionResult> Index()
    {
        var featured = await context.Listings
            .OrderByDescending(l => l.CreatedAtUtc)
            .Take(4)
            .ToListAsync();

        return View(featured);
    }

    [HttpGet("home/error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
