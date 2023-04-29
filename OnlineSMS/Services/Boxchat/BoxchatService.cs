using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Data;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;
using System.Collections.Generic;

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
                    lastMessage = b.Messages.OrderByDescending(m=> m.StartDate).LastOrDefault().Content,
                    b.Name,
                    b.Type,
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



    }



}
