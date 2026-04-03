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

        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public float Weight { get; set; }

        // Navigation
        [ForeignKey("LogID")]
        public WorkoutLog WorkoutLog { get; set; }

        [ForeignKey("ExerciseID")]
        public Exercise Exercise { get; set; }
    }
}
