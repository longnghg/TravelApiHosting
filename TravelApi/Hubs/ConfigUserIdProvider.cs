using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TravelApi.Hubs
{
    public class ConfigUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            var ssss = connection.User?.Claims.Where(c => c.Type == "UserId").FirstOrDefault().Value.ToString();
            return ssss.ToUpper();
            //return connection.User?.FindFirst(ClaimTypes.NameIdentifier).ToString();
        }
    }
}
