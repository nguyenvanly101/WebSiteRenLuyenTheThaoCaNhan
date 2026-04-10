using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class WorkoutDay
    {
        [Key]
        public int DayID { get; set; }

        // Foreign Key
        public int PlanID { get; set; }

        [Range(1, 31)]
        public int DayNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string FocusArea { get; set; } = string.Empty;

        [StringLength(255)]
        public string Note { get; set; } = string.Empty;

        // Navigation
        [ForeignKey("PlanID")]
        public WorkoutPlan WorkoutPlan { get; set; } = null!;

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}
