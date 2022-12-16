using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels;

namespace Travel.Data.Interfaces
{
    public interface ILog
    {
       bool  AddLog(string content, string type,string emailCreator, string classContent);
        Response GetsList(long fromDate, long toDate, int pageIndex, int pageSize);
        Task<Response> SearchLogByType(JObject frmData);
        Task<Response> GetDetail(Guid id);
    }
}
