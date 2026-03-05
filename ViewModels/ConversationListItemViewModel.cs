namespace New_project_2.ViewModels;

public class ConversationListItemViewModel
{
    public int Id { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public string ListingTitle { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastAtUtc { get; set; }
}
