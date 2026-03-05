using New_project_2.Models;

namespace New_project_2.ViewModels;

public class ProfileViewModel
{
    public AppUser User { get; set; } = new();
    public List<Listing> Listings { get; set; } = [];
}
