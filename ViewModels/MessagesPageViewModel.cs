using System.ComponentModel.DataAnnotations;

namespace New_project_2.ViewModels;

public class MessagesPageViewModel
{
    public List<ConversationListItemViewModel> Conversations { get; set; } = [];
    public int? ActiveConversationId { get; set; }
    public string ActivePartnerName { get; set; } = string.Empty;
    public string ActiveListingTitle { get; set; } = string.Empty;
    public List<ChatMessageViewModel> Messages { get; set; } = [];

    [Required]
    [MaxLength(1000)]
    public string NewMessageText { get; set; } = string.Empty;
}
