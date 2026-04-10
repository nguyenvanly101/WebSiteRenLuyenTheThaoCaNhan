using System.ComponentModel.DataAnnotations;
using WebsiteRenLuyenTheThaoCaNhan.Models;

namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class WorkoutLogFormViewModel
{
    [Display(Name = "Ngay tap")]
    public DateTime WorkoutDate { get; set; } = DateTime.Today;

    [Range(0, 600)]
    [Display(Name = "Thoi luong (phut)")]
    public int DurationMinutes { get; set; } = 60;

    [Required]
    [StringLength(20)]
    [Display(Name = "Trang thai nang luong")]
    public string EnergyLevel { get; set; } = "Balanced";

    [StringLength(255)]
    [Display(Name = "Ghi chu buoi tap")]
    public string Note { get; set; } = string.Empty;
}

public class WorkoutLogDetailFormViewModel
{
    public int LogId { get; set; }

    [Required]
    [Display(Name = "Bai tap")]
    public int ExerciseID { get; set; }

    [Range(1, 20)]
    [Display(Name = "Set so")]
    public int SetNumber { get; set; } = 1;

    [Range(0, 1000)]
    [Display(Name = "Reps")]
    public int Reps { get; set; } = 10;

    [Range(0, 1000)]
    [Display(Name = "Muc ta (kg)")]
    public float Weight { get; set; }
}

public class WorkoutLogDetailsViewModel
{
    public WorkoutLog Log { get; set; } = null!;
    public float TotalVolume { get; set; }
}
