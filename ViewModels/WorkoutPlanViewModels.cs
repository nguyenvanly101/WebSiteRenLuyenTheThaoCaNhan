using System.ComponentModel.DataAnnotations;
using WebsiteRenLuyenTheThaoCaNhan.Models;

namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class WorkoutPlanFormViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Ten ke hoach")]
    public string PlanName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Muc tieu")]
    public string Goal { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Cap do")]
    public string Level { get; set; } = string.Empty;

    [StringLength(600)]
    [Display(Name = "Tom tat")]
    public string Summary { get; set; } = string.Empty;

    [Range(1, 365)]
    [Display(Name = "Thoi luong (ngay)")]
    public int Duration { get; set; } = 28;
}

public class WorkoutDayFormViewModel
{
    public int PlanId { get; set; }

    [Range(1, 31)]
    [Display(Name = "Thu tu ngay")]
    public int DayNumber { get; set; } = 1;

    [Required]
    [StringLength(100)]
    [Display(Name = "Trong tam")]
    public string FocusArea { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Ghi chu")]
    public string Note { get; set; } = string.Empty;
}

public class WorkoutExerciseFormViewModel
{
    public int DayId { get; set; }

    [Required]
    [Display(Name = "Bai tap")]
    public int ExerciseID { get; set; }

    [Range(1, 20)]
    [Display(Name = "Sets")]
    public int Sets { get; set; } = 4;

    [Range(1, 50)]
    [Display(Name = "Reps")]
    public int Reps { get; set; } = 10;

    [Range(0, 600)]
    [Display(Name = "Nghi giua set (giay)")]
    public int RestTime { get; set; } = 60;

    [Display(Name = "Thu tu hien thi")]
    public int DisplayOrder { get; set; } = 1;
}

public class WorkoutPlanDetailsViewModel
{
    public WorkoutPlan Plan { get; set; } = null!;
    public int TotalExercises { get; set; }
    public int TotalDays { get; set; }
}
