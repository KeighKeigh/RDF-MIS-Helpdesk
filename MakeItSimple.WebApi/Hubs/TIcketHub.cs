using Microsoft.AspNetCore.SignalR;

namespace MakeItSimple.WebApi.Hubs
{
    public class TicketHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //var userRole = Context.GetHttpContext()?.User?.FindFirst("Role").Value;

            //if (userRole == "Admin")
            //{
            //    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            //}
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

            await base.OnConnectedAsync();
        }
    }
}
