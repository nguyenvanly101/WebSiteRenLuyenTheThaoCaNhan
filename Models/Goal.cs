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

        [StringLength(50)]
        public string GoalType { get; set; } // giảm cân, tăng cơ

        public float TargetValue { get; set; }
        public float CurrentValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; }
    }

}

