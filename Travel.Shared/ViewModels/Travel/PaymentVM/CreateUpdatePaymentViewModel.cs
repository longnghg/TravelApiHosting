using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class UpdatePaymentViewModel : CreatePaymentViewModel
    {

    }
    public class CreatePaymentViewModel
    {

        private string idPayment;
        private string namePayment;
        private string type;

        public string IdPayment { get => idPayment; set => idPayment = value; }
        public string NamePayment { get => namePayment; set => namePayment = value; }
        public string Type { get => type; set => type = value; }
    }
}
