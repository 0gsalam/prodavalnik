using System.ComponentModel.DataAnnotations;

namespace New_project_2.Models;

public class Conversation
{
    public int Id { get; set; }

    public int ListingId { get; set; }
    public Listing? Listing { get; set; }

    public int BuyerId { get; set; }
    public AppUser? Buyer { get; set; }

    public int SellerId { get; set; }
    public AppUser? Seller { get; set; }

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<ChatMessage> Messages { get; set; } = [];
}
