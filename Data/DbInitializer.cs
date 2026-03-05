using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using New_project_2.Models;

namespace New_project_2.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>();

        // Keep the demo DB schema in sync without introducing migrations complexity.
        try
        {
            await context.Database.EnsureCreatedAsync();
            await context.Conversations.Take(1).AnyAsync();
        }
        catch
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var demoUser = new AppUser
            {
                Username = "demo",
                Email = "demo@prodavalnik.local"
            };
            demoUser.PasswordHash = hasher.HashPassword(demoUser, "demo123");

            var alexUser = new AppUser
            {
                Username = "alex",
                Email = "alex@prodavalnik.local"
            };
            alexUser.PasswordHash = hasher.HashPassword(alexUser, "demo123");

            context.Users.AddRange(demoUser, alexUser);
            await context.SaveChangesAsync();

            context.Listings.AddRange(
                new Listing
                {
                    Title = "Gaming Laptop RTX",
                    Description = "15-inch performance laptop in excellent condition.",
                    Price = 950m,
                    Category = "Electronics",
                    ImageUrl = "https://images.unsplash.com/photo-1517336714739-489689fd1ca8?auto=format&fit=crop&w=800&q=80",
                    SellerId = demoUser.Id
                },
                new Listing
                {
                    Title = "City E-Bike",
                    Description = "Lightweight commuter e-bike with 45km range.",
                    Price = 620m,
                    Category = "Vehicles",
                    ImageUrl = "https://images.unsplash.com/photo-1550355291-bbee04a92027?auto=format&fit=crop&w=800&q=80",
                    SellerId = alexUser.Id
                },
                new Listing
                {
                    Title = "Scandinavian Sofa",
                    Description = "3-seat fabric sofa, minimal wear, modern design.",
                    Price = 340m,
                    Category = "Home and Garden",
                    ImageUrl = "https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?auto=format&fit=crop&w=800&q=80",
                    SellerId = demoUser.Id
                },
                new Listing
                {
                    Title = "Vintage Leather Jacket",
                    Description = "Classic brown leather jacket, size M.",
                    Price = 110m,
                    Category = "Fashion",
                    ImageUrl = "https://images.unsplash.com/photo-1483985988355-763728e1935b?auto=format&fit=crop&w=800&q=80",
                    SellerId = alexUser.Id
                }
            );

            await context.SaveChangesAsync();
        }

        if (!await context.Conversations.AnyAsync())
        {
            var demo = await context.Users.FirstAsync(u => u.Username == "demo");
            var alex = await context.Users.FirstAsync(u => u.Username == "alex");
            var listing = await context.Listings.FirstAsync();

            var conversation = new Conversation
            {
                BuyerId = demo.Id,
                SellerId = alex.Id,
                ListingId = listing.Id,
                UpdatedAtUtc = DateTime.UtcNow
            };
            context.Conversations.Add(conversation);
            await context.SaveChangesAsync();

            context.ChatMessages.AddRange(
                new ChatMessage
                {
                    ConversationId = conversation.Id,
                    SenderId = demo.Id,
                    Content = "Hi, is this listing still available?",
                    SentAtUtc = DateTime.UtcNow.AddMinutes(-30)
                },
                new ChatMessage
                {
                    ConversationId = conversation.Id,
                    SenderId = alex.Id,
                    Content = "Yes, it is available.",
                    SentAtUtc = DateTime.UtcNow.AddMinutes(-28)
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
