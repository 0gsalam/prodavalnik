namespace New_project_2.ViewModels;

public class ChatMessageViewModel
{
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; }
    public bool IsMine { get; set; }
}
