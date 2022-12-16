using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Travel
{
    [NotMapped]
   public  class OTP
    {
        public int Id { get; set; }
        public string OTPCode { get; set; }
        public long BeginTime { get; set; }
        public long EndTime { get; set; }
    }
}
