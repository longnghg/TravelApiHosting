using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    [NotMapped]
    public class Payment
    {
        public int IdPayment { get; set; }
        public string NamePayment { get; set; }
        public string Type { get; set; }
        public virtual ICollection<TourBooking> TourBooking { get; set; }
    }
}
