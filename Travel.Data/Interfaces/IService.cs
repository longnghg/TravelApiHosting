using Newtonsoft.Json.Linq;
using System;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Data.Interfaces
{
    public  interface IService
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, Shared.Ultilities.Enums.TypeService type, bool isUpdate = false);



        Response CreateContract(CreateContractViewModel input);

        Response GetsRestaurant(bool isDelete);
        Response GetsWaitingRestaurant(Guid idUser, int pageIndex, int pageSize);
        Response CreateRestaurant(CreateRestaurantViewModel input);
        Response DeleteRestaurant(Guid id, Guid idUser);
        Response GetsWaitingPlace(Guid idUser, int pageIndex, int pageSize);
        Response GetsPlace(bool isDelete);
        Response CreatePlace(CreatePlaceViewModel input);
        Response DeletePlace(Guid id, Guid idUser);
        Response UpdatePlace(UpdatePlaceViewModel input);
        Response ApprovePlace(Guid id);
        Response RefusedPlace(Guid id);

        Response RestorePlace(Guid id, Guid idUser);



        Response GetsHotel(bool isDelete);
        Response GetsWaitingHotel(Guid idUser, int pageIndex, int pageSize);
        Response CreateHotel(CreateHotelViewModel input);
        Response UpdateHotel(UpdateHotelViewModel input);

        Response DeleteHotel(Guid id, Guid idUser);
        Response ApproveHotel(Guid id);
        Response RefusedHotel(Guid id);
        Response RestoreHotel(Guid id, Guid idUser);

        Response RefusedRestaurant(Guid id);
        Response ApproveRestaurant(Guid id);
        Response UpdateRestaurant(UpdateRestaurantViewModel input);
        Response RestoreRestaurant(Guid id, Guid idUser);

        Response SearchHotel(JObject frmData);
        Response SearchHotelWaiting(JObject frmData);

        Response SearchPlace(JObject frmData);
        Response SearchPlaceWaiting(JObject frmData);
        Response SearchRestaurant(JObject frmData);
        Response SearchRestaurantWaiting(JObject frmData);
    }
}
