using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class WorkoutLogDetail
    {
        [Key]
        public int ID { get; set; }

        // Foreign Keys
        public int LogID { get; set; }
        public int ExerciseID { get; set; }

        [Range(1, 20)]
        public int SetNumber { get; set; }

        [Range(0, 1000)]
        public int Reps { get; set; }

        [Range(0, 1000)]
        public float Weight { get; set; }

        // Navigation
        [ForeignKey("LogID")]
        public WorkoutLog WorkoutLog { get; set; } = null!;

        [ForeignKey("ExerciseID")]
        public Exercise Exercise { get; set; } = null!;
    }
}
