using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class Goal
    {
        [Key]
        public int GoalID { get; set; }

        // Foreign Key
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string GoalType { get; set; } = string.Empty;

        [Range(0, 999999)]
        public float TargetValue { get; set; }

        [Range(0, 999999)]
        public float CurrentValue { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "kg";

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(2);

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; } = null!;
    }

}

