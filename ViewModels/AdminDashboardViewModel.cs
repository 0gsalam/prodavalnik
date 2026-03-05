using New_project_2.Models;

namespace New_project_2.ViewModels;

public class AdminDashboardViewModel
{
    public string ActiveTab { get; set; } = "users";
    public List<AppUser> Users { get; set; } = [];
    public List<Listing> Listings { get; set; } = [];
}
