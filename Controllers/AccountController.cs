using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_project_2.Data;
using New_project_2.Models;
using New_project_2.ViewModels;

namespace New_project_2.Controllers;

[Route("")]
public class AccountController(AppDbContext context, IPasswordHasher<AppUser> hasher) : Controller
{
    [HttpGet("login")]
    [HttpGet("account/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost("login")]
    [HttpPost("account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var value = model.UsernameOrEmail.Trim();
        var user = await context.Users.FirstOrDefaultAsync(u =>
            u.Username == value || u.Email == value);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Невалидни данни.");
            return View(model);
        }

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Невалидни данни.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("register")]
    [HttpGet("account/register")]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost("register")]
    [HttpPost("account/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var exists = await context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
        if (exists)
        {
            ModelState.AddModelError(string.Empty, "Името или имейлът е вече използван.");
            return View(model);
        }

        var user = new AppUser
        {
            Username = model.Username.Trim(),
            Email = model.Email.Trim()
        };
        user.PasswordHash = hasher.HashPassword(user, model.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}

