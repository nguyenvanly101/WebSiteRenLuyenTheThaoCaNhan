using System.ComponentModel.DataAnnotations;

namespace WebsiteRenLuyenTheThaoCaNhan.ViewModels;

public class GoalFormViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Ten muc tieu")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Loai muc tieu")]
    public string GoalType { get; set; } = string.Empty;

    [Range(0, 999999)]
    [Display(Name = "Gia tri muc tieu")]
    public float TargetValue { get; set; }

    [Range(0, 999999)]
    [Display(Name = "Gia tri hien tai")]
    public float CurrentValue { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Don vi")]
    public string Unit { get; set; } = "kg";

    [Required]
    [StringLength(20)]
    [Display(Name = "Trang thai")]
    public string Status { get; set; } = "Active";

    [Display(Name = "Ngay bat dau")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "Ngay ket thuc")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(2);
}
