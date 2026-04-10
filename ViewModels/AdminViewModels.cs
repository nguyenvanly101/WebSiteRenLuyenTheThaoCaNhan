using System.ComponentModel.DataAnnotations;
using WebsiteRenLuyenTheThaoCaNhan.Models;

namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalExercises { get; set; }
    public int TotalPlans { get; set; }
    public int TotalLogs { get; set; }
    public IReadOnlyList<User> RecentUsers { get; set; } = Array.Empty<User>();
}

public class AdminUserEditViewModel
{
    public int UserID { get; set; }

    [Required]
    [StringLength(80)]
    [Display(Name = "Ho ten")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    [Display(Name = "Ten dang nhap")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Vai tro")]
    public string Role { get; set; } = "User";

    [Display(Name = "Hoat dong")]
    public bool IsActive { get; set; }
}

public class AdminExerciseListItemViewModel
{
    public Exercise Exercise { get; set; } = null!;
    public int PlanUsageCount { get; set; }
    public int LogUsageCount { get; set; }
}

public class AdminUserListItemViewModel
{
    public User User { get; set; } = null!;
    public int PlanCount { get; set; }
    public int GoalCount { get; set; }
    public int LogCount { get; set; }
}
