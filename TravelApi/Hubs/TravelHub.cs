using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Travel.Context.Models;

namespace TravelApi.Hubs
{
    [Authorize]
    public class TravelHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            int repeatTimes = 4;
            for (int i = 0; i < repeatTimes; i++)
            {
                if (httpContext != null)
                {
                    i = 5;
                    var jwtToken = httpContext.Request.Query["access_token"];

                    var handler = new JwtSecurityTokenHandler();
                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        var token = handler.ReadJwtToken(jwtToken);
                        var tokenS = token as JwtSecurityToken;
                        var jti = tokenS.Claims.Where(x => x.Type == "RoleId").FirstOrDefault();

                        // replace email with your claim name

                        if (jti != null && jti.Value != "")
                        {
                            Groups.AddToGroupAsync(Context.ConnectionId, jti.Value.ToUpper());
                        }
                    }
                }
            }
           
            return base.OnConnectedAsync();
        }
        public async Task Block(string idUser)
        {
            await Clients.User(idUser.ToUpper()).SendAsync("BlockUser");

        }
        public async Task SendNotyf(string idRole)
        {
            var arrIdRole = idRole.Split(',');
            foreach (var id in arrIdRole)
            {
                await Clients.Group(id.ToUpper()).SendAsync("Notification");
            }
        }
        public async Task Chat(string idUser)
        {
            await Clients.User(idUser.ToUpper()).SendAsync("Message");
            //await Clients.Group("b76c4137-c497-42f5-821d-554a51862e45").SendAsync("Init");
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnDisconnectedAsync(ex);
        }
        public async Task Send()
        {
            var httpContext = Context.GetHttpContext();
            await Clients.All.SendAsync("Init");
        }
    }
}
