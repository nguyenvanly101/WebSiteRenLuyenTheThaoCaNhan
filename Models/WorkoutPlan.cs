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

        [Required]
        [StringLength(100)]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Goal { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Level { get; set; } = string.Empty;

        [StringLength(600)]
        public string Summary { get; set; } = string.Empty;

        [Range(1, 365)]
        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; } = null!;

        public ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();
    }
}
