using System.ComponentModel.DataAnnotations;

namespace New_project_2.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<Listing> Listings { get; set; } = [];
    public List<Conversation> BuyerConversations { get; set; } = [];
    public List<Conversation> SellerConversations { get; set; } = [];
    public List<ChatMessage> SentMessages { get; set; } = [];
}
