using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Shared.ViewModels.Travel.PromotionVM
{
    public class PromotionViewModel
    {
        private int idPromotion;
        private int value;
        private long toDate;
        private long fromDate;
        private string modifyBy;
        private long modifyDate;
        private int approve;
        private bool isDelete;
        private string typeAction;
        private string idUserModify;
        public int IdPromotion { get => idPromotion; set => idPromotion = value; }
        public int Value { get => value; set => this.value = value; }
        public long ToDate { get => toDate; set => toDate = value; }
        public long FromDate { get => fromDate; set => fromDate = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public long ModifyDate { get => modifyDate; set => modifyDate = value; }
        public int Approve { get => approve; set => approve = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public string TypeAction { get => typeAction; set => typeAction = value; }
        public string IdUserModify { get => idUserModify; set => idUserModify = value; }
    }
}
