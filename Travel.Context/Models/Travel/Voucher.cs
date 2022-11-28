using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models.Travel;

namespace Travel.Context.Models
{
    public class Voucher
    {
        public Guid IdVoucher { get; set; }
        public string Code { get; set; }
        
        public int Value { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        

        public virtual ICollection<Customer_Voucher> Vouchers_Customer { get; set; }
    }
}
