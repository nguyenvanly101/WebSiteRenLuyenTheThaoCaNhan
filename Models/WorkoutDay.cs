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

        public int DayNumber { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        // Navigation
        [ForeignKey("PlanID")]
        public WorkoutPlan WorkoutPlan { get; set; }

        public List<WorkoutExercise> WorkoutExercises { get; set; }
    }
}
