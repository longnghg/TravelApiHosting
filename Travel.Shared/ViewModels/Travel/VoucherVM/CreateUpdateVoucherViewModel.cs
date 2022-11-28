using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.VoucherVM
{
    public class UpdateVoucherViewModel : CreateVoucherViewModel
    {
    }
    public class CreateVoucherViewModel
    {
        private Guid idVoucher;
        private string code;
        private int value;
        private long startDate;
        private long endDate;     

      
        public string Code { get => code; set => code = value; }
        public int Value { get => value; set => this.value = value; }
        public long StartDate { get => startDate; set => startDate = value; }
        public long EndDate { get => endDate; set => endDate = value; }   
        public Guid IdVoucher { get => idVoucher; set => idVoucher = value; }
    }
}
