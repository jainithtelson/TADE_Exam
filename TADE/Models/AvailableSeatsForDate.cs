using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TADE.Models
{
    public class AvailableSeatsForDate
    {
        public int? SlotId { get; set; }
        public string SlotName { get; set; }
        public int availableSeatsForSlotsId { get; set; }
        public int? TotalSeats { get; set; }
        public int? RemainingSeats { get; set; }
    }
}