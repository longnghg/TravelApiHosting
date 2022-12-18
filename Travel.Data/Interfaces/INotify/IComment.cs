using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;
using Travel.Shared.ViewModels.Notify.CommentVM;

namespace Travel.Data.Interfaces.INotify
{
    public interface IComment
    {
        Task <Response> Gets(string idTour);
        //Task<Response> GetsId(Guid idCustomer);


        Task<Response> Create(CreateCommentViewModel input);
        Task<Response> Delete(Guid id, Guid idUser);
    }
}
