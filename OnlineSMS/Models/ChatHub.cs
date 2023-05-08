using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OnlineSMS.Data;

namespace OnlineSMS.Models
{
    [Authorize(Roles = "User")]

    public class ChatHub : Hub
    {
        private readonly OnlineSMSContext onlineSMSContext;
        public ChatHub(OnlineSMSContext onlineSMSContext)
        {
            this.onlineSMSContext = onlineSMSContext;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var jwtToken = httpContext.Request.Cookies["token"].ToString();

            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ReadJwtToken(jwtToken).Claims;
            var userId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            Console.WriteLine($"{userId} Joined. ConnectionId: {Context.ConnectionId}");

            UserConnection userConnection = new UserConnection
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId
            };

            onlineSMSContext.UserConnection.Add(userConnection);

            await onlineSMSContext.SaveChangesAsync();

            await base.OnConnectedAsync();
        }


        public async Task Hello(string connectionId, string message)
        {
            Console.WriteLine($"{connectionId}: {message}");
            //await Clients.Client(connectionId).SendAsync("Hello", connectionId, message);
        }
        
    }
}
