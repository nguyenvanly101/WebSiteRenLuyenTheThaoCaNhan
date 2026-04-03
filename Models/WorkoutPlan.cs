using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class WorkoutPlan
    {
        [Key]
        public int PlanID { get; set; }

        // Foreign Key
        public int UserID { get; set; }

        [StringLength(100)]
        public string PlanName { get; set; }

        [StringLength(50)]
        public string Goal { get; set; }

        [StringLength(50)]
        public string Level { get; set; }

        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; }

        public List<WorkoutDay> WorkoutDays { get; set; }
    }
}
