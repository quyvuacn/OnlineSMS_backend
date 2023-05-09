using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using OnlineSMS.Data;
using OnlineSMS.Models;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.RequestModels;
using Polly;
using static OnlineSMS.Constants;

namespace OnlineSMS.Controllers
{
    [Authorize(Roles = "User")]

    public class Hub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly OnlineSMSContext onlineSMSContext;
        public Hub(OnlineSMSContext onlineSMSContext)
        {
            this.onlineSMSContext = onlineSMSContext;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var sessionId = GetSessionId();

            Console.WriteLine($"SessionId: {sessionId} ConnectionId: {Context.ConnectionId}");

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



            var listMember = await onlineSMSContext.MemberBoxchat.Where(m => m.BoxchatId == boxChatId).ToListAsync();

            var listMemberConnection = listMember.Select(m => m.UserId).ToList();

            var userConnections = await onlineSMSContext.UserConnection.Where(u => listMemberConnection.Contains(u.UserId)).ToListAsync();

            foreach (var item in userConnections)
            {
                Console.WriteLine($"UserId {item.UserId}");
                Console.WriteLine($"ConnectionId {item.ConnectionId}");
            }

            var listConnection = userConnections.Select(c => c.ConnectionId).ToList();

            var userSendProfile = await onlineSMSContext.UserProfile.Where(u => u.UserId == userSendId).FirstOrDefaultAsync(); 

            var newMessage = new Message
            {
                BoxchatId = boxChatId,
                UserSendId = userSendId,
                UserAvatar = userSendProfile.Avatar,
                UserFullName = userSendProfile.FullName,
                Status = StatusBoxChatMessage.Show,
                Type = type,
                Content = dataMessage,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            var boxchat = await onlineSMSContext.Boxchat.FindAsync(boxChatId);

            boxchat.LastMessageContent = dataMessage;
            boxchat.LastUserSendFullName = userSendProfile.FullName;
            boxchat.LastUserSendId = userSendId;

            
            onlineSMSContext.Boxchat.Update(boxchat);

            onlineSMSContext.Message.Add(newMessage);

            await onlineSMSContext.SaveChangesAsync();

            var listUnreadMessages = new List<UnreadMessage>();
            
            foreach (var member in listMember)
            {
                if(member.UserId != userSendId)
                {
                    listUnreadMessages.Add(new UnreadMessage
                    {
                        UserId = member.UserId,
                        MessageId = newMessage.Id,
                        BoxchatId = boxChatId
                    });
                }
            }

            onlineSMSContext.UnreadMessage.AddRange(listUnreadMessages);
            await onlineSMSContext.SaveChangesAsync();

            var userProfileUserSend = await onlineSMSContext.UserProfile
                .Select(m => new { 
                    m.UserId,
                    m.FullName,
                    m.Avatar
                }).Where(u => u.UserId == userSendId).FirstOrDefaultAsync();

            Console.WriteLine("SendMessage");

            await Clients.Clients(listConnection).SendAsync("ListenMesage", new
            {
                boxchatId = boxChatId,
                userSendId,
                content =  dataMessage,
                type,
                newMessage.Status,
                newMessage.StartDate,
                newMessage.EndDate,
                lastUserSendFullName = userSendProfile.FullName,
                UserAvatar = userSendProfile.Avatar
            });
        }

        public async Task CallTo(CallTo callTo)
        {
            string userSendId = GetUserId();
            string boxChatId = callTo.BoxchatId;
            string type = callTo.Type;

            var listMember = await onlineSMSContext.MemberBoxchat
                .Where(m => m.BoxchatId == boxChatId && m.UserId != userSendId).ToListAsync();

            var listMemberConnection = listMember.Select(m => m.UserId).ToList();

            var userConnections = await onlineSMSContext.UserConnection.Where(u => listMemberConnection.Contains(u.UserId)).ToListAsync();

            var listConnection = userConnections.Select(c => c.ConnectionId).ToList();

            var userSendProfile = await onlineSMSContext.UserProfile.Where(u => u.UserId == userSendId).FirstOrDefaultAsync();
            Console.WriteLine("New ListenCall");
            await Clients.Clients(listConnection).SendAsync("ListenCall", new
            {
                boxchatId = boxChatId,
                userSendId,
                type,
                UserSendFullName = userSendProfile.FullName,
                UserAvatar = userSendProfile.Avatar
            });
        }

        public async Task PickUp(CallTo callTo)
        {
            string userId = GetUserId();
            string boxChatId = callTo.BoxchatId;

            var listMember = await onlineSMSContext.MemberBoxchat
                .Where(m => m.BoxchatId == boxChatId && m.UserId != userId).ToListAsync();

            var listMemberConnection = listMember.Select(m => m.UserId).ToList();

            var userConnections = await onlineSMSContext.UserConnection.Where(u => listMemberConnection.Contains(u.UserId)).ToListAsync();

            var listConnection = userConnections.Select(c => c.ConnectionId).ToList();


            await Clients.Clients(listConnection).SendAsync("ListenPickUp", new
            {
                boxchatId = boxChatId,
                userId
            });
        }

        public async Task Cancel(CallTo callTo)
        {
            string userId = GetUserId();
            string boxChatId = callTo.BoxchatId;

            var listMember = await onlineSMSContext.MemberBoxchat
                .Where(m => m.BoxchatId == boxChatId && m.UserId != userId).ToListAsync();

            var listMemberConnection = listMember.Select(m => m.UserId).ToList();

            var userConnections = await onlineSMSContext.UserConnection.Where(u => listMemberConnection.Contains(u.UserId)).ToListAsync();

            var listConnection = userConnections.Select(c => c.ConnectionId).ToList();

            await Clients.Clients(listConnection).SendAsync("ListenCancel", new
            {
                boxchatId = boxChatId,
                userId
            });

        }

        public async Task SendRoom(CallTo callTo)
        {
            string userId = GetUserId();
            string boxChatId = callTo.BoxchatId;
            string roomId = callTo.RoomId;

            var listMember = await onlineSMSContext.MemberBoxchat
                .Where(m => m.BoxchatId == boxChatId && m.UserId != userId).ToListAsync();

            var listMemberConnection = listMember.Select(m => m.UserId).ToList();

            var userConnections = await onlineSMSContext.UserConnection.Where(u => listMemberConnection.Contains(u.UserId)).ToListAsync();

            var listConnection = userConnections.Select(c => c.ConnectionId).ToList();

            await Clients.Clients(listConnection).SendAsync("ListenSendRoom", new
            {
                boxchatId = boxChatId,
                roomId
            });
        }

        public async Task ReloadBoxchats(CallTo callTo)
        {
            var userTargetId = callTo.UserTargetId;
            var listUserTargetId = callTo.ListUserTargetId;

            List<string> listConnection = new List<string>();

            if (userTargetId != null)
            {
                var userConnections = await onlineSMSContext.UserConnection
                .Where(u => u.UserId == userTargetId)
                .ToListAsync();

                listConnection = userConnections.Select(c => c.ConnectionId).ToList();
            }

            if(listUserTargetId != null)
            {
                Console.WriteLine("==================");
                Console.WriteLine("Create Group.");
                var userConnections = await onlineSMSContext.UserConnection
                .Where(u => listUserTargetId.Contains(u.UserId))
                .ToListAsync();

                listConnection = userConnections.Select(c => c.ConnectionId).ToList();
            }

            await Clients.Clients(listConnection).SendAsync("ListenReloadBoxchats", new { });
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
