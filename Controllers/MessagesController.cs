using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using New_project_2.Data;
using New_project_2.Models;
using New_project_2.ViewModels;

namespace New_project_2.Controllers;

[Authorize]
[Route("")]
public class MessagesController(AppDbContext context) : Controller
{
    [HttpGet("messages")]
    [HttpGet("messages/index")]
    public async Task<IActionResult> Index(int? conversationId, int? listingId, int? sellerId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Challenge();
        }

        if (listingId.HasValue && sellerId.HasValue && sellerId.Value != currentUserId.Value)
        {
            conversationId = await GetOrCreateConversationIdAsync(currentUserId.Value, sellerId.Value, listingId.Value);
        }

        var conversations = await context.Conversations
            .Where(c => c.BuyerId == currentUserId.Value || c.SellerId == currentUserId.Value)
            .Select(c => new ConversationListItemViewModel
            {
                Id = c.Id,
                PartnerName = c.BuyerId == currentUserId.Value ? c.Seller!.Username : c.Buyer!.Username,
                ListingTitle = c.Listing!.Title,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAtUtc).Select(m => m.Content).FirstOrDefault() ?? "No messages yet",
                LastAtUtc = c.Messages.OrderByDescending(m => m.SentAtUtc).Select(m => (DateTime?)m.SentAtUtc).FirstOrDefault() ?? c.UpdatedAtUtc
            })
            .OrderByDescending(c => c.LastAtUtc)
            .ToListAsync();

        if (!conversationId.HasValue && conversations.Count > 0)
        {
            conversationId = conversations[0].Id;
        }

        var model = new MessagesPageViewModel
        {
            Conversations = conversations,
            ActiveConversationId = conversationId
        };

        if (conversationId.HasValue)
        {
            var conversation = await context.Conversations
                .Include(c => c.Listing)
                .Include(c => c.Buyer)
                .Include(c => c.Seller)
                .FirstOrDefaultAsync(c => c.Id == conversationId.Value && (c.BuyerId == currentUserId.Value || c.SellerId == currentUserId.Value));

            if (conversation is not null)
            {
                model.ActiveListingTitle = conversation.Listing?.Title ?? string.Empty;
                model.ActivePartnerName = conversation.BuyerId == currentUserId.Value
                    ? conversation.Seller?.Username ?? string.Empty
                    : conversation.Buyer?.Username ?? string.Empty;

                model.Messages = await context.ChatMessages
                    .Where(m => m.ConversationId == conversation.Id)
                    .Include(m => m.Sender)
                    .OrderBy(m => m.SentAtUtc)
                    .Select(m => new ChatMessageViewModel
                    {
                        SenderName = m.Sender!.Username,
                        Content = m.Content,
                        SentAtUtc = m.SentAtUtc,
                        IsMine = m.SenderId == currentUserId.Value
                    })
                    .ToListAsync();
            }
        }

        return View(model);
    }

    [HttpPost("messages/send")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int conversationId, string newMessageText)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return Challenge();
        }

        var conversation = await context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && (c.BuyerId == currentUserId.Value || c.SellerId == currentUserId.Value));

        if (conversation is null)
        {
            return NotFound();
        }

        var cleanText = (newMessageText ?? string.Empty).Trim();
        if (cleanText.Length == 0)
        {
            return RedirectToAction(nameof(Index), new { conversationId });
        }

        context.ChatMessages.Add(new ChatMessage
        {
            ConversationId = conversationId,
            SenderId = currentUserId.Value,
            Content = cleanText,
            SentAtUtc = DateTime.UtcNow
        });

        conversation.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { conversationId });
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var userId) ? userId : null;
    }

    private async Task<int> GetOrCreateConversationIdAsync(int buyerId, int sellerId, int listingId)
    {
        var conversation = await context.Conversations.FirstOrDefaultAsync(c =>
            c.BuyerId == buyerId && c.SellerId == sellerId && c.ListingId == listingId);

        if (conversation is not null)
        {
            return conversation.Id;
        }

        conversation = new Conversation
        {
            BuyerId = buyerId,
            SellerId = sellerId,
            ListingId = listingId,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Conversations.Add(conversation);
        await context.SaveChangesAsync();

        return conversation.Id;
    }
}
