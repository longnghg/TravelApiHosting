using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Shared.ViewModels.Travel.PromotionVM
{
    public class CreatePromotionViewModel: UpdateApproveData
    {
        private int idPromotion;
        private int value;
        private long toDate;
        private long fromDate;
        private string modifyBy;
        public int IdPromotion { get => idPromotion; set => idPromotion = value; }
        public int Value { get => value; set => this.value = value; }
        public long ToDate { get => toDate; set => toDate = value; }
        public long FromDate { get => fromDate; set => fromDate = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
    }
    public class UpdatePromotionViewModel : CreatePromotionViewModel
    {

    }
}
