using GiftOfTheGivers.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        [Key]
        public int DonationID { get; set; }

        [Required, MaxLength(100)]
        public string DonorName { get; set; }

        public decimal? Amount { get; set; }

        [MaxLength(255)]
        public string ItemDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateReceived { get; set; }

        // Foreign key
        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public Event? Event { get; set; }
    }
}
