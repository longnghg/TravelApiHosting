using Newtonsoft.Json.Linq;
using System;
using Travel.Context.Models;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Data.Interfaces
{
    public  interface IService
    {
        string CheckBeforSave(JObject frmData, ref Notification _message, Shared.Ultilities.Enums.TypeService type, bool isUpdate = false);



        Response CreateContract(CreateContractViewModel input, string emailUser);

        Response GetsRestaurant(bool isDelete);
        Response GetsWaitingRestaurant(Guid idUser, int pageIndex, int pageSize);
        Response CreateRestaurant(CreateRestaurantViewModel input, string emailUser);
        Response DeleteRestaurant(Guid id, Guid idUser, string emailUser);
        Response GetsWaitingPlace(Guid idUser, int pageIndex, int pageSize);
        Response GetsPlace(bool isDelete);
        Response CreatePlace(CreatePlaceViewModel input, string emailUser);
        Response DeletePlace(Guid id, Guid idUser, string emailUser);
        Response UpdatePlace(UpdatePlaceViewModel input, string emailUser);
        Response ApprovePlace(Guid id);
        Response RefusedPlace(Guid id);

        Response RestorePlace(Guid id, Guid idUser, string emailUser);



        Response GetsHotel(bool isDelete);
        Response GetsWaitingHotel(Guid idUser, int pageIndex, int pageSize);
        Response CreateHotel(CreateHotelViewModel input, string emailUser);
        Response UpdateHotel(UpdateHotelViewModel input, string emailUser);

        Response DeleteHotel(Guid id, Guid idUser, string emailUser);
        Response ApproveHotel(Guid id);
        Response RefusedHotel(Guid id);
        Response RestoreHotel(Guid id, Guid idUser, string emailUser);

        Response RefusedRestaurant(Guid id);
        Response ApproveRestaurant(Guid id);
        Response UpdateRestaurant(UpdateRestaurantViewModel input, string emailUser);
        Response RestoreRestaurant(Guid id, Guid idUser, string emailUser);

        Response SearchHotel(JObject frmData);
        Response SearchPlace(JObject frmData);
        Response SearchRestaurant(JObject frmData);


        Response GetListHotelByProvince(string toPlace);
        Response GetListPlaceByProvince(string toPlace);
        Response GetListRestaurantByProvince(string toPlace);

    }
}
