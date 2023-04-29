using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OnlineSMS.Data;
using OnlineSMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.RequestModels;

namespace OnlineSMS.Controllers
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

            var userId = GetUserId();
            var sessionId = GetSessionId();

            Console.WriteLine($" SessionId: {sessionId} ConnectionId: {Context.ConnectionId}");

            var userConnection = onlineSMSContext.UserConnection
                .Where(uc => uc.UserId == userId && uc.SessionId == sessionId).FirstOrDefault();

            if (userConnection != null)
            {
                userConnection.ConnectionId = Context.ConnectionId;
            }
            else
            {
                userConnection = new UserConnection
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId,
                    SessionId = sessionId
                };
                onlineSMSContext.UserConnection.Add(userConnection);
            }

            await onlineSMSContext.SaveChangesAsync();

            await base.OnConnectedAsync();

        }


        public async Task SendMessageTo(SendMessage sendMessage)
        {
            string userSendId = GetUserId();
            string boxChatId = sendMessage.BoxChatId;
            string dataMessage = sendMessage.DataMessage;
            string type = sendMessage.Type;


            Console.WriteLine($"{boxChatId}: {dataMessage}");

            var listMember = await onlineSMSContext.MemberBoxchat.Where(m => m.BoxchatId == boxChatId).ToListAsync();

            var listMemberConnection = listMember.Select(m => m.UserId).ToList();

            var userConnections = await onlineSMSContext.UserConnection.Where(u => listMemberConnection.Contains(u.UserId)).ToListAsync();

            var listConnection = userConnections.Select(c => c.ConnectionId).ToList();

            var newMessage = new Message
            {
                BoxchatId = boxChatId,
                UserSendId = userSendId,
                Status = Constants.StatusBoxChatMessage.Show,
                Type = type,
                Content = dataMessage,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            onlineSMSContext.Message.Add(newMessage);
            await onlineSMSContext.SaveChangesAsync();


            var listUnreadMessages = new List<UnreadMessages>();
            
            foreach (var member in listMember)
            {
                if(member.UserId != userSendId)
                {
                    listUnreadMessages.Add(new UnreadMessages
                    {
                        UserId = member.UserId,
                        MessageId = newMessage.Id
                    });
                }
            }

            onlineSMSContext.UnreadMessages.AddRange(listUnreadMessages);
            await onlineSMSContext.SaveChangesAsync();

            var userProfileUserSend = await onlineSMSContext.UserProfile
                .Select(m => new { 
                    m.UserId,
                    m.FullName,
                    m.Avatar
                }).Where(u => u.UserId == userSendId).FirstOrDefaultAsync();


            await Clients.Clients(listConnection).SendAsync("ListenMesage", new
            {
                boxchatId = boxChatId,
                userSendId,
                content =  dataMessage,
                type,
                newMessage.Status,
                newMessage.StartDate,
                newMessage.EndDate
            });
        }

















        private string GetUserId()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Cookies["userId"].ToString();
            return userId;
        }

        private string GetSessionId()
        {
            var httpContext = Context.GetHttpContext();
            var sessionId = httpContext.Request.Cookies["session"].ToString();
            return sessionId;
        }

    }
}
