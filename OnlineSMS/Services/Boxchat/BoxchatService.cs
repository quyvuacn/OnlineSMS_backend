using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Controllers;
using OnlineSMS.Data;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;
using System.Collections.Generic;
using static OnlineSMS.Constants;

namespace OnlineSMS.Services.Boxchat
{
    public class BoxchatService
    {
        private readonly OnlineSMSContext context;
        private readonly UserManager<User> userManager;
        public BoxchatService(OnlineSMSContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
            
        }


        public async Task<RequestResult> GetBoxchats(string userId) 
        {

            var user = await userManager.FindByIdAsync(userId);

            var listBoxchatId = await context.MemberBoxchat.Where(m => m.UserId == userId).ToListAsync();

            List<string> arrBoxChatId = listBoxchatId.Select(m => m.BoxchatId).ToList();

            var listBoxchat = context.Boxchat
                .Select(b => new
                {
                    BoxchatId = b.Id,
                    b.Name,
                    b.Type,
                    b.Avatar,
                    b.LastUserSendId,
                    b.LastUserSendFullName,
                    b.LastMessageContent,
                    UnreadMessages = b.UnreadMessages.Where(m=> m.UserId == userId && arrBoxChatId.Contains(m.BoxchatId)).Count(),
                    MemberChats = b.MemberBoxes.Select(m => new
                    {
                        UserId = m.User.Id,
                        m.BoxchatId,
                        m.Nickname,
                        m.TimeOfLastMessage,
                        m.User.UserProfile.FullName,
                        m.User.PhoneNumber,
                        m.User.UserProfile.Avatar,
                    }).ToList(),
                    

                }).Where(b => arrBoxChatId.Contains(b.BoxchatId));

            return new RequestResult
            {
                IsSuccess= true,
                Data = listBoxchat
            };
        }

        public async Task<RequestResult> GetMessages(string boxchatId, string userId)
        {
            var boxChat = await context.MemberBoxchat.Where(m => m.UserId == userId && m.BoxchatId == boxchatId).FirstOrDefaultAsync();

            if(boxChat == null)
            {
                return new RequestResult
                {
                    Message = "Không tồn tại"
                };
            }


            var listMessage = await context.Message
                .Select(m => new
                {
                    m.BoxchatId,
                    m.UserSendId,
                    UserAvatar = m.UserSend.UserProfile.Avatar,
                    m.UserFullName,
                    m.Content,
                    m.Type,
                    m.Status,
                    m.StartDate,
                    m.EndDate,
                })
                .Where(m => m.BoxchatId == boxchatId)
                .OrderByDescending(m => m.StartDate)
                .ToListAsync();

            return new RequestResult
            {
                IsSuccess= true,
                Data = new
                {
                   boxchatId,
                   listMessage
                }
            };
        }

        public async Task<RequestResult> SetReadAllMessasesBoxchat(string userId, string boxchatId)
        {
            var unreadMessages = await context.UnreadMessage
                .Where(m => m.BoxchatId == boxchatId && m.UserId == userId)
                .ToListAsync();

            context.UnreadMessage.RemoveRange(unreadMessages);
            await context.SaveChangesAsync();

            return new RequestResult
            {
                IsSuccess = true
            };
        }

        public async Task<RequestResult> CreateGroup(CreateGroup group)
        {
            var members = group.Members.ToList();

            Models.Boxchat boxchat = new Models.Boxchat
            {
                Type = BoxChatType.Group,
                Avatar = group.Avatar,
                Name = group.GroupName
            };

            foreach (var item in members)
            {
                Console.WriteLine($"userId: {item}");
            }

            var listMemberBoxchat = members.Select(userId => new MemberBoxchat
            {
                BoxchatId = boxchat.Id,
                UserId = userId
            });

            Console.WriteLine("=========================");
            Console.WriteLine($"BoxchatId : {boxchat.Id}");

            await context.Boxchat.AddAsync(boxchat);
            await context.MemberBoxchat.AddRangeAsync(listMemberBoxchat);

            await context.SaveChangesAsync();

            return new RequestResult 
            {
                IsSuccess = true 
            };

        }
    }



}
