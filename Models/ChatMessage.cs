using System.ComponentModel.DataAnnotations;

namespace New_project_2.Models;

public class ChatMessage
{
    public int Id { get; set; }

    public int ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public int SenderId { get; set; }
    public AppUser? Sender { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
}
