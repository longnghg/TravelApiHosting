using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models.Notification;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Notify.MessengerVM;

namespace Travel.Data.Interfaces.INotify
{
    public interface IMessenger
    {
        Task<Response> Create(Messenger input);
        Task<Response> SupportedReply(Messenger input);
        Task<Response> CustomerViewMessenger(Guid IdCustomer);
        Task<Response> SupporterViewMessenger(Guid IdSuporter);
        Task<Response> CheckSeenMessenger(Guid IdMessenger);
        Task<Response> CheckSeenMessenger(Guid idCus, Guid idSp);

        Task<Response> UpdateGuestMessenger(Guid idCus, Guid idGuest);
    }
}
