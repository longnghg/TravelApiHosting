using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;

namespace Travel.Shared.ViewModels.Travel
{
    public class PaymentViewModel
    {
        private int idPayment;
        private string namePayment;
        private string type;
  

       

        public int IdPayment { get => idPayment; set => idPayment = value; }
        public string NamePayment { get => namePayment; set => namePayment = value; }
        public string Type { get => type; set => type = value; }
        
    }
}
