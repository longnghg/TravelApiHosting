using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.ReviewVM;

namespace Travel.Data.Interfaces
{
   public interface IReview
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, bool isUpdate);
        Response GetsReview();
  
        Response CreateReview(CreateReviewModel input, string emailUser);

    }
}
