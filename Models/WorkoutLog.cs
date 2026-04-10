using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteRenLuyenTheThaoCaNhan.Models
{
    public class WorkoutLog
    {
        [Key]
        public int LogID { get; set; }

        // Foreign Key
        public int UserID { get; set; }

        public DateTime WorkoutDate { get; set; } = DateTime.Now;

        [Range(0, 600)]
        public int DurationMinutes { get; set; } = 60;

        [Required]
        [StringLength(20)]
        public string EnergyLevel { get; set; } = "Balanced";

        [StringLength(255)]
        public string Note { get; set; } = string.Empty;

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; } = null!;

        public ICollection<WorkoutLogDetail> WorkoutLogDetails { get; set; } = new List<WorkoutLogDetail>();
    }
}
