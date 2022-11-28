using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Travel
{
   public class Customer_Voucher
    {
        public Guid IdCustomer_Voucher { get; set; }
        public Guid CustomerId { get; set; }

        public Guid VoucherId { get; set; }

        public Voucher voucher { get; set; }
        public Customer customer { get; set; }

    }
}
