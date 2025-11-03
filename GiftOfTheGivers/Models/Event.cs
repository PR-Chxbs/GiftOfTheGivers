using GiftOfTheGivers.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GiftOfTheGivers.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        // Navigation properties
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}

