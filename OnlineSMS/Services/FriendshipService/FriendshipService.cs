using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineSMS.Data;
using OnlineSMS.Models;
using OnlineSMS.RequestModels;
using static OnlineSMS.Constants;

namespace OnlineSMS.Services.FriendshipService
{
    public class FriendshipService
    {
        private readonly UserManager<User> userManager;
        private readonly OnlineSMSContext context;

        public FriendshipService(UserManager<User> userManager, OnlineSMSContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<RequestResult> FindPhoneNumber(string phoneNumber,string seekerId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            UserProfile userProfile = null;

            if (user!=null)
            {
                userProfile = await context.UserProfile.Where(p => p.UserId == user.Id).FirstAsync();
            }
            var friendship = await context.Friendship
                .Where(f => (f.UserAcceptId == seekerId && f.UserRequestId == user.Id)
                        || (f.UserAcceptId == user.Id && f.UserRequestId == seekerId)).FirstOrDefaultAsync();
            
            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
                Data = new {
                    userProfile.Avatar,
                    userProfile.UserId,
                    userProfile.FullName,
                    user.PhoneNumber,
                    Status = friendship == null ? StatusFriendship.Cancelled : friendship.Status ,
                    UserAcceptId = friendship == null ? null :  friendship.UserAcceptId,
                    UserRequestId = friendship == null ? null : friendship.UserRequestId,
                    myUserId = seekerId,
                    isYourself = seekerId == user.Id
                }
            };
        }

        public async Task<RequestResult> AddFriend(FriendRequest friendRequest)
        {
            Friendship friendship = await context.Friendship
                .Where(f => f.UserAcceptId == friendRequest.UserAcceptId && f.UserRequestId == friendRequest.UserRequestId).FirstOrDefaultAsync();
            if (friendship == null)
            {
                friendship = new Friendship
                {
                    UserAcceptId = friendRequest.UserAcceptId,
                    UserRequestId = friendRequest.UserRequestId,
                    Status = StatusFriendship.Pending
                };
                context.Friendship.Add(friendship);
            }
            else
            {
                friendship.Status = StatusFriendship.Pending;
                context.Friendship.Update(friendship);
            }

            await context.SaveChangesAsync();

            return new RequestResult
            {
                IsSuccess = true,
                Message = "Ok!",
                Data = new
                {
                    Status = StatusFriendship.Pending
                }
            };
        }

        public async Task<RequestResult> DeleteFriendRequest(FriendRequest friendRequest)
        {
            Friendship friendship = await context.Friendship
                .Where(f => f.UserAcceptId == friendRequest.UserAcceptId && 
                        f.UserRequestId == friendRequest.UserRequestId)
                .FirstOrDefaultAsync();

            context.Friendship.Remove(friendship);

            await context.SaveChangesAsync();


            return new RequestResult
            {
                IsSuccess = friendship != null ,
                Message = "",
                Data = new
                {
                    Status = StatusFriendship.Cancelled
                }
            };
        }
        public async Task<RequestResult> AgreeFriendRequest(FriendRequest friendRequest)
        {
            Friendship friendship = await context.Friendship
                .Where(f => f.UserAcceptId == friendRequest.UserAcceptId &&
                        f.UserRequestId == friendRequest.UserRequestId
                ).FirstOrDefaultAsync();

            if (friendship != null)
            {
                friendship.Status = StatusFriendship.Friend;
                context.Friendship.Update(friendship);
            }

            Models.Boxchat boxchat = new Models.Boxchat
            {
                Type = BoxChatType.Normal,
            };

            MemberBoxchat memberBoxchatAccept = new MemberBoxchat
            {
                BoxchatId = boxchat.Id,
                UserId = friendRequest.UserAcceptId
            };

            MemberBoxchat memberBoxchatRequest = new MemberBoxchat
            {
                BoxchatId = boxchat.Id,
                UserId = friendRequest.UserRequestId
            };

            await context.Boxchat.AddAsync(boxchat);
            await context.MemberBoxchat.AddAsync(memberBoxchatAccept);
            await context.MemberBoxchat.AddAsync(memberBoxchatRequest);

            await context.SaveChangesAsync();

            return new RequestResult
            {
                IsSuccess = friendship != null ,
                Message = "",
                Data = new
                {
                    Status = StatusFriendship.Friend
                }
            };
        }
        public async Task<RequestResult> Unfriend(FriendRequest friendRequest)
        {
            Friendship friendship = await context.Friendship
               .Where(f => 
                       (f.UserAcceptId == friendRequest.UserAcceptId &&
                       f.UserRequestId == friendRequest.UserRequestId)
               ).FirstOrDefaultAsync() ?? await context.Friendship
               .Where(f =>
                       (f.UserAcceptId == friendRequest.UserRequestId &&
                       f.UserRequestId == friendRequest.UserAcceptId)
               ).FirstOrDefaultAsync();

            if (friendship != null)
            {
                context.Friendship.Remove(friendship);
            }

            await context.SaveChangesAsync();


            return new RequestResult
            {
                IsSuccess = friendship != null ,
                Message = "",
                Data = new
                {
                    Status = StatusFriendship.Cancelled
                }
            };
        }

        public async Task<RequestResult> ListFriend(string userId)
        {
            var listFriends = await context.Friendship
               .Where(f => f.Status == StatusFriendship.Friend && (f.UserAcceptId == userId || f.UserRequestId == userId)).ToListAsync();

            var arrFriendId = new List<string>();

            foreach (var fr in listFriends)
            {
                string frId = fr.UserRequestId == userId ? fr.UserAcceptId : fr.UserRequestId;
                arrFriendId.Add(frId);
            }

            var friends = await userManager.Users
                .Include(u=>u.UserProfile)
                .Select(u=>new
                {
                    u.UserProfile,
                })
                .Where(u => arrFriendId.Contains(u.UserProfile.UserId)).ToListAsync();



            return new RequestResult
            {
                IsSuccess = true,
                Message = "",
                Data = new
                {
                    friends
                }
            };
        }


        public async Task<RequestResult> ListFriendRequest(string userId)
        {
            var listFriends = await context.Friendship
               .Where(f => f.UserRequestId == userId && f.Status == StatusFriendship.Pending).ToListAsync();

            var arrFriendId = new List<string>();

            foreach (var fr in listFriends)
            {
                string frId = fr.UserAcceptId;
                arrFriendId.Add(frId);
            }

            var friends = await userManager.Users
                .Include(u => u.UserProfile)
                .Select(u => new
                {
                    u.UserProfile,
                   
                })
                .Where(u => arrFriendId.Contains(u.UserProfile.UserId)).ToListAsync();


            return new RequestResult
            {
                IsSuccess = true,
                Message = "",
                Data = new
                {
                    friends
                }
            };
        }

        public async Task<RequestResult> ListFriendAccept(string userId)
        {
            var listFriends = await context.Friendship
               .Where(f => f.UserAcceptId == userId && f.Status == StatusFriendship.Pending).ToListAsync();

            var arrFriendId = new List<string>();

            foreach (var fr in listFriends)
            {
                string frId =  fr.UserRequestId;
                arrFriendId.Add(frId);

            }

            var friends = await userManager.Users
                .Include(u => u.UserProfile)
                .Select(u => new
                {
                    u.UserProfile
                })
                .Where(u => arrFriendId.Contains(u.UserProfile.UserId)).ToListAsync();

            

            return new RequestResult
            {
                IsSuccess = true,
                Message = "",
                Data = new
                {
                    friends
                }
            };
        }

    }
}
