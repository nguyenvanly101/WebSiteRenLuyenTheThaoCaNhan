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

        public DateTime WorkoutDate { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; }

        public List<WorkoutLogDetail> WorkoutLogDetails { get; set; }
    }
}
