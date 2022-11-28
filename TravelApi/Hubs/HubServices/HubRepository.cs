using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TravelApi.Helpers;

namespace TravelApi.Hubs.HubServices
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HubRepository : IHubRepository
    {
        IHubContext<TravelHub> _hubContext;
        public HubRepository(IHubContext<TravelHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Send(string idusser)
        {
            await _hubContext.Clients.User("b07a87d7-c378-4e6c-9af8-447a3ee852b1").SendAsync("Init");
            //await _hubContext.Clients.All.SendAsync("Init");
        }
    }
}
