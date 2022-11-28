using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel;
using Travel.Shared.ViewModels.Travel.PromotionVM;

namespace Travel.Data.Interfaces
{
    public interface IPromotions
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, Shared.Ultilities.Enums.TypeService type, bool isUpdate = false);
        Response GetsPromotion(bool isDelete);
        Response GetsPromotionExists();
        Response GetsWaitingPromotion(Guid idUser);

        Response CreatePromotion(CreatePromotionViewModel input);
        Response DeletePromotion(int id, Guid idUser);

        Response UpdatePromotion(UpdatePromotionViewModel input);
        Response ApprovePromotion(int id);
        Response RefusedPromotion(int id);

        Response RestorePromotion(int id, Guid idUser);

        Response StatisticPromotion();

    }
}

