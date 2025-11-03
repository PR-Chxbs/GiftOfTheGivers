using GiftOfTheGivers.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentID { get; set; }

        // Foreign keys
        public string UserID { get; set; }  // IdentityUser primary key is string
        [ForeignKey("UserID")]
        public ApplicationUser? User { get; set; }

        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public Event? Event { get; set; }

        [MaxLength(100)]
        public string RoleInProject { get; set; }
    }
}
