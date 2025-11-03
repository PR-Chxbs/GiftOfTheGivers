using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Resource
    {
        [Key]
        public int ResourceID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int Quantity { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; }

        // Foreign key
        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public Event? Event { get; set; }
    }
}
